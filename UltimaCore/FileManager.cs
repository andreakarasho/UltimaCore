using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UltimaCore.Fonts;
using UltimaCore.Graphics;


namespace UltimaCore
{
    public static class FileManager
    {
        private static string _uofolderpath;

        public static string UoFolderPath
        {
            get => _uofolderpath;
            set
            {
                _uofolderpath = value;
                FileInfo client = new FileInfo(Path.Combine(value, "client.exe"));
                if (!client.Exists)
                    throw new FileNotFoundException();

                var versInfo = FileVersionInfo.GetVersionInfo(client.FullName);

                ClientVersion = (CLIENT_VERSION)((versInfo.ProductMajorPart << 24) | (versInfo.ProductMinorPart << 16) | (versInfo.ProductBuildPart << 8) | (versInfo.ProductPrivatePart));
            }
        }

        public static CLIENT_VERSION ClientVersion { get; private set; }
        
            

        public static void LoadFiles()
        {
            Art.Load();
            BodyDef.Load();
            GraphicHelper.Load();
            Cliloc.Load();
            Animations.Load();
            Gumps.Load();
            Fonts.Fonts.Load();
            Hues.Load();
        }
    }
}
