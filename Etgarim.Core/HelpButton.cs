using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etgarim.Core
{
    public class HelpButton
    {
        public int width { set; get; }
        public int height { set; get; }
        public string name { set; get; }
        public string location { set; get; }
        public string text { set; get; }
        public string phone { set; get; }
        public string GetTag { get { return PhoneNameIndanger(); } }
        public bool isivisible { set; get; }
        public string PhoneNameIndanger()
        {
            return phone + ";" + name + ";" + IsInDanger+";"+isivisible;
        }
        public bool IsInDanger { set; get; }
    }
}