using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using Etgarim.Core.ViewModels;
using Cirrious.MvvmCross.WindowsCommon.Views;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Net;
using Etgarim.Core.Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Core;
using Windows.Graphics.Display;
using Etgarim.Core;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.UI;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Etgarim.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapView : MvxWindowsPage
    {

        private bool ready = false;

        double lat = 0;
        double lon = 0;
        Geolocator gl = new Geolocator();
        MapIcon Micon = new MapIcon();
        Geopoint blo;

        int mone = 0;

        //MapControl OmNaMap = new MapControl();

        string id = "";

        Student[] arrstudent;

        string courseid = "";
        string email = "";
        string date = "";
        string phone = "";
        bool state = false;


        RandomAccessStreamReference RedCircleImage = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/RedCircle.png"));
        RandomAccessStreamReference GreenCircleImage = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/GreenCircle.png"));
        BitmapImage Empty = new BitmapImage(new Uri("ms-appx:///Assets/button/Empty.png"));
        BitmapImage Green = new BitmapImage(new Uri("ms-appx:///Assets/GreenCircle.png"));
        BitmapImage Red = new BitmapImage(new Uri("ms-appx:///Assets/RedCircle.png"));
        BitmapImage OkPeople = new BitmapImage(new Uri("ms-appx:///Assets/MapButtons/People.png"));
        BitmapImage DangerPeople1 = new BitmapImage(new Uri("ms-appx:///Assets/MapButtons/p1.png"));
        BitmapImage DangerPeople2 = new BitmapImage(new Uri("ms-appx:///Assets/MapButtons/p2.png"));
        BitmapImage Globus = new BitmapImage(new Uri("ms-appx:///Assets/MapButtons/Globus.png"));
        BitmapImage World = new BitmapImage(new Uri("ms-appx:///Assets/MapButtons/World.png"));

        public MapView()
        {
            this.InitializeComponent();

            //this.Loaded += (sender, e) => 

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            gl.DesiredAccuracyInMeters = 500;

            gl.DesiredAccuracy = PositionAccuracy.Default;
            //this.Loaded += (sender, e) => OmNaMap.Visibility = Visibility.Collapsed;

            //MapGrid.Children.Add(OmNaMap);

            gl.MovementThreshold = 2;
            DataContextChanged += TravelerView_DataContextChanged;
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = new System.TimeSpan(0, 0, 5);
            t.Tick += t_Tick;
            t.Start();
            DispatcherTimer t2 = new DispatcherTimer();
            t2.Interval = new System.TimeSpan(0, 0, 1);
            t2.Tick += t2_Tick;
            t2.Start();

            //gl.MovementThreshold = 1;
            //gl.PositionChanged += gl_PositionChanged;
        }

        void t2_Tick(object sender, object e)
        {
            if (mone == 0||ListRect.Visibility==Visibility.Visible)
            {
                //DangerButton.Content = mone + " Person In Danger";
                DangerButton.Source = OkPeople;
            }
            else
            {
                if (DangerButton.Source != DangerPeople1)
                {
                    DangerButton.Source = DangerPeople1;
                }
                else
                {
                    DangerButton.Source = DangerPeople2;
                }
           }
        }
        void TravelerView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            gl.StatusChanged += gl_StatusChanged;
        }
        private bool closing = false;
        void gl_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            if (gl.LocationStatus == PositionStatus.Ready)
            {
                if (!ready)
                {
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        GetInitialContact();

                        MoveToPosition();
                    });
                }
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ErrorRect.Visibility = Visibility.Collapsed; ErrorMsg.Visibility = Visibility.Collapsed; ErrorMsg.Text = ""; });
            }
            else if (gl.LocationStatus == PositionStatus.Disabled)
            {
                if (ready)
                {
                    //Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { ErrorRect.Visibility = Visibility.Visible; ErrorMsg.Visibility = Visibility.Visible; ErrorMsg.Text = "GPS erorr"; });
                }
                if (!closing)
                {
                    gl.StatusChanged -= gl_StatusChanged;

                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => this.Loaded += (sender2, e) => ViewModel.NavigateFirst("GPS or Internet Problem"));
                    closing = true;
                }
            }
        }

        void t_Tick(object sender, object e)
        {
            if (ready)
                UpdateMap();
        }

        private async void GetInitialContact()
        {
            DataContractJsonSerializer DataJson = new DataContractJsonSerializer(typeof(JsonReciever));
            //Windows.Storage.StorageFile
            //http://etgarim.tandemwise.com/get_lesson_contacts?access_key=fkjdfskjh23984273skjsdah84&user_email=anan.mazhar@gmail.com&course_id=3028&lesson_date=10/04/2015

            phone = ViewModel.Info.Split(';')[0];
            email = ViewModel.Info.Split(';')[1];
            courseid = ViewModel.Info.Split(';')[2];
            date = ViewModel.Info.Split(';')[3];

            bool isinstructor = false;
            try
            {
                WebRequest request = HttpWebRequest.Create(string.Format(@"http://etgarim.tandemwise.com/get_lesson_contacts?access_key=fkjdfskjh23984273skjsdah84&user_email=" + email + "&course_id=" + courseid + "&lesson_date=" + date));
                request.ContentType = "application/json";
                request.Method = "POST";
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
                                Regex rgx = new Regex("[^a-zA-Z0-9 -]");
                                phone = Regex.Replace(phone, "[^.0-9]", "");
                                foreach (instructor inst in jsr.instroctors)
                                {
                                    inst.instructor_phone = Regex.Replace(inst.instructor_phone, "[^.0-9]", "");
                                    if (inst.instructor_phone == phone)
                                    {
                                        isinstructor = true;
                                    }
                                }
                                if (isinstructor)
                                {
                                    arrstudent = jsr.Students;
                                    UpdateMap();
                                }
                                else
                                {
                                    ViewModel.NavigateFirst("Incorrect Phone Number");
                                }
                            }

                            //Assert.NotNull(content);
                        }
                    }
                }
                if (isinstructor)
                    ready = true;
            }
            catch (Exception ex)
            {
                if (!closing && ex.Message == "The remote server returned an error: NotFound.")
                {
                    closing = true;
                    Dispatcher.RunAsync(CoreDispatcherPriority.High, () => ViewModel.NavigateFirst("Internet Problem"));
                }
                else if (!closing)
                {
                    closing = true;
                    Dispatcher.RunAsync(CoreDispatcherPriority.High, () => ViewModel.NavigateFirst("Error; make sure the data" + Environment.NewLine + " you provided is correct"));
                }
            }
        }

        ObservableCollection<Button> a = new ObservableCollection<Button>();
        private void AddtoDangerList(double latitiude, double longtitude, string name)
        {
            Button tmp = new Button();
            tmp.Content = name;
            tmp.Background = new SolidColorBrush(Colors.IndianRed);

            tmp.Click += (sender, args) => { OmNaMap.TrySetViewAsync(new Geopoint(new BasicGeoposition() { Latitude = latitiude, Longitude = longtitude })); InNeedList.Visibility = Visibility.Collapsed; ListRect.Visibility = Visibility.Collapsed; HelpText.Visibility = Visibility.Collapsed; };
            a.Add(tmp);
        }


        private async void UpdateMap()
        {
            ready = false;
            ICollection<MapElement> Copy = new List<MapElement>();
            //int counter = 0;
            try
            {

                List<HelpButton> l = new List<HelpButton>();
                foreach (Student s in arrstudent)
                {
                    if (s.student_id != "")
                    {
                        string ss = string.Format("http://etgarim.tandemwise.com/get_contact_pos?access_key=fkjdfskjh23984273skjsdah84&user_email=" + email + "&contact_id=" + s.student_id);
                        WebRequest request = HttpWebRequest.Create(ss);
                        request.ContentType = "application/json";
                        request.Method = "POST";
                        using (HttpWebResponse response2 = await request.GetResponseAsync() as HttpWebResponse)
                        {
                            using (StreamReader reader2 = new StreamReader(response2.GetResponseStream()))
                            {
                                Student Temp = JsonConvert.DeserializeObject<Student>(reader2.ReadToEnd());
                                if (Temp.contact_pos != null && Temp.contact_pos.Contains(';'))
                                {
                                    Micon = new MapIcon();
                                    Micon.Location = new Geopoint(new BasicGeoposition() { Latitude = double.Parse(Temp.contact_pos.Split(';')[0]), Longitude = double.Parse(Temp.contact_pos.Split(';')[1]) });
                                    Micon.NormalizedAnchorPoint = new Point(0, 0);
                                    Micon.Title = s.student_name;
                                    Micon.ZIndex = 2;
                                    HelpButton b = new HelpButton() { location = Temp.contact_pos, text = "TEXT", width = 18, height = 18 };

                                    b.name = s.student_name;
                                    b.phone = s.student_phone;
                                    b.IsInDanger = Temp.contact_pos.Contains("True");
                                    b.isivisible = Micon.Visible;


                                    if (Temp.contact_pos.Contains("True"))
                                    {
                                        Micon.Image = RedCircleImage;
                                        b.text = Red.UriSource.AbsolutePath;
                                    }
                                    else
                                    {
                                        Micon.Image = GreenCircleImage;
                                        b.text = Green.UriSource.AbsolutePath;
                                    }
                                    l.Add(b);
                                    Copy.Add(Micon);
                                }
                            }
                        }
                    }
                }

                if (OmNaMap.MapElements.Count == 0)
                {
                    foreach (MapIcon me in Copy)
                    {
                        OmNaMap.MapElements.Add(me);
                    }
                }
                else
                {
                    foreach (MapIcon me in Copy)
                    {
                        bool Added = false;
                        foreach (MapIcon me2 in OmNaMap.MapElements)
                        {
                            if (me2.Title == me.Title && me.Title == "לילי גולדווין")
                            {
                                Added = true;

                                me2.Location = me.Location;
                                me2.Image = me.Image;
                            }
                            else if (me2.Title == me.Title)
                            {
                                Added = true;

                                me2.Location = me.Location;
                                me2.Image = me.Image;
                            }
                        }
                        if (!Added)
                        {
                            OmNaMap.MapElements.Add(me);
                        }
                    }
                }
                mone = 0;

                a.Clear();
                foreach (MapIcon me2 in OmNaMap.MapElements)
                {
                    if (me2.Image == RedCircleImage)
                    {
                        bool added = false;
                        foreach (Button b in a)
                        {
                            if (b.Content.ToString() == me2.Title)
                                added = true;
                        }
                        if (!added)
                        {
                            AddtoDangerList(me2.Location.Position.Latitude, me2.Location.Position.Longitude, me2.Title);
                            mone++;
                        }
                    }
                }
                ErrorMsg.Visibility = Visibility.Collapsed;
                ErrorRect.Visibility = Visibility.Collapsed;
                //InNeedList.ItemsSource = null;

                InNeedList.ItemsSource = a;
                // InNeedList.UpdateLayout();
                ViewModel.mapitems = l;

            }
            catch (Exception ex)
            {
                if (ex.Message == "The remote server returned an error: NotFound.")
                {
                    ErrorMsg.Visibility = Visibility.Visible;
                    ErrorRect.Visibility = Visibility.Visible;
                    ErrorMsg.Text = "Internet Error";
                }
            }
            ready = true;
        }

        private MapViewModel ViewModel
        {
            get { return DataContext as MapViewModel; }
        }

        private bool HasViewModel { get { return ViewModel != null; } }
        void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true;
            if (ListRect.Visibility == Visibility.Collapsed)
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                ViewModel.NavigateFirst();
            }
            else
            {
                DangerButton_Tapped(sender, null);
            }
        }

        async void gl_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Geoposition gp = await gl.GetGeopositionAsync();
            lat = gp.Coordinate.Latitude;
            lon = gp.Coordinate.Longitude;
            blo = gp.Coordinate.Point;
        }
        async void MoveToPosition()
        {
            while (OmNaMap.LoadingStatus.ToString() != "Loaded")
            {
                await Task.Delay(100);
            }

            IAsyncOperation<Geoposition> locationTask = null;

            try
            {
                Geoposition gp = await gl.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                await OmNaMap.TrySetViewAsync(new Geopoint(new BasicGeoposition() { Latitude = gp.Coordinate.Latitude, Longitude = gp.Coordinate.Longitude }), 15);
                Loading.Visibility = Visibility.Collapsed;
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
                /*Micon.Location = new Geopoint(new BasicGeoposition() { Latitude = 47.620, Longitude = -122.349 });
                Micon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                Micon.Title = "Test";
                OmNaMap.MapElements.Add(Micon);*/
            }

            //ProgBar.IsEnabled = false;
            //ProgBar.Visibility = Visibility.Collapsed;

            //await OmNaMap.TrySetViewAsync(OmNaMap.Center, 15);


            //OmNaMap.ZoomLevel = 15;

            /*try
            {
                locationTask = gl.GetGeopositionAsync(TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(10));
                while (gl.LocationStatus == PositionStatus.Initializing)
                {
                    await Task.Delay(100);
                }
                Geoposition position = await gl.GetGeopositionAsync();
                lat = position.Coordinate.Latitude;
                lon = position.Coordinate.Longitude;
                //OmNaMap.Center = new Geopoint(new BasicGeoposition() { Latitude = lat, Longitude = lon });
                //Micon.Location = new Geopoint(new BasicGeoposition() { Latitude = lat, Longitude = lon });
            }*/
        }

        private void DangerButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (InNeedList.Visibility == Visibility.Visible)
            {
                InNeedList.Visibility = Visibility.Collapsed;
                ListRect.Visibility = Visibility.Collapsed;
                HelpText.Visibility = Visibility.Collapsed;
            }
            else
            {
                InNeedList.Visibility = Visibility.Visible;
                ListRect.Visibility = Visibility.Visible;
                HelpText.Visibility = Visibility.Visible;
            }
        }


        private void PositionButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MoveToPosition();
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image hb = (Image)sender;
            string name = hb.Tag.ToString().Split(';')[1];

            if (OmNaMap.ZoomLevel >= 13)
            {
                string phone = hb.Tag.ToString().Split(';')[0];
                bool isindanger = bool.Parse(hb.Tag.ToString().Split(';')[2]);
                if (isindanger)
                    Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI(phone, name);
            }
        }

        private void ChangeMap_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (OmNaMap.Style != MapStyle.AerialWithRoads)
            {
                ChangeMap.Source = Globus;
                OmNaMap.Style = MapStyle.AerialWithRoads;
            }
            else
            {
                ChangeMap.Source = World;
                OmNaMap.Style = MapStyle.Road;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
    }
}