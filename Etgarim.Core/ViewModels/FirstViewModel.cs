using Cirrious.MvvmCross.ViewModels;

namespace Etgarim.Core.ViewModels
{
    public class FirstViewModel : MvxViewModel
    {
        public string AppName { get { return "������"; } }

        public string Login { get { return "������"; } }

        public string Login2 { get { return "������"; } }

        public string enter { get { return "�����"; } }

        public string LoginPage { get { return "�������"; } }

        public string LoginForCoach { get { return "������� ������"; } }

        public string LoginForStudent { get { return "������� �����"; } }

        public string RememberMe { get { return "���� ����"; } }
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
