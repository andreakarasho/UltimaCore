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

            ushort[] pixels = Art.ReadStaticArt(8298, out short width, out short height);
            ushort[] landpixels = Art.ReadLandArt(3);

            GraphicHelper.HasBody(46);

            int hue = 38;
            // 1254 = wild tiger
            //var animation = Animations.GetAnimation(1421, 0, 0, ref hue);
            /*var animation1 = Animations.GetAnimation(1253, 0, 0, ref hue);
            var animation2 = Animations.GetAnimation(1250, 0, 0, ref hue);*/

            var a = Fonts.GetASCII(0).GetChar('A');

            ushort[] pixelsgump = Gumps.GetGump(0x1393, out int widthgump, out int heightgump);

            Map.Felucca.Load();
            int x = 1201;
            int y = 1694;

            for (int i = 0, oy = y; i < 10; i++, oy++)
            {
                for (int j = 0, ox = x; j < 10; j++, ox++)
                {
                    var aa = Map.Felucca.GetRenderedBlock(ox, oy, 24, 24);
                }
            }

            Map.Felucca.Unload();

            Map.Malas.Load();
            x = 644;
            y = 480;
            var aaa = Map.Malas.GetRenderedBlock(x, y, 24, 24);

            MultiComponentList components = Multi.GetMulti(50);

            var skill = Skills.GetSkill(2);
            
            Console.WriteLine(w.ElapsedMilliseconds + " ms");
            Console.ReadLine();
        }
    }
}
