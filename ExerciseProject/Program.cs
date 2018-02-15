using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ExerciseProject.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExerciseProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var peoplePet = new PeoplePet();
            peoplePet.RetrievePetNames();
            Console.ReadLine();
        }

    }

    public class PeoplePet
    {
        public void RetrievePetNames()
        {
            var peopleList = FetchPeopleData().GetAwaiter().GetResult();

            var catMList = peopleList.Where(person => person.Gender.ToLower().Equals("male") && person.Pets != null)
                                     .Select(person => person.Pets.Where(pet => pet.Type.ToLower().Equals("cat"))
                                     .Select(pet => pet.Name)).SelectMany(k => k).ToList();

            var catFList = peopleList.Where(person => person.Gender.ToLower().Equals("female") && person.Pets != null)
                                    .Select(person => person.Pets.Where(pet => pet.Type.ToLower().Equals("cat"))
                                    .Select(pet => pet.Name)).SelectMany(k => k).ToList();
            catMList.Sort();
            DisplayElements(catMList, "Male");
            catFList.Sort();
            DisplayElements(catFList, "Female");
        }

        private static void DisplayElements(IEnumerable<string> nameList, string gender)
        {
            Console.WriteLine(gender);
            nameList.ToList().ForEach(mcats => Console.WriteLine($"\t{mcats}"));
            Console.WriteLine();
            Console.WriteLine();
        }

        private static async Task<IEnumerable<People>> FetchPeopleData()
        {
            List<People> peopleList;
            string peopleInfo = string.Empty;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

               // var response = await httpClient.GetAsync("http://localhost:55514/api/people");
                var response = await httpClient.GetAsync("Http://agl-developer-test.azurewebsites.net/people.json");
                if (response.IsSuccessStatusCode)
                {
                    peopleInfo = await response.Content.ReadAsStringAsync();
                }
                peopleList = JsonConvert.DeserializeObject<List<People>>(JToken.Parse(peopleInfo).ToString());
            }

            return peopleList;

        }
    }
}
