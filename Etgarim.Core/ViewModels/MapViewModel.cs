using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using System.ComponentModel;

namespace Etgarim.Core.ViewModels
{
    public class MapViewModel : MvxViewModel, INotifyPropertyChanged
    {
        List<HelpButton> _mapitems;
        public List<HelpButton> mapitems { set { _mapitems = value; RaisePropertyChanged("mapitems"); } get { return _mapitems; } }
        public string Info { set; get; }
        int x = 1;
        public int ratio { set { x = value; RaisePropertyChanged("ratio"); } get { return x; } }
        public void NavigateTraveler()
        {
            ShowViewModel<TravelerViewModel>();
            Close(this);
        }
        public void test()
        {
            Close(this);
        }

        public void Init(string str)
        {
            Info = str;
        }
        public void NavigateFirst()
        {
            ShowViewModel<FirstViewModel>();
            Close(this);
        }
        public void NavigateFirst(string IsOk)
        {
            ShowViewModel<FirstViewModel>(new { Con = IsOk });
            //Close(this);
        }
        public string helptext { get { return "אנשים במצוקה"; } set { helptext = value; } }
    }
}
