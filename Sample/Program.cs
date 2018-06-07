using System;
using System.Diagnostics;
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


        static void Main(string[] args)
        {
            FileManager.UoFolderPath = @"E:\Giochi\Ultima Online Classic ORION";

            Stopwatch w = Stopwatch.StartNew();

            Art.Load();
            ushort[] pixels = Art.ReadStaticArt(19781);

            Animations.Load();

            BodyConverter.Load();
            BodyConverter.HasBody(46);

            int hue = 38;
            var animation = Animations.GetAnimation(46, 0, 0, ref hue);

            Console.WriteLine(w.ElapsedMilliseconds + " ms");
            Console.ReadLine();
        }
    }
}
