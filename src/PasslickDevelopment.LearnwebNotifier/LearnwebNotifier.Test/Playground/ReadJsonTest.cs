using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace LearnwebNotifier.Test.Playground
{
    class ReadJsonTest
    {

        static void Main(string[] args)
        {
            List<string> courses = new List<string>();

            courses.Add("43023"); // CACS-2020_1
            courses.Add("42869"); // CSOS-2020
            courses.Add("43472"); // Da-2020_1
            courses.Add("43562"); // PM-2020_1
            courses.Add("43547"); // Si-2020_1
            courses.Add("27915"); // WI-Koord

            string jsonString = JsonSerializer.Serialize(courses);
            Console.WriteLine(jsonString);
            File.WriteAllText("courses.json", jsonString);


            string unmarshalString;
            List<string> uCourses;

            unmarshalString = File.ReadAllText("courses.json");
            uCourses = JsonSerializer.Deserialize<List<string>>(unmarshalString);
            Console.WriteLine(unmarshalString);
            foreach (string s in uCourses)
            {
                Console.WriteLine(s);
            }

        }

    }
}
