using System;

namespace Console3DEngineLibrary.Map
{
    public class ASCIIMap
    {
        private ASCIIMap(MapConfig config, char[,] mapNodes)
        {
            Config = config;
            MapNodes = mapNodes;
        }

        internal MapConfig Config { get; }
        internal char[,] MapNodes { get; }

        /// <summary>
        ///     Creates an instance of the class from a string array
        /// </summary>
        public static ASCIIMap FromLines(MapConfig config, params string[] lines)
        {
            // Make sure the input isn't null or empty
            if (lines == null || lines[0]?.Length == 0) throw new ArgumentNullException(nameof(lines));

            // Assume all lines are the same length
            var mapWidth = lines[0].Length;
            var mapHeight = lines.Length;

            // char[x, y]
            var mapNodes = new char[mapWidth, mapHeight];

            // Converts the string array into the 2D char array
            for (var x = 0; x < mapWidth; x++)
            for (var y = 0; y < mapHeight; y++)
                mapNodes[x, y] = lines[y][x];


            return new ASCIIMap(config, mapNodes);
        }
    }
}