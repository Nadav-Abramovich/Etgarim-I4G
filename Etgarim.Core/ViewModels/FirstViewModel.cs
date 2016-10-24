using Cirrious.MvvmCross.ViewModels;

namespace Etgarim.Core.ViewModels
{
    public class FirstViewModel : MvxViewModel
    {
        public string AppName { get { return "אתגרים"; } }

        public string Login { get { return "למדריך"; } }

        public string Login2 { get { return "למשתמש"; } }

        public string enter { get { return "כניסה"; } }

        public string LoginPage { get { return "התחברות"; } }

        public string LoginForCoach { get { return "התחברות למדריך"; } }

        public string LoginForStudent { get { return "התחברות לחניך"; } }

        public string RememberMe { get { return "זכור אותי"; } }
        private string _isok;
        public string IsOk { get { return _isok; } set { _isok = value; } }

        public void Init(string Con)
        {
            IsOk = Con;
        }

        public void NavigateTraveler(string phone, string email, string id, string date)
        
        {
            ShowViewModel<TravelerViewModel>(new { str = phone + ";" + email + ";" + id + ";" + date });
        }
        public void NavigateMap(string phone, string email, string id, string date)
        {
            ShowViewModel<MapViewModel>(new { str = phone + ";" + email + ";" + id + ";" + date });
        }
        public void NavigateStart()
        {
            ShowViewModel<StartViewModel>();
        }

    }


}
