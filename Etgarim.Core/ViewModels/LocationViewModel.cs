using Cirrious.MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etgarim.Core.ViewModels
{
    public class LocationViewModel : MvxViewModel, INotifyPropertyChanged
    {
        public void NavigateFirst()
        {
            ShowViewModel<FirstViewModel>();
        }

        public void NavigateStart()
        {
            ShowViewModel<StartViewModel>();
        }
    }
}
