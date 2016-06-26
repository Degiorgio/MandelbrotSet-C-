/// Author: Kurt.Degiorgio
///

using System;
using System.Threading;

namespace ImperativeMendalBrotSet.Model
{
    /// <summary>
    /// Abstract Class that represents
    /// a generator for a fractal image. Currently
    /// We only support the Mandelbrot set but
    /// In the future other fractals maybe added hence this abstraction
    /// facilitates this.
    /// </summary>
    public abstract class FractalGenerator
    {
        protected bool _cancel;               // If true Image generation will be stopped.
        protected long _maxIterations;        // Current Max Iteration value.
        protected string _colorIndex;         // Current Colour Scheme.
        protected FractalImage _currentImage;   // Image being processed.

        /// <summary>
        /// Cancels the Image Generation process.
        /// Used by the UI when the user wants to stop generating the image.
        /// </summary>
        public void Stop()
        {
            _cancel = true;
        }

        /// <summary>
        /// Creates an Image with certain dimensions.
        /// </summary>
        /// <param name="height"> Height of Image </param>
        /// <param name="width"> Width of Image </param>
        public FractalImage CreateImage(int height, int width)
        {
            return new FractalImage(width, height);
        }

        /// <summary>
        /// Starts the generation of the Mandelbrot image. 
        /// The image will be split into an number of segments
        /// depending on the number of available threads.
        /// </summary>
        /// <param name="image"> The image object which will be generated. </param>
        /// <param name="iterations"> Maximum iterations. </param>
        /// <param name="segments"> Number of threads to generate. </param>
        /// <param name="colorIndex"> Colour scheme to used .</param>
        public void GeneratePixels(FractalImage image, long iterations, int segments, string colorIndex)
        {
            /// We lock this object
            /// As we cannot let anyone mess with its internals
            /// state until it finished generating pixels.
            lock (this)
            {
                // Set internal states
                this._colorIndex = colorIndex;
                this._maxIterations = iterations;
                this._currentImage = image;

                // Default Segment count calculated based on CPU count.
                if (segments == 0)
                    segments = Environment.ProcessorCount - 1;

                _cancel = false;
                Thread[] _generationThreads = new Thread[segments];

                // Calculate scale factors.
                double xfactor = MenderboltComputation.CalculateXScalar(image);
                double yfactor = MenderboltComputation.CalculateYScalar(image);

                // Calculate number of segments and pixels per image.
                int pixels = image.RawPixels.Length / image.BytesPixel;
                int pixelsPerthread = pixels / segments;

                // Start calculation of pixels in separate threads. 
                for (int i = 0; i < segments; i++)
                {
                    int segmentStart = i * pixelsPerthread;
                    int segmentEnd = segmentStart + pixelsPerthread;
                    _generationThreads[i] = new Thread(() => ProcessSegment(segmentStart, segmentEnd, xfactor, yfactor));
                    _generationThreads[i].Start();
                }

                // Wait for threads to finish doing there job.
                foreach (var thread in _generationThreads)
                    thread.Join();
            }
        }

        /// <summary>
        ///  Abstract function
        ///  That does the actual computation of the fractal
        ///  The implementation of this function will differ according to the type of fractal we
        ///  are generating.
        ///  The function will only generate an image for the given segment of the byte array.
        /// </summary>
        /// <param name="segmentStart"> Start of the segment </param>
        /// <param name="segmentEnd"> End of the segment </param>
        /// <param name="xfactor"> X-Scalar factor of the segment (for zooming) </param>
        /// <param name="yfactor"></param>
        protected abstract void ProcessSegment(int segmentStart, int segmentEnd, double xfactor, double yfactor);
    }
}
