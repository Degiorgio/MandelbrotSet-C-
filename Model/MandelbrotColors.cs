/// Author: Kurt.Degiorgio
///

using System;
using System.Numerics;
using System.Windows.Media;

namespace ImperativeMendalBrotSet.Model
{
    /// <summary>
    /// Static class that provides
    /// a number of different colour schemes.
    /// </summary>
    static public class MandelbrotColors
    {
        /// <summary>
        /// Very Basic Colour Scheme colour scheme
        /// Points in the set will be simply coloured black
        /// </summary>
        /// <returns> Colour Object </returns>
        public static Color SchemeBlack()
        {
            Color pixelcolor = new Color();

            pixelcolor.R = 255;
            pixelcolor.G = 255;
            pixelcolor.B = 255;
            pixelcolor.A = 255;

            return pixelcolor;
        }

        /// <summary>
        /// Gold non-gradient (discrete function) Colouring Scheme.
        /// </summary>
        /// <param name="iterations"></param>
        /// <returns> Colour </returns>
        public static Color SchemeColorGold(long iterations)
        {
            Color pixelcolor = new Color();
            long i = iterations % 16;
            if (i == 0)
            {
                pixelcolor.R = 40; pixelcolor.G = 30; pixelcolor.B = 25;
            }
            else if (i == 1)
            {
                pixelcolor.R = 26; pixelcolor.G = 30; pixelcolor.B = 25;
            }
            else if (i == 2)
            {
                pixelcolor.R = 43; pixelcolor.G = 30; pixelcolor.B = 25;
            }
            else if (i == 3)
            {
                pixelcolor.R = 100; pixelcolor.G = 7; pixelcolor.B = 26;
            }
            else if (i == 4)
            {
                pixelcolor.R = 4; pixelcolor.G = 1; pixelcolor.B = 83;
            }
            else if (i == 5)
            {
                pixelcolor.R = 50; pixelcolor.G = 4; pixelcolor.B = 63;
            }
            else if (i == 6)
            {
                pixelcolor.R = 10; pixelcolor.G = 7; pixelcolor.B = 120;
            }
            else if (i == 7)
            {
                pixelcolor.R = 50; pixelcolor.G = 82; pixelcolor.B = 177;
            }
            else if (i == 8)
            {
                pixelcolor.R = 43; pixelcolor.G = 181; pixelcolor.B = 29;
            }
            else if (i == 9)
            {
                pixelcolor.R = 99; pixelcolor.G = 236; pixelcolor.B = 248;
            }
            else if (i == 10)
            {
                pixelcolor.R = 33; pixelcolor.G = 33; pixelcolor.B = 191;
            }
            else if (i == 11)
            {
                pixelcolor.R = 25; pixelcolor.G = 221; pixelcolor.B = 95;
            }
            else if (i == 12)
            {
                pixelcolor.R = 255; pixelcolor.G = 43; pixelcolor.B = 0;
            }
            else if (i == 13)
            {
                pixelcolor.R = 102; pixelcolor.G = 63; pixelcolor.B = 0;
            }
            else if (i == 14)
            {
                pixelcolor.R = 92; pixelcolor.G = 61; pixelcolor.B = 0;
            }
            else if (i == 15)
            {
                pixelcolor.R = 73; pixelcolor.G = 92; pixelcolor.B = 3;
            }
            pixelcolor.A = 255;
            return pixelcolor;
        }


        /// <summary> 
        ///  This continues colour scheme that uses the Bernstein set of polynomials
        ///  to calculated the RGBA values. It uses the current iterations and max iterations
        ///  value to normalize the functions.
        /// </summary>
        /// <param name="iterations"> </param>
        /// <param name="max_iter"></param>
        /// <returns></returns>
        public static Color SchemeContinious(double iterations, double max_iter)
        {
            // Calculate interval.
            double x = iterations / max_iter;

            // Using  Bernstein Polynomial (http://mathworld.wolfram.com/BernsteinPolynomial.html).
            // As they perfect properties for use.

            int r = (int) Math.Floor(9 * (1 - x) * x * x * x * 255);
            int g = (int) Math.Floor(14 * (1 - x) * (1 - x) * x * x * 255);
            int b = (int) Math.Floor(8 * (1 - x) * (1 - x) * (1 - x) * x * 255);

            Color pixelcolor = new Color();

            pixelcolor.A = 255;
            pixelcolor.R = (byte) (r * 1.1);
            pixelcolor.G = (byte) (g * 1.1);
            pixelcolor.B = (byte) (b * 1.1);

            return pixelcolor;
        }

        /// <summary>
        /// Smooth red colour scheme.Colr is created based on iteration count
        /// and value of the point in question.
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="z"></param>
        /// <returns> Colour Object </returns>
        public static Color SchemeSmoothRed(long iterations, Complex z)
        {
            Color pixelcolor = new Color();

            double Z = Math.Sqrt(z.Magnitude);

            double log = Math.Log(Math.Log(Z, 2), 2);
            int brightness = (int)(256.0 * Math.Log(1.00 + iterations - Z, 2));

            pixelcolor.A = 255;
            pixelcolor.R = 255;
            pixelcolor.G = (byte) brightness;
            pixelcolor.B = (byte) brightness; 

            return pixelcolor;
        }

        /// <summary>
        /// Creates a wavy smooth colour scheme based on the calculated value
        /// of the point in question and the number of iterations it took 
        /// to calculate it. 
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="z"></param>
        /// <returns> Colour Object </returns>
        public static Color SchemeWaveyBlue(long iterations, Complex z)
        {
            Color pixelcolor = new Color();

            double Z   = Math.Sqrt(z.Imaginary * z.Imaginary + z.Real * z.Real);
            double log = Math.Log(Math.Log(Z, 2), 2);

            int brightness = (int)(256.0 * Math.Log(1.00 + iterations - Z, 2));

            pixelcolor.A = 255;
            pixelcolor.R = (byte) brightness;
            pixelcolor.G = (byte) brightness;
            pixelcolor.B = 200;

            return pixelcolor;
        }
    }
}
