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
            FileManager.UoFolderPath = @"E:\Giochi\Ultima Online Classic ORION";

            Stopwatch w = Stopwatch.StartNew();

            Art.Load();
            BodyDef.Load();
            GraphicHelper.Load();
            Animations.Load();

            ushort[] pixels = Art.ReadStaticArt(19781);


            GraphicHelper.HasBody(46);

            int hue = 38;
            var animation = Animations.GetAnimation(400, 0, 0, ref hue);

            //UOFileUop animSeq = new UOFileUop(Path.Combine(FileManager.UoFolderPath, "AnimationSequence.uop"), ".bin", 2048);

            Fonts.Load();
            var a = Fonts.GetASCII(0).GetChar('A');



            Console.WriteLine(w.ElapsedMilliseconds + " ms");
            Console.ReadLine();
        }
    }
}
