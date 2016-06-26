/// Author: Kurt.Degiorgio
///

namespace ImperativeMendalBrotSet.ViewModel
{
    /// <summary>
    /// Holds current values of the configuration in the UI.
    /// This view model is bounded directly with the UI controls
    /// that host these values.
    /// </summary>
    public class ConfigurationViewModel : ViewModelBase
    {
        // Determines  number of iterations calculated for values of c.
        public long MaxIterations { get; set; }

        // Determines the height of the generated image (Bound to Canvas Height).
        public int Height { get; set; }

        // Determines the width of the generated image (Bound to Canvas Width).
        public int Width { get; set; }

        // Determines colour scheme used to generate image.
        public string SelectedColor { get; set; }

        // Determines number of threads that will be used.
        public int SegmentIndex { get; set; }

        // Determines Fame rate of how quickly the Draw thread updates the image.
        public int DrawDelayMS { get; set; }
    }
}
