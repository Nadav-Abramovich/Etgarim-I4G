using Cirrious.MvvmCross.WindowsCommon.Views;
using Etgarim.Core.ViewModels;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Etgarim.Views
{
    public sealed partial class LocationView : MvxWindowsPage
    {
        public LocationView()
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
        private LocationViewModel ViewModel
        {
            get { return DataContext as LocationViewModel; }
        }

        private bool HasViewModel { get { return ViewModel != null; } }

        private async void btn_Save_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (AllowToggle.IsOn && HasViewModel)
            {
                await WriteDataToFileAsync("AllowGps.True", "");
                ViewModel.NavigateFirst();
            }
        }
    }
}
