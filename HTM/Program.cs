using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using HTM.Models;

namespace HTM
{
    class Program
    {
        static void Main(string[] args)
        {
            CPM cpm = CPM.Instance;

            Console.WriteLine("Enter X:");
            uint x = Convert.ToUInt32(Console.ReadLine());
            Console.WriteLine("Enter Y:");
            uint y = Convert.ToUInt32(Console.ReadLine());
            Console.WriteLine("Enter Z:");
            uint z = Convert.ToUInt32(Console.ReadLine());

            Console.WriteLine("Initializing...");
            CPM.Initialize(x, y, z);
            Console.WriteLine("Done.");

            //Set Up Spatial Input pattern Source

            //Set Up Temporal Input Pattern Source

            //Set Up Apical Input Pattern Source


            Bitmap B = new Bitmap("../../Pictures/a.png");

            int size = B.Width * B.Height;
            int connectionfraction = Convert.ToInt32(ConfigurationSettings.AppSettings.Get("ConnectionFraction"));

            SDR inputsdr = new SDR(x,y);

            /*
             -Join - Associate each of the pixel with its nearby neibhours in its SDR 
             -Set - Associate each of the pixel value in the image to its corresponding attached bits in SDR              
            */

      
            for (int i = 0; i < B.Width; i++)
                for (int j = 0; j < B.Height; j++)
                {

                }

            Bitmap bmp = GetInput();
        }

        public static Bitmap GetInput()
        {
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                               Screen.PrimaryScreen.Bounds.Height,
                               PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            // Save the screenshot to the specified path that the user has chosen.
            //bmpScreenshot.Save("Screenshot.png", ImageFormat.Png);

            return bmpScreenshot;
        }
    }
}
