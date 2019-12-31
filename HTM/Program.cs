using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace HTM
{
    class Program
    {
        static void Main(string[] args)
        {
            SynapseManager cpm = SynapseManager.GetInstance;            

            Console.WriteLine("Enter lenth of the block:");
            uint x = Convert.ToUInt32(Console.ReadLine());
            Console.WriteLine("Enter breadth of the block:");
            uint y = Convert.ToUInt32(Console.ReadLine());
            Console.WriteLine("Enter width of the block:");
            uint z = Convert.ToUInt32(Console.ReadLine());

            Console.WriteLine("Initializing...");
            SynapseManager.Initialize(x, y, z);
            Console.WriteLine("Done.");

            //Set Up Spatial Input pattern Source

            //Set Up Temporal Input Pattern Source

            //Set Up Apical Input Pattern Source


            Bitmap B = GetInput();

            int size = B.Width * B.Height;
            int connectionfraction = Convert.ToInt32(ConfigurationSettings.AppSettings.Get("ConnectionFraction"));

            /*
             -Join - Associate each of the pixel with its nearby neibhours in its SDR 
             -Set - Associate each of the pixel value in the image to its corresponding attached bits in SDR              
            */


      
            for (int i = 0; i < B.Width; i++)
                for (int j = 0; j < B.Height; j++)
                {

                }            
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

            return bmpScreenshot;
        }
    }
}
