/// Author: Kurt.Degiorgio
///

using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImperativeMendalBrotSet.Model
{
    /// <summary>
    ///  Describes a single generated Mandelbrot image.
    /// </summary>
    public class FractalImage
    {
        /// When images are stored in memory there is extra byte paddings after pixels row.
        /// (These are done for computational reasons). The stride determines the number of bytes
        /// that are added after each row. It is needed to determine the location of the pixel on the bitmap.
        /// See https://msdn.microsoft.com/en-us/library/windows/desktop/aa473780%28v=vs.85%29.aspx  for details.
        private int _stride;
        private byte[] _rawPixels;
        private WriteableBitmap _bitMap;
        private int _imageWidth, _imageHeight, _bytesPixel;

        /// Pixels in the image.
        public byte[] RawPixels { get { return _rawPixels; } }

        /// Image dimensions.
        public int Height { get { return _imageHeight; } }
        public int Width { get { return _imageWidth; } }

        /// Bytes required to represent pixel in memory.
        public int BytesPixel { get { return _bytesPixel; } }

        /// Actual generated BitMap object.
        public WriteableBitmap BitMap { get { return _bitMap; } }

        /// Current view point.
        public Rect ViewPoint { get; set; }

        /// ZoomIndex
        public int ZoomIndex { get; set; }

        /// <summary>
        /// Default image constructor.
        /// </summary>
        /// <param name="imageWidth"> Height of the image </param>
        /// <param name="imageHeight"> Width of the image </param>
        public FractalImage(int imageWidth, int imageHeight)
        {
            ZoomIndex = 0;
            _imageHeight = imageHeight;
            _imageWidth = imageWidth;

            // Creating bitMap object.
            // We will use BGRA32 format for the pixel colour information
            // as this natively supported by Windows.
            // (See: https://msdn.microsoft.com/en-us/library/aa346378%28v=vs.110%29.aspx)
            _bitMap = new WriteableBitmap(imageWidth, imageHeight, 96, 96, PixelFormats.Bgra32, null);

            // Calculate Bytes per pixel  and raw pixel array.
            _bytesPixel = BitMap.Format.BitsPerPixel / 8;
            _rawPixels = new byte[Height * Width * _bytesPixel];
            _stride = _imageWidth * _bytesPixel;
        }

        /// <summary>
        /// Copy constructor that is used
        /// to create a by-value copy. 
        /// </summary>
        /// <param name="instance"></param>
        public FractalImage(FractalImage instance)
        {
            this._imageHeight = instance._imageHeight;
            this._imageWidth = instance._imageWidth;
            this._stride = instance._stride;
            this._bitMap = instance._bitMap.Clone();
            this._rawPixels = (byte[])instance._rawPixels.Clone();
            this.ViewPoint = instance.ViewPoint;
        }

        /// <summary>
        /// Exports image to Disk as PNG.
        /// </summary>
        /// <param name="filePath"> image path</param>
        public void ExportImage(string filePath)
        {
            using (var filestream = new FileStream(filePath, FileMode.Create))
            {
                PngBitmapEncoder png_encoder = new PngBitmapEncoder();
                png_encoder.Frames.Add(BitmapFrame.Create(BitMap));
                png_encoder.Save(filestream);
            }
        }

        /// <summary>
        ///  Colours a single pixel
        /// </summary>
        /// <param name="index"> Index of the pixel in the array </param>
        /// <param name="colorInformation"></param>
        public void ColorPixel(long index, Color colorInformation)
        {
            if (index + 4 > RawPixels.Length || index < 0)
                new Exception("Error: Accessing Invalid location in array");

            RawPixels[index] = colorInformation.B;
            RawPixels[index + 1] = colorInformation.G;
            RawPixels[index + 2] = colorInformation.R;
            RawPixels[index + 3] = colorInformation.A;
        }

        /// <summary>
        /// Writes a current Raw Byte array to the image.
        /// </summary>
        public void WritePixelsToImage()
        {
            Int32Rect rect = new Int32Rect(0, 0, Width, Height);
            BitMap.WritePixels(rect, RawPixels, _stride, 0);
        }

        /// <summary>
        /// Calculates the location of the pixel on the screen
        /// We must take into account the stride buffer.
        /// </summary>
        public void CalculatePixelCoordinates(int pixelIndex, out int ycord, out int xcord)
        {
            ycord = pixelIndex / _stride;
            xcord = pixelIndex % _stride / BytesPixel;
        }
    }
}
