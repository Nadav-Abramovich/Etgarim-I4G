using Cirrious.MvvmCross.WindowsCommon.Views;
using System.IO;
using System.Net;
using Etgarim.Core.ViewModels;
using System;
using System.Net.NetworkInformation;
using System.Windows;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Core;
using System.Runtime.Serialization.Json;
using Etgarim.Core.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using System.Collections.Generic;

namespace Etgarim.Views
{
    public sealed partial class TravelerView : MvxWindowsPage
    {
        string id = "";

        string courseid = "";
        string email = "";
        string date = "";
        string phone = "";

        bool IsInDanger = false;

        double[] latarr = new double[4];
        double[] longarr = new double[4];

        double latavg;
        double lonavg;

        double lat = 0;
        double lon = 0;
        Geolocator gl = new Geolocator();
        DispatcherTimer t = new DispatcherTimer();
        DispatcherTimer tfi = new DispatcherTimer();
        List<Button> a = new List<Button>();

        private TravelerViewModel ViewModel
        {
            get { return DataContext as TravelerViewModel; }
        }

        private bool HasViewModel { get { return ViewModel != null; } }
        private bool IsDataContextReady = false;
        private bool isready = false;
        public TravelerView()
        {
            this.InitializeComponent();

            tfi.Interval = new System.TimeSpan(0, 0, 1);
            tfi.Tick += tfi_Tick;

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            DataContextChanged += TravelerView_DataContextChanged;

            gl.MovementThreshold = 2;

            gl.DesiredAccuracyInMeters = 10;

            gl.DesiredAccuracy = PositionAccuracy.Default;

            gl.ReportInterval = 0;

            t.Tick += t_Tick;
            t.Interval = new TimeSpan(0, 0, 9);
            t.Start();
        }

        void tfi_Tick(object sender, object e)
        {
            TopButton.Visibility = Visibility.Visible;
            TopButton.IsTapEnabled = false;
            ButtonBitul.IsEnabled = true;
            tfi.Stop();
        }

        void TravelerView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            ViewModel.Image = "/Assets/button/Button2.png";

            gl.StatusChanged += gl_StatusChanged;
            if (gl.LocationStatus != PositionStatus.Initializing)
            {

            }
        }

        void t_Tick(object sender, object e)
        {
            if (isready)
                gl_PositionChanged(gl, null);
        }

        void gl_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            /*if (gl.LocationStatus != PositionStatus.Ready)
            {
                //ViewModel.NavigateFirst("");
                this.Loaded += (sende2r, e) => ViewModel.NavigateFirst("Error, Make sure the gps service is working");
                DataContextChanged -= TravelerView_DataContextChanged;
            }
            else
            {*/
            //gl.MovementThreshold = 1;

            if (gl.LocationStatus == PositionStatus.Ready)
            {
                isready = true;
                test();
            }
            else if (gl.LocationStatus == PositionStatus.Disabled)
            {
                nottest();
            }
            //}
        }
        private void test()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ErrorRect.Visibility = Visibility.Collapsed; ErrorMsg.Visibility = Visibility.Collapsed; ErrorMsg.Text = ""; });
        }
        private void nottest()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!isready)
                {
                    ViewModel.NavigateFirst("No GPS");
                }
                ErrorMsg.Visibility = Visibility.Visible;
                ErrorMsg.Text = "No GPS";
                ErrorRect.Visibility = Visibility.Visible;
            });
        }
        async void GetInitialPosition()
        {

            //IAsyncOperation<Geoposition> locationTask = null;

            try
            {
                Geoposition gp = await gl.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                //LatitudeTextBlock.Text = geoposition.Coordinate.Latitude.ToString("0.00");
                //LongitudeTextBlock.Text = geoposition.Coordinate.Longitude.ToString("0.00");
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    //StatusTextBlock.Text = "location  is disabled in phone settings.";
                }
                //else
                {
                    // something else happened acquring the location
                }
            }
            finally
            {
                //OmNaMap.Center = new Geopoint(new BasicGeoposition() { Latitude = 47.620, Longitude = -122.349 });
            }
        }

        async void gl_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            DataContractJsonSerializer DataJson = new DataContractJsonSerializer(typeof(JsonReciever));
            //Windows.Storage.StorageFile
            //http://etgarim.tandemwise.com/get_lesson_contacts?access_key=fkjdfskjh23984273skjsdah84&user_email=anan.mazhar@gmail.com&course_id=3028&lesson_date=10/04/2015

            //phone = ViewModel.Info.Split(';')[0];
            //email = ViewModel.Info.Split(';')[1];
            //courseid = ViewModel.Info.Split(';')[2];
            //date = ViewModel.Info.Split(';')[3];

            IAsyncOperation<Geoposition> locationTask = null;

            try
            {
                locationTask = gl.GetGeopositionAsync(TimeSpan.FromMinutes(0), TimeSpan.FromSeconds(7));
                Geoposition gp = await locationTask;
                Geocoordinate gc = gp.Coordinate;

                for (int i = 0; i < latarr.Length - 1; i++)
                {
                    latarr[i + 1] = latarr[i];
                }
                for (int i = 0; i < longarr.Length - 1; i++)
                {
                    longarr[i + 1] = longarr[i];
                }
                latarr[0] = gc.Point.Position.Latitude;
                longarr[0] = gc.Point.Position.Longitude;

                latavg = (latarr[0] + latarr[1] + latarr[2] + latarr[3]) / (4 - howmanytimes(latarr, 0));
                lonavg = (longarr[0] + longarr[1] + longarr[2] + longarr[3]) / (4 - howmanytimes(longarr, 0));

                string s = string.Format("http://etgarim.tandemwise.com/set_contact_pos?access_key=fkjdfskjh23984273skjsdah84&user_email=" + email + "&contact_id=" + id + "&contact_pos=" + latavg + ";" + lonavg + ";" + IsInDanger);
                WebRequest request = HttpWebRequest.Create(s);
                request.ContentType = "application/json";
                request.Method = "POST";
                await request.GetResponseAsync();
                ErrorMsg.Visibility = Visibility.Collapsed;
                ErrorRect.Visibility = Visibility.Collapsed;
            }
            catch
            {
                if (ErrorMsg.Visibility == Visibility.Collapsed)
                {
                    ErrorMsg.Visibility = Visibility.Visible;
                    ErrorMsg.Text = "No Internet";
                    ErrorRect.Visibility = Visibility.Visible;
                }
            }
            finally
            {
                if (locationTask != null)
                {
                    if (locationTask.Status == AsyncStatus.Started)
                        locationTask.Cancel();

                    locationTask.Close();
                }
            }
        }
        private int howmanytimes(double[] d, int num)
        {
            int mone = 0;
            for (int i = 0; i < d.Length; i++)
            {
                if (d[i] == num)
                {
                    mone++;
                }
            }
            return mone;
        }
        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;

            t.Stop();
            tfi.Stop();
            ViewModel.NavigateFirst("");
        }

        private void OmNaOpenMap(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //btn_member.
            //groman tivdok ze amoor laavod achshav :
            ViewModel.NavigateMap();
            //Frame
        }
        private void ButtonOpen(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.NavigateFirst("");
        }
        protected async override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContractJsonSerializer DataJson = new DataContractJsonSerializer(typeof(JsonReciever));
            //Windows.Storage.StorageFile
            //http://etgarim.tandemwise.com/get_lesson_contacts?access_key=fkjdfskjh23984273skjsdah84&user_email=anan.mazhar@gmail.com&course_id=3028&lesson_date=10/04/2015

            phone = ViewModel.Info.Split(';')[0];
            email = ViewModel.Info.Split(';')[1];
            courseid = ViewModel.Info.Split(';')[2];
            date = ViewModel.Info.Split(';')[3];

            WebRequest request = HttpWebRequest.Create(string.Format(@"http://etgarim.tandemwise.com/get_lesson_contacts?access_key=fkjdfskjh23984273skjsdah84&user_email=" + email + "&course_id=" + courseid + "&lesson_date=" + date));
            request.ContentType = "application/json";
            request.Method = "POST";
            try
            {
                using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //Console.Out.WriteLine("Error fetching data. Server returned status code: {0}", response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            var content = reader.ReadToEnd();
                            if (string.IsNullOrWhiteSpace(content))
                            {
                                //Console.Out.WriteLine("Response contained empty body...");

                            }
                            else
                            {
                                //Console.Out.WriteLine("Response Body: \r\n {0}", content);

                                JsonReciever jsr;
                                jsr = JsonConvert.DeserializeObject<JsonReciever>(content);

                                foreach (Student student in jsr.Students)
                                {
                                    Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                                    student.student_phone = Regex.Replace(student.student_phone, "[^.0-9]", "");
                                    phone = Regex.Replace(phone, "[^.0-9]", "");
                                    if (student.student_phone == phone)
                                    {
                                        id = student.student_id;
                                        break;
                                    }
                                }
                                if (string.IsNullOrEmpty(id))
                                {
                                    ViewModel.NavigateFirst("Error; Please make sure the information you provided is correct and isnt of a guide");
                                }
                                foreach (instructor inst in jsr.instroctors)
                                {
                                    Button b=new Button();
                                    b.Content = inst.instructor_name;
                                    b.Click += (sender, args) => { Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(inst.instructor_phone, inst.instructor_name); };
                                    a.Add(b);
                                }
                                view.ItemsSource = a;
                            }

                            //Assert.NotNull(content);
                        }
                    }
                }
                gl_PositionChanged(gl, null);
            }
            catch (Exception ex)
            {
                if (ex.Message == "The remote server returned an error: NotFound.")
                {
                    ViewModel.NavigateFirst("Internet Problem");
                }
                else
                {
                    ViewModel.NavigateFirst("Error; Please make sure the information you provided is correct");
                }
            }
        }
        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //ViewModel.NavigateFirst();
        }

        private void Button_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
        }

        private void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            tfi.Start();
            TopButton.Visibility = Visibility.Collapsed;

            ButtonBitul.Visibility = Visibility.Visible;
            ButtonBitul.IsEnabled = true;
            TextBitul.Visibility = Visibility.Visible;

            ButtonBitul.IsEnabled = false;

            IsInDanger = true;
        }

        private void BottomButton_ManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {

        }

        private void ButtonBitul_Click(object sender, RoutedEventArgs e)
        {
            ButtonBitul.Visibility = Visibility.Collapsed;
            ButtonBitul.IsEnabled = false;
            TextBitul.Visibility = Visibility.Collapsed;
            TopButton.IsTapEnabled = true;
            IsInDanger = false;
        }
    }
}
