using System;
using System.Diagnostics;
using Console3DEngineLibrary.Map;
using Console3DEngineLibrary.Rendering;

namespace Console3DEngineLibrary
{
    internal class GameLoop
    {
        private readonly Stopwatch _diagnosticStopwatch = new Stopwatch();
        private readonly Stopwatch _frameStopwatch = new Stopwatch();
        private readonly ASCIIMap _map;
        private readonly Renderer _renderer;
        private readonly Camera _camera;

        private double _currentFrameTime;
        private bool _done;
        private string _drawTime = string.Empty;
        private string _inputTime = string.Empty;

        public GameLoop(Renderer renderer, ASCIIMap map, double fov)
        {
            _renderer = renderer;
            _map = map;

            _camera = new Camera(fov)
            {
                PositionVector = new Vector
                {
                    X = (double) map.MapNodes.GetLength(0) / 2, Y = (double) map.MapNodes.GetLength(1) / 2
                }
            };
        }

        //speed modifiers
        private double MoveSpeed => 40 * _currentFrameTime; // The constant value is in squares/second
        private double RotationSpeed => 20 * _currentFrameTime; // The constant value is in radians/second

        internal void Start()
        {
            _renderer.Clear();

            while (!_done)
            {
                // Delta time for each frame
                _frameStopwatch.Restart();

                // Clear the console - without it performance is much worse!??
                _renderer.Clear();

                _diagnosticStopwatch.Restart();

                // Cast a ray for every column on the screen
                for (var x = 0; x < _renderer.ScreenWidth; x++) RayCast(x);

                _diagnosticStopwatch.Stop();
                var forLoopTime = _diagnosticStopwatch.ElapsedMilliseconds + " ms";

                _frameStopwatch.Stop();
                _currentFrameTime = (double) _frameStopwatch.ElapsedTicks / Stopwatch.Frequency;

                _diagnosticStopwatch.Restart();
                _renderer.DrawFPS(1.0 / _currentFrameTime);
                _renderer.DrawMap(_map.MapNodes, _camera.PositionVector);
                _renderer.DrawDebug($"InputTime: {_inputTime}, DrawTime: {_drawTime}, ForLoopTime: {forLoopTime}");
                _renderer.Flush();
                _diagnosticStopwatch.Stop();
                _drawTime = _diagnosticStopwatch.ElapsedMilliseconds + " ms";

                _diagnosticStopwatch.Restart();
                ProcessInput();
                _diagnosticStopwatch.Stop();
                _inputTime = _diagnosticStopwatch.ElapsedMilliseconds + " ms";
            }
        }

        private void RayCast(int screenX)
        {
            // Ray position and direction

            // X coordinate in the camera space
            var cameraX = 2 * screenX / (double) _renderer.ScreenWidth - 1;
            var rayDirectionVector = new Vector
            {
                X = _camera.DirectionVector.X + _camera.CameraPlaneVector.X * cameraX,
                Y = _camera.DirectionVector.Y + _camera.CameraPlaneVector.Y * cameraX
            };

            var mapX = (int) _camera.PositionVector.X;
            var mapY = (int) _camera.PositionVector.Y;

            var distanceFromSideVector = new Vector();

            var deltaDistanceFromSideVector = new Vector
            {
                X = Math.Abs(1 / rayDirectionVector.X),
                Y = Math.Abs(1 / rayDirectionVector.Y)
            };

            double perpendicularWallDistance;

            int stepX, stepY;
            var hit = false;
            var side = 0;

            if (rayDirectionVector.X < 0)
            {
                stepX = -1;
                distanceFromSideVector.X = (_camera.PositionVector.X - mapX) * deltaDistanceFromSideVector.X;
            }
            else
            {
                stepX = 1;
                distanceFromSideVector.X =
                    (mapX + 1.0 - _camera.PositionVector.X) * deltaDistanceFromSideVector.X;
            }

            if (rayDirectionVector.Y < 0)
            {
                stepY = -1;
                distanceFromSideVector.Y = (_camera.PositionVector.Y - mapY) * deltaDistanceFromSideVector.Y;
            }
            else
            {
                stepY = 1;
                distanceFromSideVector.Y =
                    (mapY + 1.0 - _camera.PositionVector.Y) * deltaDistanceFromSideVector.Y;
            }

            while (!hit)
            {
                if (distanceFromSideVector.X < distanceFromSideVector.Y)
                {
                    distanceFromSideVector.X += deltaDistanceFromSideVector.X;
                    mapX += stepX;
                    side = 0;
                }
                else
                {
                    distanceFromSideVector.Y += deltaDistanceFromSideVector.Y;
                    mapY += stepY;
                    side = 1;
                }

                // Check if the ray has hit a wall
                if (_map.MapNodes[mapX, mapY] != _map.Config.EmptyChar) hit = true;
            }

            if (side == 0)
                perpendicularWallDistance =
                    (mapX - _camera.PositionVector.X + (double) (1 - stepX) / 2) / rayDirectionVector.X;
            else
                perpendicularWallDistance =
                    (mapY - _camera.PositionVector.Y + (double) (1 - stepY) / 2) / rayDirectionVector.Y;

            var lineHeight = (int) (_renderer.ScreenHeight / perpendicularWallDistance);

            var drawStart = -lineHeight / 2 + _renderer.ScreenHeight / 2;
            if (drawStart < 0) drawStart = 0;
            var drawEnd = lineHeight / 2 + _renderer.ScreenHeight / 2;
            if (drawEnd >= _renderer.ScreenHeight) drawEnd = _renderer.ScreenHeight - 1;

            var color = ConsoleColor.DarkGreen;

            switch (_map.MapNodes[mapX, mapY])
            {
                case 'x':
                    if (side == 1)
                        color = ConsoleColor.Green;
                    break;
                case 'y':
                    color = ConsoleColor.Blue;
                    if (side == 1)
                        color = ConsoleColor.DarkBlue;
                    break;
                case 'z':
                    color = ConsoleColor.Yellow;
                    if (side == 1)
                        color = ConsoleColor.DarkYellow;
                    break;
            }

            _renderer.DrawColumn(screenX, drawStart, drawEnd, color);
        }

        private void ProcessInput()
        {
            while (System.Console.KeyAvailable)
            {
                var key = _renderer.ReadKey(true);

                switch (key.Key)
                {
                    //move forward if no wall in front of you
                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        MovePlayer(true);
                        break;

                    //move backwards if no wall behind you
                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        MovePlayer(false);
                        break;

                    //rotate to the right
                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        RotateCamera(true);
                        break;

                    //rotate to the left
                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        RotateCamera(false);
                        break;
                }
            }
        }

        private void RotateCamera(bool rightDirection)
        {
            var rotationSpeed = RotationSpeed * (rightDirection ? -1 : 1);
            //both camera direction and camera plane must be rotated
            var oldDirX = _camera.DirectionVector.X;
            _camera.DirectionVector.X = _camera.DirectionVector.X * Math.Cos(rotationSpeed) -
                                        _camera.DirectionVector.Y * Math.Sin(rotationSpeed);
            _camera.DirectionVector.Y = oldDirX * Math.Sin(rotationSpeed) +
                                        _camera.DirectionVector.Y * Math.Cos(rotationSpeed);
            var oldPlaneX = _camera.CameraPlaneVector.X;
            _camera.CameraPlaneVector.X = _camera.CameraPlaneVector.X * Math.Cos(rotationSpeed) -
                                          _camera.CameraPlaneVector.Y * Math.Sin(rotationSpeed);
            _camera.CameraPlaneVector.Y = oldPlaneX * Math.Sin(rotationSpeed) +
                                          _camera.CameraPlaneVector.Y * Math.Cos(rotationSpeed);
        }

        private void MovePlayer(bool forward)
        {
            var directionVectorDirectionMultiplier = forward ? 1 : -1;
            if (_map.MapNodes[
                    (int) (_camera.PositionVector.X +
                           _camera.DirectionVector.X * directionVectorDirectionMultiplier * MoveSpeed),
                    (int) _camera.PositionVector.Y] ==
                _map.Config.EmptyChar)
                _camera.PositionVector.X += _camera.DirectionVector.X * directionVectorDirectionMultiplier * MoveSpeed;
            if (_map.MapNodes[(int) _camera.PositionVector.X,
                    (int) (_camera.PositionVector.Y +
                           _camera.DirectionVector.Y * directionVectorDirectionMultiplier * MoveSpeed)] ==
                _map.Config.EmptyChar)
                _camera.PositionVector.Y += _camera.DirectionVector.Y * directionVectorDirectionMultiplier * MoveSpeed;
        }
    }
}