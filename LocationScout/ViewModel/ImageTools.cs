using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LocationScout.ViewModel
{
    internal class ImageTools
    {
        internal static byte[] FileToByteArray(string fileName)
        {
            byte[] fileData = null;

            using (FileStream fs = File.OpenRead(fileName))
            {
                var binaryReader = new BinaryReader(fs);
                fileData = binaryReader.ReadBytes((int)fs.Length);
            }
            return fileData;
        }

        internal static BitmapImage ByteArrayToBitmapImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        internal static byte[] BitmapImageToByteArray(BitmapImage image)
        {
            if (image == null) return null;

            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            try
            {
                encoder.Frames.Add(BitmapFrame.Create(image));
            }
            catch (NotSupportedException)
            {
                // that is not a problem
            }

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        internal static bool SamePhoto(byte[] photo1, byte[] photo2)
        {
            // both are null, so return true
            if (photo1 == null && photo2 == null) return true;

            // one is null, the other not null, so return false;
            if (((photo1 == null) && (photo2 != null)) || ((photo1 != null) && (photo2 == null))) return false;

            // els do a real compare
            return photo1.SequenceEqual(photo1);
        }

        internal static bool SamePhoto(BitmapImage photo1, BitmapImage photo2)
        {
            return SamePhoto(BitmapImageToByteArray(photo1), BitmapImageToByteArray(photo2));
        }
    }
}
