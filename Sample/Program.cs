using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using UltimaCore;
using UltimaCore.Graphics;
using UltimaCore.Fonts;

namespace Sample
{
    class Program
    {
        //http://wpdev.sourceforge.net/docs/formats/csharp/art.html
        //https://uo.stratics.com/heptazane/fileformats.shtml#3.3


        static void Main(string[] args)
        {
            Stopwatch w = Stopwatch.StartNew();

            FileManager.UoFolderPath = @"E:\Giochi\Ultima Online Classic ORION";
            FileManager.LoadFiles();

            ushort[] pixels = Art.ReadStaticArt(19781);


            GraphicHelper.HasBody(46);

            int hue = 38;
            // 1254 = wild tiger
            //var animation = Animations.GetAnimation(1421, 0, 0, ref hue);
            /*var animation1 = Animations.GetAnimation(1253, 0, 0, ref hue);
            var animation2 = Animations.GetAnimation(1250, 0, 0, ref hue);*/

            var a = Fonts.GetASCII(0).GetChar('A');

            ushort[] pixelsgump = Gumps.GetGump(1416);

            Map.Felucca.Load();
            
            Console.WriteLine(w.ElapsedMilliseconds + " ms");
            Console.ReadLine();
        }
    }
}
