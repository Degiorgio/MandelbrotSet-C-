/// Author: Kurt.Degiorgio
///

using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using ImperativeMendalBrotSet.Model;

namespace ImperativeMendalBrotSet.ViewModel
{
    /// <summary>
    /// ViewModel that is bonded to the canvas,
    /// where the Mandelbrot images will be displayed.
    /// </summary>
    public class ImageViewModel : ViewModelBase
    {
        public static  Rect DefaultArea = new Rect(new Point(-2, -1.4), new Point(0.9, 1.4));

        private bool _drawing, _free;
        private ConfigurationViewModel _config;
        private FractalImage _image;
        private MandelbrotImageGenerator _ImageGenerator;

        // Observable collection is a list that allows the UI to be
        // updated automatically When an item is added
        // or removed from the list.
        private ObservableCollection<FractalImage> _previousZooms = new ObservableCollection<FractalImage>();

        #region Properties
   
        public WriteableBitmap ImageSource
        {
            get { return _image.BitMap; }
        }

        public FractalImage Image
        {
            get { return _image;  }
        }

        public ObservableCollection<FractalImage> ZoomList
        {
            get { return _previousZooms; }
        }

        public bool CanCancel
        {
            get { return !Free; }
        }

        public bool Free
        {
            get { return _free; }
            set
            {
                _free = value;
                UpdateProperty("Free");
                UpdateProperty("Status");
                UpdateProperty("CanCancel");
            }
        }

        public string Status
        {
            get
            {
                if (!Free)
                    return "Processing image, please wait.";
                else
                    return "Ready.";
            }
        }

        #endregion

        /// <summary>
        ///  Constructor for this view model
        /// </summary>
        /// <param name="config"> Configurations </param>
        public ImageViewModel(ConfigurationViewModel config)
        {
            Free = true;
            _ImageGenerator = new MandelbrotImageGenerator();
            _config = config;
        }

        #region Operations
        
        /// <summary>
        /// Generates image Mandelbrot image for particular area.
        /// </summary>
        /// <param name="area"> Area that Mandelbrot is bounded to. </param>                 
        public void GenerateImage(Rect area)
        {
            if (!Free)
                return;

            // Create Image.
            _image = _ImageGenerator.CreateImage((int)_config.Height, (int) _config.Width);
            Image.ViewPoint = area;
            _drawing = true;

            Thread drawThread = null;

            // Set up drawing thread.
            (drawThread = new Thread(() => { DrawImage(); })).Start();
            
            // Generate Pixels in a separate thread.
            // We shall not wait for the thread, to keep UI responsive.
            // However, we shall not let the user do any more actions
            // that result in state changes for the image until this
            // thread is finished.
            Free = false;
            (new Thread(() =>
            {
                _ImageGenerator.GeneratePixels(Image, 
                                               _config.MaxIterations,
                                               _config.SegmentIndex,
                                               _config.SelectedColor);

                // Wait for Drawing thread to exit.
                _drawing = false;
                drawThread.Join();

                // Ready, Set busy to false and exit.
                Free = true;
            })).Start();
        }

        /// <summary>
        /// Draws the image onto the canvas.
        /// </summary>
        public void DrawImage()
        {
            while (_drawing)
            {
                int FPS = _config.DrawDelayMS;
                if (FPS <= 0) FPS = 100;
                Thread.Sleep(FPS);
                ImageSource.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Image.WritePixelsToImage();
                    UpdateProperty("ImageSource");
                }));
            }
        }

        /// <summary>
        /// Stops the image generation processes.  
        /// </summary>
        public void StopsProcess()
        {
            _ImageGenerator.Stop();
        }

        /// <summary>
        /// ZoomIn on particular area.
        /// </summary>
        public void ZoomIn()
        {
            if (Free)
            {
                var im = new FractalImage(_image);
                im.ZoomIndex = ZoomList.Count + 1;
                ZoomList.Add(im);
                UpdateProperty("ZoomList");
            }
        }

        /// <summary>
        /// Zoom-out to selected image.
        /// </summary>
        /// <param name="image">image to be zoomed out too.</param>
        public void ZoomOut(FractalImage image)
        {
            if (Free)
            {
                _image = image;

                var hagu = new ObservableCollection<FractalImage>();
                foreach (var item in ZoomList)
                {
                    if (item.ZoomIndex < _image.ZoomIndex)
                    {
                        item.ZoomIndex = hagu.Count + 1;
                        hagu.Add(item);
                    }
                }

                _previousZooms = hagu;

                UpdateProperty("ZoomList");
                UpdateProperty("ImageSource");
            }
        }

        /// <summary>
        /// Clears Zoom list.
        /// </summary>
        public void ClearZoomList()
        {
            if (ZoomList.Count > 0)
            {
                ZoomList.Clear();
                UpdateProperty("ZoomList");
            }
        }
    
        /// <summary>
        ///  Exports current image to file.
        /// </summary>
        /// <param name="file"> Path to file</param>
        public void ExportCurrentImage(string file)
        {
            _image.ExportImage(file);
        }

        #endregion
    }
}
