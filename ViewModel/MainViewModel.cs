/// Author: Kurt.Degiorgio
///

using System.Windows;
using System;
using ImperativeMendalBrotSet.Model;

namespace ImperativeMendalBrotSet.ViewModel
{
    /// <summary>
    ///  Main View-Model attached to the UI.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// View-Model for the Image control.
        public ImageViewModel ImageVM { get; set; }

        /// View-Model for the Configurations.
        public ConfigurationViewModel ConfigsVM { get; set; }

        /// <summary>
        /// Constructor for this VM.
        /// </summary>
        /// <param name="width"> Initial width of image </param>
        /// <param name="height"> Initial height of image </param>
        public MainViewModel(int width, int height)
        {
            ConfigsVM = new ConfigurationViewModel();

            // Defaults
            ConfigsVM.MaxIterations = 1000;
            ConfigsVM.DrawDelayMS = 100;
            ConfigsVM.SegmentIndex = Environment.ProcessorCount;

            ImageVM = new ImageViewModel(ConfigsVM);
            Resize(width, height);
        }

        /// <summary>
        /// Restores  Mandelbrot to original view point.
        /// </summary>
        public void Restore()
        {
            ImageVM.ClearZoomList();
            UpdateImage(ImageViewModel.DefaultArea);
        }

        /// <summary>
        ///  Begins the zoom in process.
        /// </summary>
        public void AskZoom(double RectWidth, double RectHeight, double TopLocation, double LeftLocation)
        {
            double xscale = MenderboltComputation.CalculateXScalar(ImageVM.Image);
            double yscale = MenderboltComputation.CalculateYScalar(ImageVM.Image);

            Point TopLeft = new Point(ImageVM.Image.ViewPoint.Left + LeftLocation * xscale, ImageVM.Image.ViewPoint.Top - TopLocation * yscale);
            Point BottomRight = TopLeft + new Vector(RectWidth * xscale, -RectHeight * yscale);

            ImageVM.ZoomIn();
            UpdateImage(new Rect(TopLeft, BottomRight));
        }

        /// <summary>
        /// Starting the zoom-out process.
        /// </summary>
        /// <param name="index"> Index to image </param>
        public void AskZoomOut(FractalImage image)
        {
            ImageVM.ZoomOut(image);
            UpdateProperty("ImageSource");
        }

        /// <summary>
        /// Resizes image.
        /// </summary>
        /// <param name="width"> Width of new image.</param>
        /// <param name="height"> Height of new image. </param>
        public void Resize(int width, int height)
        {
            ConfigsVM.Width = width;
            ConfigsVM.Height = height;
            UpdateProperty("Configs");
            UpdateImage(ImageViewModel.DefaultArea);
        }

        /// <summary>
        ///  Regenerates the Mandelbrot set
        ///  with the currently selected area.
        /// </summary>
        public void UpdateImage(Rect Area)
        {
            ImageVM.GenerateImage(Area);
            UpdateProperty("ImageSource");
        }
    }
}
