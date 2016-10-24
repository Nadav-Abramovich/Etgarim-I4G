using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
 namespace Etgarim
 {
    public class PosConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            string pos = (string)value;
            return new Geopoint(new BasicGeoposition() { Latitude = double.Parse(pos.Split(';')[0]), Longitude = double.Parse(pos.Split(';')[1]) });
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            string strValue = value as string;
            DateTime resultDateTime;
            if (DateTime.TryParse(strValue, out resultDateTime))
            {
                return resultDateTime;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
