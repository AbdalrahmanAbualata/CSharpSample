using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace CSharpSample
{
    public class ImageHelper
    {
        /// <summary>
        ///   Creates the image from image bytes.
        /// </summary>
        /// <param name = "imageData">The image data.</param>
        /// <returns></returns>
        public static Image CreateImageFromImageBytes(byte[] imageData)
        {
            Image image;
            using (MemoryStream inStream = new MemoryStream())
            {
                inStream.Write(imageData, 0, imageData.Length);

                image = Image.FromStream(inStream, true, false);
            }

            return image;
        }

        /// <summary>
        ///   Convert Image type to Bmp
        /// </summary>
        /// <param name = "image">image to be converted to byte array</param>
        /// <returns>byte array of converted image</returns>
        public static byte[] ImageToBmp(Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Bmp);

            return ms.ToArray();
        }

        /// <summary>
        ///   Convert Bmp to Image type
        /// </summary>
        /// <param name = "bmp">BMP image in byte[] format</param>
        /// <returns></returns>
        public static Image BmpToImage(byte[] bmp)
        {
            if (bmp == null)
                return null;

            MemoryStream ms = new MemoryStream(bmp);
            ms.Seek(0, 0);
            return Image.FromStream(ms);
        }

        /// <summary>
        ///   Convert Image type to raw 8-bit byte array
        /// </summary>
        /// <param name = "image">input image </param>
        /// <param name = "width">width of the image</param>
        /// <param name = "height">height of the image</param>
        /// <returns></returns>
        public static byte[] ImageToRaw8BitByteArray(Image image, out int width, out int height)
        {
            Bitmap bitmapImage = (Bitmap)image;

            BitmapData bitmapData = bitmapImage.LockBits(new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height),
                                                         ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

            int size = bitmapData.Width * bitmapData.Height;

            width = bitmapData.Width;
            height = bitmapData.Height;

            byte[] rawImage = new byte[size];

            Marshal.Copy(bitmapData.Scan0, rawImage, 0, size);

            bitmapImage.UnlockBits(bitmapData);

            return rawImage;
        }

        /// <summary>
        ///   Convert RAW 8-bit byte array to Image type
        /// </summary>
        /// <param name = "byteArray">RAW image in byte array format</param>
        /// <param name = "width">width of the raw image</param>
        /// <param name = "height">height of the raw image</param>
        /// <returns>converted image</returns>
        public static Image Raw8BitByteArrayToImage(byte[] byteArray, int width, int height)
        {
            if ((byteArray == null) || (width <= 0) || (height <= 0))
                return null;

			int iImageWidth = ((width + 3) / 4) * 4;
			int iImageHeight = ((height + 3) / 4) * 4;
			byte[] byImage = new byte[iImageWidth * iImageHeight];

			for (int i = 0; i < height; i++)
			{
				Array.Copy((byte[])byteArray, i * width, byImage, i * iImageWidth,
						   width);
			}

			Bitmap bmp = new Bitmap(iImageWidth, iImageHeight, PixelFormat.Format8bppIndexed);
            ColorPalette palette = bmp.Palette;

            for (int index = 0; index < palette.Entries.Length; index++)
                palette.Entries[index] = Color.FromArgb(index, index, index);

            bmp.Palette = palette;

            BitmapData bData = bmp.LockBits(new Rectangle(new Point(), bmp.Size),
                                            ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // Copy the bytes to the bitmap object
			Marshal.Copy(byImage, 0, bData.Scan0, byImage.Length);

            bmp.UnlockBits(bData);

            return bmp;
        }
    }
}