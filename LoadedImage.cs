using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TotalCopperArea
{
    public class LoadedImage
    {
        public string name;
        public Bitmap image;
        public TextBlock myTextBlock;

        public double dpi;
        public double iDPI;

        public ulong totalPixels;
        public ulong pixelsCounted;

        public ulong pixWhite;
        public ulong pixBlack;
    }
}
