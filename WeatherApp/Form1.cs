using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using IWshRuntimeLibrary;

namespace WeatherApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            panel1.Visible = true;
            panel2.Visible = false;

            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Shortcut(folder);

            #region Current Weather API 
            string city = "Riga";
            string url = $"https://api.openweathermap.org/data/2.5/weather?&q={city}&lang=en&appid=154d1eeac867d0df4b0eba68a5638835&units=metric";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string response = streamReader.ReadToEnd();

            WeatherData weatherResponse = JsonConvert.DeserializeObject<WeatherData>(response);

            var sunrise = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunrise);
            var sunset = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunset);    
            #endregion

            #region First Panel Labels
            // Main data
            label1.Text = WeatherData.Name + ", " + weatherResponse.Sys.Country;
            label3.Text = Math.Round(weatherResponse.Main.Temperature) + "°C";
            label5.Text = "Feels like: " + Math.Round(weatherResponse.Main.FeelsLikeTemperature) + "°C";
            label6.Text = "Weather: " + WeatherData.WeatherInfo[0].Description;
            label7.Text = "Wind speed: " + weatherResponse.Sys.Speed + " m/s";
            label8.Text = "Humidity: " + weatherResponse.Main.Humidity;

            // Details
            label2.Text = "Today`s details";
            label4.Text = "Sunrise: " + sunrise.LocalDateTime.ToString("HH:mm");
            label9.Text = "Sunset: " + sunset.LocalDateTime.ToString("HH.mm");
            label11.Text = "Max: " + weatherResponse.Main.MaxTemperature + "°C";
            label12.Text = "Min: " + weatherResponse.Main.MinTemperature + "°C";

            // Header Clock
            label10.Text = DateTime.Now.ToString("HH:mm");
                #endregion

            #region Second Panel Labels
            label13.Text = DateTime.Now.ToString("HH:mm");
            #endregion
        }

        #region Button Controls
        private void button1_Click(object sender, EventArgs e)
        {
            this.Invalidate();
            panel2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Invalidate();
            panel2.Visible = false;
            panel1.Visible = true;
        }
        #endregion

        private void Shortcut(string folder)
        {
            WshShell wshShell = new WshShell();
            string fileName = folder + "\\" + ProductName + ".lnk";
            IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(fileName);
            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.IconLocation = "Resources\\icon.ico";
            shortcut.Save();
        }
    }

    #region Json
    public class WeatherData 
    {
        
        public TemperatureData Main { get; set; }

        [JsonProperty("name")]
        public static string Name { get; set; }

        public WeatherSysData Sys { get; set; }

        [JsonProperty("weather")]
        public static WeatherInfo[] WeatherInfo { get; set; }
    }

    public class WeatherSysData
    {
        [JsonProperty("sunrise")]
        public int Sunrise { get; set; }

        [JsonProperty("sunset")]
        public int Sunset { get; set; }

        [JsonProperty("speed")]
        public float Speed { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }
    }

    public class TemperatureData
    {
        [JsonProperty("feels_like")]
        public float FeelsLikeTemperature { get; set; }

        [JsonProperty("temp")]
        public float Temperature { get; set; }

        [JsonProperty("temp_max")]
        public float MaxTemperature { get; set; }

        [JsonProperty("temp_min")]
        public float MinTemperature { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }
    }

    public class WeatherInfo
    {
        [JsonProperty("description")]
        public string Description { get; set; }
    }
    #endregion
}
