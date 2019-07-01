using System;
using System.Collections.Generic;

namespace Console3DEngineLibrary.Rendering
{
    internal class RendererConfig
    {
        public Dictionary<char, ConsoleColor> ForegroundColors { get; set; }
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

        public ConsoleColor GetColorOrDefault(char key)
        {
            // TODO: Potentially abstract the default values to a static instance

            // Check null
            if (ForegroundColors == null) return ConsoleColor.White;

            // Check if entry exists
            return ForegroundColors.TryGetValue(key, out var output) ? output : ConsoleColor.White;
        }
    }
}