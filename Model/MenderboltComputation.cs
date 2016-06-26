/// Author: Kurt.Degiorgio
///

using System.Numerics;

namespace ImperativeMendalBrotSet.Model
{
    // MenderBlot Class: 
    // Hosts functions that calculate the Mandelbrot set.
    public static class MenderboltComputation
    {
        /// <summary>
        ///  Calculates XScalar for the image. 
        ///  This used for the zooming process to scale
        ///  The number of points available in particular area.
        /// </summary>
        /// <param name="image"> Image for which scalar will be calculated  </param>
        /// <returns> X Scalar </returns>
        public static double CalculateXScalar(FractalImage image)
        {
            return (image.ViewPoint.Right - image.ViewPoint.Left) / image.Width;
        }

        /// <summary>
        ///  Calculates YScalar for the image. 
        ///  This used for the zooming process to scale
        ///  The number of points available in particular area.
        /// </summary>
        /// <param name="image"> Image for which scalar will be calculated </param>
        /// <returns> X Scalar </returns>
        public static double CalculateYScalar(FractalImage image)
        {
            return  (image.ViewPoint.Top - image.ViewPoint.Bottom) / image.Height;
        }

        /// <summary>
        /// Calculates the eligibility of membership of complex number 
        /// to the Mandelbrot set using the complex number class.
        /// </summary>
        /// <param name="c">Complex number to be tested</param>
        /// <param name="MaxIterations"> max number of iterations</param>
        /// <param name="lastZ"> Last Z parameter used for certain colour schemes </param>
        /// <returns>Number of iterations taken to determine membership </returns>
        public static long Compute(Complex c, long MaxIterations, out Complex lastZ)
        {
            Complex z = Complex.Zero;
            long iteration;

            lastZ = 0;

            for (iteration = 0; iteration < MaxIterations && z.Magnitude < 4; iteration++)
            {
                z = z * z + c;
                iteration++;
            }

            lastZ = z;

            return iteration;
        }

        /// <summary>
        /// Calculates the eligibility of membership of complex number 
        /// to the Mandelbrot without using the complex number class.
        /// </summary>
        /// <param name="realC"> Real part of the complex number </param>
        /// <param name="imaginaryC"> Imaginary part of the complex number </param>
        /// <param name="MaxIterations"> Max number of iterations</param> 
        /// <returns>Number of iterations taken to determine membership</returns>
        public static long Compute(double realC, double imaginaryC, long MaxIterations)
        {
            double zReal, ZImaginary, old;
            zReal = ZImaginary = old = 0;

            long iteration = 0;

            while (iteration < MaxIterations && ((zReal * zReal) + (ZImaginary + ZImaginary)) <= 4)
            {
                zReal = zReal * zReal - ZImaginary * ZImaginary + realC;
                ZImaginary = 2 * old * ZImaginary + imaginaryC;
                old = zReal;
                iteration++;
            }

            return iteration;
        }
    }
}
