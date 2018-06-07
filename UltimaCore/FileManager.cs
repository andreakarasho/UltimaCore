using System;
using System.Collections.Generic;
using System.Text;
using UltimaCore.Fonts;
using UltimaCore.Graphics;

namespace UltimaCore
{
    public static class FileManager
    {
        public static string UoFolderPath { get; set; }

        public static void LoadFiles()
        {
            Art.Load();
            BodyDef.Load();
            GraphicHelper.Load();
            Cliloc.Load();
            Animations.Load();
            Gumps.Load();
            Fonts.Fonts.Load();
        }
    }
}
