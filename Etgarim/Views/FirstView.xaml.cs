using Cirrious.MvvmCross.WindowsCommon.Views;
using Etgarim.Core.ViewModels;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace Etgarim.Views
{
    public sealed partial class FirstView : MvxWindowsPage
    {
        public FirstView()
        {
            this.InitializeComponent();

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            ViewModel.NavigateStart();
        }
        protected override void OnNavigatedFrom(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            base.OnNavigatedFrom(e);
        }
        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        public async Task WriteDataToFileAsync(string fileName, string content)
        {
            byte[] data = Encoding.Unicode.GetBytes(content);

            var folder = ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (var s = await file.OpenStreamForWriteAsync())
            {
                await s.WriteAsync(data, 0, data.Length);
            }
        }
        private FirstViewModel ViewModel
        {
            get { return DataContext as FirstViewModel; }
        }

        private bool HasViewModel { get { return ViewModel != null; } }

        public async Task<string> ReadFileContentsAsync(string fileName)
        {
            var folder = ApplicationData.Current.LocalFolder;

            try
            {
                var file = await folder.OpenStreamForReadAsync(fileName);

                using (var streamReader = new StreamReader(file))
                {
                    return streamReader.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        private async Task<bool> SaveData()
        {
            if (UserPhone.Text == "Phone Number" || email.Text == "Email" || UserPhone.Text == "מספר טלפון" || email.Text == "אימייל" || CourseID.Text == "קוד שיעור" || CourseID.Text == "Lesson ID")
            {
                if(UserPhone.Text == "Phone Number"||UserPhone.Text == "מספר טלפון")
                {
                    UserPhone.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Red);
                }
                if (email.Text == "Email"||email.Text == "אימייל")
                {
                    email.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Red);
                }
                if (CourseID.Text == "קוד שיעור" || CourseID.Text == "Lesson ID")
                {
                    CourseID.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                if(cb_rememberInstructor.IsChecked==true)
                    await WriteDataToFileAsync("EtgarimData.txt", "UserPhone;" + UserPhone.Text + ",UserEmail;" + email.Text + ",Date;" + UserDate.Date + ",CourseID;" + CourseID.Text+",BoxChecked;True");
                return true;
            }
            return false;
        }
        private async void OmNaOpenMap(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (await SaveData())
            {
                ViewModel.NavigateMap(UserPhone.Text, email.Text, CourseID.Text, UserDate.Date.Day + @"/" + UserDate.Date.Month + @"/" + UserDate.Date.Year);
            }
        }
        private async void ButtonOpen(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (await SaveData())
            {
                ViewModel.NavigateTraveler(UserPhone.Text, email.Text, CourseID.Text, UserDate.Date.Day + @"/" + UserDate.Date.Month + @"/" + UserDate.Date.Year);
            }
        }

        /*private void TextBox_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (TravelerPhone.Text == "מספר טלפון" || TravelerPhone.Text == "Phone Number")
                TravelerPhone.Text = "";
        }*/

        private void UserPhone_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (UserPhone.Text == "מספר טלפון" || UserPhone.Text == "Phone Number")
                UserPhone.Text = "";
            UserPhone.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Black);
        }

        private void UserPhone_LostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (UserPhone.Text == "")
            {
                if (Windows.System.UserProfile.GlobalizationPreferences.Languages[0] == "he-IL")
                    UserPhone.Text = "מספר טלפון";
                else
                    UserPhone.Text = "Phone Number";
            }
        }

        /*private void TravelerPhone_LostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (TravelerPhone.Text == "")
            {
                if (Windows.System.UserProfile.GlobalizationPreferences.Languages[0] == "he-IL")
                    TravelerPhone.Text = "מספר טלפון";
                else
                    TravelerPhone.Text = "Phone Number";
            }
        }*/

        private void email_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (email.Text == "אימייל" || email.Text == "Email")
                email.Text = "";
            email.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Black);
        }

        private void email_LostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (email.Text == "")
            {
                if (Windows.System.UserProfile.GlobalizationPreferences.Languages[0] == "he-IL")
                    email.Text = "אימייל";
                else
                    email.Text = "Email";
            }
        }

        /*private void TravelerEmail_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (TravelerEmail.Text == "אימייל של המדריך" || TravelerEmail.Text == "Email Of Your Mentor")
                TravelerEmail.Text = "";
        }

        private void TravelerEmail_LostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (TravelerEmail.Text == "")
            {
                if (Windows.System.UserProfile.GlobalizationPreferences.Languages[0] == "he-IL")
                    TravelerEmail.Text = "אימייל של המדריך";
                else
                    TravelerEmail.Text = "Email Of Your Mentor";
            }
        }*/

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        private async void Grid_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string Data = await ReadFileContentsAsync("EtgarimData.txt");
            Data = GetString(System.Text.Encoding.UTF8.GetBytes(Data));
            if (Data != string.Empty)
            {
                string[] DataArr = Data.Split(',');
                foreach (string DataPiece in DataArr)
                {
                    string[] TempDataArr = DataPiece.Split(';');
                    if (TempDataArr[0] == "UserPhone")
                    {
                        UserPhone.Text = TempDataArr[1];
                    }
                    else if (TempDataArr[0] == "UserEmail")
                    {
                        email.Text = TempDataArr[1];
                    }
                    else if (TempDataArr[0] == "Date")
                    {
                        UserDate.Date = DateTime.Parse(TempDataArr[1]).Date;
                    }
                    else if (TempDataArr[0] == "CourseID")
                    {
                        CourseID.Text = TempDataArr[1];
                    }
                    else if (TempDataArr[0] == "BoxChecked")
                    {
                        cb_rememberInstructor.IsChecked = true;
                    }
                }
            }
        }

        private void CourseID_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (CourseID.Text == "קוד שיעור" || CourseID.Text == "Lesson ID")
                CourseID.Text = "";
            CourseID.Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Black);
        }

        private void CourseID_LostFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (CourseID.Text == "")
            {
                if (Windows.System.UserProfile.GlobalizationPreferences.Languages[0] == "he-IL")
                    CourseID.Text = "קוד שיעור";
                else
                    CourseID.Text = "Lesson ID";
            }
        }

        private void btn_Setting_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.AppBarButton_Setting_Click();
        }
        bool user = true;
        private void Image_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (user)
            {
                UserPhone.Text = "050-2120779";
                UserDate.Date = new DateTime(2015, 4, 10);
                CourseID.Text = "3028";
                email.Text = "Anan.mazhar@gmail.com";
            }
            else
            {
                UserPhone.Text = "052-8437620";
                UserDate.Date = new DateTime(2015, 4, 10);
                CourseID.Text = "3028";
                email.Text = "Anan.mazhar@gmail.com";
            }
            user = !user;
        }
    }
}
