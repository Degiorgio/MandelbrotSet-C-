/// Author: Kurt.Degiorgio
/// 

using System.ComponentModel;

namespace ImperativeMendalBrotSet.ViewModel
{
    /// <summary>
    ///  ViewModelBase: Abstract parent class used by ViewModels to update UI.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Updates bounded Property.
        /// </summary>
        /// <param name="str"> Name of property to be updated </param>
        protected void UpdateProperty(string str)
        {
            PropertyChangedEventHandler even = PropertyChanged;
            if (even == null)
                return;
            even(this, new PropertyChangedEventArgs(str));
        }
    }
}

