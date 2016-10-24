using Cirrious.MvvmCross.WindowsCommon.Views;
using Etgarim.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Etgarim.Views
{

    public sealed partial class StartView : MvxWindowsPage
    {
        public StartView()
        {
            this.InitializeComponent();

        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            base.OnNavigatedFrom(e);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }
        private async void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (!e.Handled && Frame.CurrentSourcePageType.FullName == "Etgarim.Views.StartView")
            {
                e.Handled = true;
                MessageDialog msg = new MessageDialog("האם אתה בטוח שברצונך לצאת?", "יציאה");

                //Commands
                msg.Commands.Add(new UICommand("לא", new UICommandInvokedHandler(CommandHandlers)));
                msg.Commands.Add(new UICommand("כן", new UICommandInvokedHandler(CommandHandlers)));

                await msg.ShowAsync();
            }

        }
        public void CommandHandlers(IUICommand commandLabel)
        {
            var Actions = commandLabel.Label;
            switch (Actions)
            {
                //Okay Button.
                case "לא":
                    break;
                //Quit Button.
                case "כן":
                    Application.Current.Exit();
                    break;
                //end.
            }
        }
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        private StartViewModel ViewModel
        {
            get { return DataContext as StartViewModel; }
        }

        private bool HasViewModel { get { return ViewModel != null; } }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (HasViewModel)
            {
                bool succeeded = false;
                try
                {
                    var file = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("AllowGps.True");

                    ViewModel.NavigateFirst();
                    succeeded = true;
                }
                catch
                {

                }
                if (!succeeded)
                {

                    ViewModel.NavigateLocation();
                }
            }
        }
        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateInfo();
        }
        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateContact();
        }
        private void btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            //App.AppBarButton_Setting_Click();
        }
    }
}
