using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiHH
{
    class JsonParser
    {
        public const int NUM_PAGES = 20; //Костыль под 100 записей на странице
        public static int counterOfVacancyELITE = 1; //Для норм челов
        public static int counterOfVacancyNISHIE = 1; //)))
        private const string path = "https://api.hh.ru/vacancies?only_with_salary=true&salary>120000&salary.currency=RUR&per_page=100";
        public static string GetContent(string url)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                webClient.Headers.Add(HttpRequestHeader.UserAgent, "HH-User-Agent");
                string response = webClient.DownloadString(url);
                var fromEncoding = Encoding.Default;
                var bytes = fromEncoding.GetBytes(response);
                var toEncoding = Encoding.UTF8;
                response = toEncoding.GetString(bytes);
                return response;
            }
        }

        public static string GetVacanciesKeySkills(string url)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
                webClient.Headers.Add(HttpRequestHeader.UserAgent, "HH-User-Agent");
                string response = webClient.DownloadString(url);
                var fromEncoding = Encoding.Default;
                var bytes = fromEncoding.GetBytes(response);
                var toEncoding = Encoding.UTF8;
                response = toEncoding.GetString(bytes);
                return response;
            }
        }

        /*public static string ChangeEncoding(ref string content)
        {
            var fromEncoding = Encoding.Default;
            var bytes = fromEncoding.GetBytes(content);
            var toEncoding = Encoding.UTF8;
            content = toEncoding.GetString(bytes);
            return content;
        }*/

        public static double CheckSalary(JToken vacancy)
        {
            JToken salaryFrom = vacancy["salary"]["from"];
            JToken salaryTo = vacancy["salary"]["to"];

            if (salaryFrom.Type == JTokenType.Null)
            {
                return (double)salaryTo;
            }
            else if (salaryTo.Type == JTokenType.Null)
            {
                return (double)salaryFrom;
            }
            else if (salaryFrom.Type != JTokenType.Null && salaryTo.Type != JTokenType.Null)
            {
                return ((double)salaryFrom + (double)salaryTo) / 2;
            }
            else
                return 0D;
        }

        public static string CheckKeySkills(JToken vacancy)
        {
            string keySkills = null;
            bool first = true;
            if (vacancy["key_skills"].HasValues)
            {
                foreach (var key in vacancy["key_skills"])
                {
                    if (first)
                    {
                        keySkills += key["name"].ToString();
                        first = false;
                    }
                    else
                        keySkills += (", " + key["name"].ToString());
                }
                return keySkills;
            }
            else
                return "-";
        }

        public static void Execute(int iteration, double nominalSalary, bool isGreater)
        {
            string content = GetContent(path + "&page=" + iteration.ToString());
            //ChangeEncoding(ref content);
            JArray vacancies = (JArray)JObject.Parse(content)["items"];
            int numOfPages = (int)JObject.Parse(content)["pages"];
            int numOfVacancies = (int)JObject.Parse(content)["found"];
            double salaryOfVacancy = 0;
            string keySkills = null;
            foreach (var vacancy in vacancies)
            {
                salaryOfVacancy = CheckSalary(vacancy);
                if (isGreater)
                {
                    if (salaryOfVacancy >= nominalSalary)
                    {
                        JToken vacancyKeySkills = JObject.Parse(GetVacanciesKeySkills("https://api.hh.ru/vacancies/" + vacancy["id"].ToString()));
                        keySkills = CheckKeySkills(vacancyKeySkills);
                        Console.WriteLine(counterOfVacancyELITE.ToString() + ". " + vacancy["name"].ToString());
                        Console.WriteLine(" - Зарплата: " + salaryOfVacancy.ToString() + " рублей");
                        Console.WriteLine(" - Ключевые навыки: " + keySkills);
                        Console.WriteLine();
                        counterOfVacancyELITE++;
                    }
                }
                else
                {
                    if (salaryOfVacancy < nominalSalary)
                    {
                        JToken vacancyKeySkills = JObject.Parse(GetVacanciesKeySkills("https://api.hh.ru/vacancies/" + vacancy["id"].ToString()));
                        keySkills = CheckKeySkills(vacancyKeySkills);
                        Console.WriteLine(counterOfVacancyNISHIE.ToString() + ". " + vacancy["name"].ToString());
                        Console.WriteLine(" - Зарплата: " + salaryOfVacancy.ToString() + " рублей");
                        Console.WriteLine(" - Ключевые навыки: " + keySkills);
                        Console.WriteLine();
                        counterOfVacancyNISHIE++;
                    }
                }
            }
        }
    }
}
