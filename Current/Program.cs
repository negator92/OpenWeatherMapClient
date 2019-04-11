using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Current
{
    class Program
    {
        static void Main()
        {
            string token = "your token here";
            ParseJson(token).Wait();
            ParseXml(token);
        }

        public async static Task ParseJson(string token)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string address = $"http://api.openweathermap.org/data/2.5/weather?q=Moscow&appid={token}&units=metric&lang=ru&mode=json";
                Owm owm = Owm.FromJson(await httpClient.GetStringAsync(address));
                Console.WriteLine($"temperature {owm.Main.Temp} min {owm.Main.TempMin} max {owm.Main.TempMax}\nhumidity {owm.Main.Humidity}%\npressure {Math.Round(owm.Main.Pressure/0.75006375541921)}mmHg");
            }
        }

        public static void ParseXml(string token)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string address =
                    $"http://api.openweathermap.org/data/2.5/weather?q=Moscow&appid={token}&units=metric&lang=ru&mode=xml";
                Task<byte[]> bytesTask = httpClient.GetByteArrayAsync(address);
                bytesTask.Wait();
                byte[] bytes = bytesTask.Result; 
                string xmlStringUTF = Encoding.UTF8.GetString(bytes);
                XDocument xDocument = XDocument.Parse(xmlStringUTF);
                XElement temperature = xDocument.XPathSelectElement("//temperature");
                Console.WriteLine($"temperature {temperature.Attribute("value").Value} min {temperature.Attribute("min").Value} max {temperature.Attribute("max").Value}");
                XElement humidity = xDocument.XPathSelectElement("//humidity");
                Console.WriteLine($"humidity {humidity.Attribute("value").Value}%");
                XElement pressure = xDocument.XPathSelectElement("//pressure");
                Console.WriteLine($"pressure {Math.Round(double.Parse(pressure.Attribute("value").Value)/0.75006375541921)}mmHg");
            }
        }
    }
}