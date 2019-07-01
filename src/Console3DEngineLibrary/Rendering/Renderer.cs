using System;
using Console.Abstractions;

namespace Console3DEngineLibrary.Rendering
{
    internal class Renderer
    {
        private readonly RendererConfig _config;
        private readonly BufferedPointConsole _console;

        internal Renderer(RendererConfig config)
        {
            _config = config;
            var systemConsole = new SystemConsole();
            _console = Helpers.CacheEnMasse(systemConsole);
        }

        public int ScreenWidth => _console.Width - 10;
        public int ScreenHeight => _console.Height - 10;

        internal void DrawColumn(int x, int drawStart, int drawEnd, ConsoleColor color)
        {
            if (drawStart < 0 || drawEnd < 0 || drawEnd > ScreenHeight || drawStart > drawEnd)
            {
                drawStart = 0;
                drawEnd = ScreenHeight;
            }

            if (0 < drawStart)
                for (var i = 0; i < drawStart; i++)
                    _console.PutChar('▒', new PutCharData
                    {
                        Background = _config.BackgroundColor,
                        Foreground = i > 10 ? ConsoleColor.Red : ConsoleColor.DarkRed,
                        X = x,
                        Y = i
                    });
            if (drawEnd < ScreenHeight)
                for (var i = drawEnd + 1; i < ScreenHeight; i++)
                    _console.PutChar('▒', new PutCharData
                    {
                        Background = _config.BackgroundColor,
                        Foreground = i < ScreenHeight - 10 ? ConsoleColor.Red : ConsoleColor.DarkRed,
                        X = x,
                        Y = i
                    });
            for (var y = drawStart; y <= drawEnd; y++)
                _console.PutChar('█', new PutCharData
                {
                    Background = _config.BackgroundColor,
                    Foreground = color,
                    X = x,
                    Y = y
                });
        }

        internal void DrawFPS(double fps)
        {
            var output = (Math.Truncate(fps) + " FPS").Trim();
            for (var i = 0; i < output.Length; i++)
                _console.PutChar(output[i], new PutCharData
                {
                    Background = _config.BackgroundColor,
                    Foreground = ConsoleColor.White,
                    X = i,
                    Y = ScreenHeight + 2
                });
        }

        internal void DrawDebug(string debug)
        {
            for (var i = 0; i < debug.Length; i++)
                _console.PutChar(debug[i], new PutCharData
                {
                    Background = _config.BackgroundColor,
                    Foreground = ConsoleColor.White,
                    X = i,
                    Y = ScreenHeight + 3
                });
        }

        internal void DrawMap(char[,] nodes, Vector positionVector)
        {
            var width = nodes.GetLength(0);
            var height = nodes.GetLength(1);

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
            {
                var output = !(y == (int) positionVector.Y && x == (int) positionVector.X) ? nodes[x, y] : 'P';
                _console.PutChar(output, new PutCharData
                {
                    Background = _config.BackgroundColor,
                    Foreground = ConsoleColor.White,
                    X = ScreenWidth - width + x,
                    Y = ScreenHeight - height + y
                });
            }
        }

        internal void Clear()
        {
            _console.Clear(new PutCharData
            {
                Background = ConsoleColor.Black,
                Foreground = ConsoleColor.White
            });
        }

        internal void Flush()
        {
            _console.Flush();
        }

        internal ConsoleKeyInfo ReadKey(bool intercept) => _console.ReadKey(intercept);
    }
}