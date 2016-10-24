using Cirrious.MvvmCross.ViewModels;
using System.ComponentModel;

namespace Etgarim.Core.ViewModels
{
    public class TravelerViewModel : MvxViewModel , INotifyPropertyChanged
    {
        public string AppName { get { return "אתגרים"; } }

        public string Login { get { return "התחבר"; } }

        public string LoginPage { get { return "התחברות"; } }

        public string LoginForCoach { get { return "התחברות למדריך"; } }

        public string LoginForStudent { get { return "התחברות לחניך"; } }

        public string RememberMe { get { return "זכור אותי"; } }

        public string Info { get; set; }

        public string HelpButtonText { get { return "לחצן מצוקה"; } }

        public string CallText { get { return "התקשר למדריך"; } }

        private string Img;

        public string Image { get { return Img; } set { Img = value; RaisePropertyChanged("Image"); } }

        public void Init(string str)
        {
            Info=str;
        }

        public void NavigateFirst(string IsOk)
        {
            ShowViewModel<FirstViewModel>(new { Con = IsOk });
            Close(this);
        }
        public void NavigateMap()
        {
            ShowViewModel<MapViewModel>();
            Close(this);
        }
    }


}
