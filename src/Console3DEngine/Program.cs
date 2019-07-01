using Console3DEngineLibrary;
using Console3DEngineLibrary.Map;

namespace Console3DEngineUI
{
    internal static class Program
    {
        private static void Main()
        {
            var mapLines = new[]
            {
                "xxxxxxxxxxxxxxxxxxxx",
                "x y                x",
                "x    y        y    x",
                "x y                x",
                "x      zzzzz       x",
                "x   y  z   z       x",
                "x      z   z       x",
                "x  y   zz zz   x   x",
                "x      y           x",
                "x            y     x",
                "x      y     y     x",
                "xxxxxxxxxxxxxxxxxxxx"
            };

            var mapConfig = new MapConfig
            {
                EmptyChar = ' ',
                PlayerChar = 'p'
            };
            var fov = 90;
            var game = Game.StartNewGame(ASCIIMap.FromLines(mapConfig, mapLines), fov);
        }
    }
}