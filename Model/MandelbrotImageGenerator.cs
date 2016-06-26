
/// Author: Kurt.Degiorgio
///

using System.Numerics;
using System.Windows.Media;

namespace ImperativeMendalBrotSet.Model
{
    /// <summary>
    /// This class is where the magic happens. It generates the image
    /// Using a number of threads.
    /// </summary>   
    public class MandelbrotImageGenerator : FractalGenerator
    {
        /// <summary>
        /// Process a single segment of the Mandelbrot image.
        /// </summary>
        /// <param name="segmentStart"> Indicates the start of the segment that is to be processed. </param>
        /// <param name="segmentEnd"> Indicates the end of the segment that is to be processed. </param>
        /// <param name="xfactor"> Scaling factor for the real numbers. </param>
        /// <param name="yfactor"> Scaling factor for the Imaginary numbers. </param>
        protected override void ProcessSegment(int segmentStart, int segmentEnd, double xfactor, double yfactor)
        {
            segmentStart = segmentStart * _currentImage.BytesPixel;
            segmentEnd = segmentEnd * _currentImage.BytesPixel;

            for (int pixelIndex = segmentStart; pixelIndex < segmentEnd; pixelIndex += _currentImage.BytesPixel)
            {
                int xcord, ycord;
                Complex c, z;

                if (_cancel)  // User wants to abandon image generation process just break.                 
                    break;

                // Calculate Coordinates of pixel on the screen.
                _currentImage.CalculatePixelCoordinates(pixelIndex, out ycord, out xcord);

                // Scale points according to there positions on the image 
                // and  X and Y factors we just calculated.
                double yscaled = _currentImage.ViewPoint.Top - ycord * yfactor;
                double xscaled = _currentImage.ViewPoint.Left + xcord * xfactor;

                // Do the computation.
                c = new Complex(xscaled, yscaled);
                long count = MenderboltComputation.Compute(c, _maxIterations, out z);

                // Use the iteration and calculated Z value to pick a colour and 
                // Colour the pixel.
                _currentImage.ColorPixel(pixelIndex, SelectColor(count, z));
            }
        }

        /// <summary>
        /// Selects colour information based on the value of the
        /// parameters.
        /// </summary>
        /// <param name="iterations"> Determines colour scheme. </param>
        /// <returns> Colour </returns>
        private Color SelectColor(long iterations, Complex zValue)
        {
            Color pixelcolor;
            if (iterations == _maxIterations)
            {
                pixelcolor = new Color();
                // Point seems to never break out of the set
                // at least for the current setting of _maxIterations
                // hence we colour it black.
                pixelcolor.A = 255;
                pixelcolor.B = pixelcolor.G = pixelcolor.R = 0;
            }
            else
            {
                switch (_colorIndex)
                {
                    case "Wavey Blue":
                        pixelcolor = MandelbrotColors.SchemeWaveyBlue(iterations, zValue);
                        break;
                    case "Smooth Red":
                        pixelcolor = MandelbrotColors.SchemeSmoothRed(iterations, zValue);
                        break;
                    case "Black":
                        pixelcolor = MandelbrotColors.SchemeBlack();
                        break;
                    case "Gold":
                        pixelcolor = MandelbrotColors.SchemeColorGold(iterations);
                        break;
                    default:
                    case "Continious":
                        pixelcolor = MandelbrotColors.SchemeContinious(iterations, _maxIterations);
                        break;
                }
            }
            return pixelcolor;
        }
    }
}
