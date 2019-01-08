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
    class Program
    {
       
        static void Main(string[] args)
        {
            for(int i=0;i<JsonParser.NUM_PAGES;i++)
            {
                JsonParser.Execute(i, 120000, true);
            }

            Console.WriteLine("А теперь о 90% населения РФ\n");

            for (int i = 0; i < JsonParser.NUM_PAGES; i++)
            {
                JsonParser.Execute(i, 15000, false);
            }
            Console.ReadKey();
        }
    }
}
