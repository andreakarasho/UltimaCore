using System;
using System.Drawing;
using System.IO;
using UltimaCore;
using UltimaCore.Graphics;

namespace Sample
{
    class Program
    {
        //http://wpdev.sourceforge.net/docs/formats/csharp/art.html
        //https://uo.stratics.com/heptazane/fileformats.shtml#3.3

        private static string _path = @"E:\Giochi\Ultima Online Classic ORION";

        static void Main(string[] args)
        {
            FileManager.UoFolderPath = _path;

            Art.Load();
            ushort[] pixels = Art.ReadStaticArt(19781);

        }
    }
}
