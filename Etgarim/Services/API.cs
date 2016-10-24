using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Etgarim.Core.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Windows.Data.Json;
using Newtonsoft.Json;

namespace Etgarim.Services
{
    public class API
    {
        public List<instructor> InstructorList { get; set; }
        public List<Student> StudentList { get; set; }

        public async void InstructorLogIn(string access_key, string user_email, string course_id, string lesson_date)
        {
            string request = string.Format("http://www.etgarim.tandemwise.com/get_lesson_contacts?access_key={0}&user_email={1}&course_id={2}&lesson_date={3}", access_key, user_email, course_id, lesson_date);

            StudentList = new List<Student>();

            InstructorList = new List<instructor>();

            HttpClient http = new HttpClient();
            HttpResponseMessage response = await http.GetAsync(request);
            string webresponse = await response.Content.ReadAsStringAsync();


            //InstructorList = JsonReader.Deserialize<Instructor>(webresponse);
            //StudentList = JsonReader.Deserialize<Instructor>(webresponse);

        }
    }
}
