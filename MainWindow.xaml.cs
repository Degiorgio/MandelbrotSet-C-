/// Author: Kurt.Degiorgio
///

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using Microsoft.Win32;
using ImperativeMendalBrotSet.Model;
using ImperativeMendalBrotSet.ViewModel;

namespace ImperativeMendalBrotSet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IList<string> _Colors = new List<string>();
        private Point _Position;
        private MainViewModel _ViewModel = null;
        private bool _PanelOpen = false;
        private bool _LeftClickHold = false;

        public MainWindow()
        {
            // Supported Color Schemes.
            _Colors.Add("Continious");
            _Colors.Add("Gold");
            _Colors.Add("Black");
            _Colors.Add("Smooth Red");
            _Colors.Add("Wavey Blue");

            // Init UI Components.
            InitializeComponent();

            // We have to execute some stuff only when
            // the UI components have been loaded.
            Loaded += delegate
            {
                int height = (int)Application.Current.MainWindow.ActualHeight;
                CanvasBoard.Width = height;

                _ViewModel = new MainViewModel(height, height);

                // Set Data context, this will determine what part of the UI,
                // is binding to a particular View Model.
                this.DataContext = _ViewModel;
                this.StatusValue.DataContext = _ViewModel.ImageVM;
                this.ImageBoard.DataContext = _ViewModel.ImageVM;
                this.ZoomListBox.DataContext = _ViewModel.ImageVM;
                IterationTextBox.Text = _ViewModel.ConfigsVM.MaxIterations.ToString();

                // Set Colour Scheme to UI.
                this.ColorScheme.ItemsSource = _Colors;
                this.ColorScheme.SelectedIndex = 0;
            };
        }

        /// <summary>
        /// Opens or Closes the Zoom-out panel.
        /// </summary>
        private void OpenOrClosePanel()
        {
            if (!_PanelOpen)
            {
                TriggerPannelOpen.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                _PanelOpen = true;
            }
            else
            {
                TriggerPannelClose.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                _PanelOpen = false;
            }
        }

        #region Events

        // Stop Button clicked.
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.ImageVM.StopsProcess();
        }


        // User Clicked Zoom out button.
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            OpenOrClosePanel();
        }

        // Reset Button clicked.
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.Restore();
        }

        // User Clicked Export button.
        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog(); // Save Dialog
            dialog.OverwritePrompt = true;
            dialog.Title = "Export Mandelbrot image";
            dialog.DefaultExt = ".png";
            dialog.Filter = "Image Files(*.png)|*.png;|All files (*.*)|*.*Image (*.png)";
            if (dialog.ShowDialog() == true)
                _ViewModel.ImageVM.ExportCurrentImage(dialog.FileName);
        }

        // User Wants to Zoom out.
        private void ZoomListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var addedItems = e.AddedItems;
            if (addedItems.Count > 0)
            {
                var selectedItem = addedItems[0];

                if (_PanelOpen) OpenOrClosePanel();

                _ViewModel.AskZoomOut(selectedItem as FractalImage);
            }
            System.Threading.Thread.Sleep(100);  // We need to the sleep to prevent weird scenarios related to incorrect selections.
        }

        // User wants to change colour scheme.
        private void ColorScheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ViewModel.ConfigsVM.SelectedColor = ColorScheme.SelectedItem as string;

            if (_PanelOpen) OpenOrClosePanel();

            _ViewModel.UpdateImage(_ViewModel.ImageVM.Image.ViewPoint);
            System.Threading.Thread.Sleep(100);  // We need to the sleep to prevent weird scenarios related to incorrect selections.
        }

        // Iteration Box Lost Focus, Update Iteration value.
        private void IterationTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            long number = 0;
            try
            {
                number = long.Parse(IterationTextBox.Text);
            }
            catch (Exception)
            {
                number = 0; // Only update if number is valid.
            }

            if (number > 0 && number != _ViewModel.ConfigsVM.MaxIterations)
            {
                _ViewModel.ConfigsVM.MaxIterations = number;

                if (_PanelOpen) OpenOrClosePanel();

                _ViewModel.UpdateImage(_ViewModel.ImageVM.Image.ViewPoint);
            }
        }

        // Size of window changed, updated image.
        private void Window_SizeChanged(object sender, RoutedEventArgs e)
        {
            if (_ViewModel == null)
                return;

            int height = (int)Application.Current.MainWindow.ActualHeight;
            CanvasBoard.Width = height;

            // When resizing we have to keep the aspect ratio of the image intact, if we don't
            // We mess up the image and the zoom functionality.
            // TODO: scale aspect ratio proportionally. 
            _ViewModel.Resize((int)height, height);
        }

        // User clicked on the canvas we must draw the selection 
        // rectangle at the point he clicked.
        private void Canvas_Clicked(object sender, MouseButtonEventArgs e)
        {
            _LeftClickHold = true;
            _Position = e.GetPosition(CanvasBoard);

            // Position the rectangle on the canvas according to 
            // Where the use clicked.        
            Canvas.SetLeft(SelectionRectangle, _Position.X);
            Canvas.SetTop(SelectionRectangle, _Position.Y);
        }

        // User lifted mouse button, we must zoom-in. 
        private void Canvas_MouseReleased(object sender, MouseButtonEventArgs e)
        {
            _LeftClickHold = false;

            // Checking that the rectangle is not super small, this
            // means that the user just clicked and did not drag. 
            if (SelectionRectangle.Width > 2 && SelectionRectangle.Height > 2)
            {
                if (_PanelOpen) OpenOrClosePanel();

                _ViewModel.AskZoom(SelectionRectangle.Width, SelectionRectangle.Height,
                                   Canvas.GetTop(SelectionRectangle), Canvas.GetLeft(SelectionRectangle));
            }

            // Set Rectangle dimensions to zero so that it is not visible anymore.
            SelectionRectangle.Width = SelectionRectangle.Height = 0;
        }

        // Mouse moved on the canvas board.
        private void CanvasBoard_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_LeftClickHold)
            {
                return;
            }
            else
            {
                CalculateDimensions(e.GetPosition(CanvasBoard));
            }
        }

        /// <summary>
        /// Calculate Rectangle position
        /// </summary>
        /// <param name="currentMP"> Mouse position </param>
        private void CalculateDimensions(Point currentMP)
        {
            var changexy = currentMP - _Position;

            // Rectangle is reversed, disallow.
            if (changexy.Y <= 0) return;
             SelectionRectangle.Width = SelectionRectangle.Height = changexy.Y;
            Canvas.SetLeft(SelectionRectangle, _Position.X);
            Canvas.SetTop(SelectionRectangle, _Position.Y);
        }

        #endregion


    }
}