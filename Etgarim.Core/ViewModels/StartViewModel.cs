using Cirrious.MvvmCross.ViewModels;
using System.ComponentModel;

namespace Etgarim.Core.ViewModels
{
    public class StartViewModel : MvxViewModel, INotifyPropertyChanged
    {

        public void NavigateFirst()
        {
            ShowViewModel<FirstViewModel>();
        }
        public void NavigateLocation()
        {
            ShowViewModel<LocationViewModel>();
        }
        public void NavigateInfo()
        {
            ShowViewModel<InfoViewModel>();
        }
        public void NavigateContact()
        {
            ShowViewModel<ContactViewModel>();
        }

    }
}
