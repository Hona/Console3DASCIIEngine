using Console3DEngineLibrary.Map;
using Console3DEngineLibrary.Rendering;

namespace Console3DEngineLibrary
{
    public class Game
    {
        private readonly GameLoop _gameLoop;

        private Game(ASCIIMap map, double fov)
        {
            var renderer = new Renderer(new RendererConfig());

            _gameLoop = new GameLoop(renderer, map, fov);
        }

        public static Game StartNewGame(ASCIIMap map, double fov)
        {
            var game = new Game(map, fov);
            game._gameLoop.Start();
            return game;
        }
    }
}