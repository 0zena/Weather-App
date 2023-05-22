using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using IWshRuntimeLibrary;
using System.Collections.Generic;
using System.Globalization;

namespace WeatherApp
{
    public partial class Form1 : Form
    {
        private string City;

        public Form1()
        {
            InitializeComponent();

            const string key = "154d1eeac867d0df4b0eba68a5638835";
            if (string.IsNullOrEmpty(City)) { City = "Riga"; }

            panel1.Visible = true;
            panel2.Visible = false;

            string folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Shortcut(folder);

            #region Current Weather API
            string url = $"https://api.openweathermap.org/data/2.5/weather?&q={City}&lang=en&appid={key}&units=metric";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            string response = streamReader.ReadToEnd();

            WeatherData weatherResponse = JsonConvert.DeserializeObject<WeatherData>(response);

            var sunrise = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunrise);
            var sunset = DateTimeOffset.FromUnixTimeSeconds(weatherResponse.Sys.Sunset);
            #endregion

            #region Forecast Weather API
            string f_url = $"https://api.openweathermap.org/data/2.5/forecast?&q={City}&lang=en&appid={key}&units=metric";

            HttpWebRequest httpWebRequest1 = (HttpWebRequest)WebRequest.Create(f_url);
            HttpWebResponse httpWebResponse1 = (HttpWebResponse)httpWebRequest1.GetResponse();

            StreamReader streamReader1 = new StreamReader(httpWebResponse1.GetResponseStream());
            string response1 = streamReader1.ReadToEnd();

            ForecastRoot forecastRoot = JsonConvert.DeserializeObject<ForecastRoot>(response1);
            
            List<Forecast> forecasts = forecastRoot.Forecasts;
            #endregion

            #region First Panel Labels
            // Main data
            label1.Text = WeatherData.Name + ", " + weatherResponse.Sys.Country;
            label3.Text = Math.Round(weatherResponse.Main.Temperature) + "°C";
            label5.Text = "Feels like: " + Math.Round(weatherResponse.Main.FeelsLikeTemperature) + "°C";
            label6.Text = "Weather: " + WeatherData.WeatherInfo[0].Description;
            label7.Text = "Wind speed: " + weatherResponse.Wind.Speed + " m/s";
            label8.Text = "Humidity: " + weatherResponse.Main.Humidity;

            // Details
            label2.Text = "Today`s details";
            label4.Text = "Sunrise: " + sunrise.LocalDateTime.ToString("HH:mm");
            label9.Text = "Sunset: " + sunset.LocalDateTime.ToString("HH:mm");
            label11.Text = "Max: " + Math.Round(weatherResponse.Main.MaxTemperature) + "°C";
            label12.Text = "Min: " + Math.Round(weatherResponse.Main.MinTemperature) + "°C";

            pictureBox1.LoadAsync($"https://openweathermap.org/img/wn/{WeatherData.WeatherInfo[0].Icons}@2x.png");

            // Header Clock
            label10.Text = DateTime.Now.ToString("HH:mm");
            #endregion

            #region Forecast data processing
            try
            {
                List<string> ForecastList = new List<string>();

                for (int i = 0; i < forecasts.Count; i++)
                {
                    if (i % 8 == 0)
                    {
                        string DatePath = DateTime.ParseExact(forecasts[i].DateText.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dddd");
                        string listItem = $"\t{DatePath}\nTemperature: {Math.Round(forecasts[i].Main.Temperature)}°C\nHumidity: {forecasts[i].Main.Humidity}\n";
                        ForecastList.Add(listItem);
                    }
                }
                #region Second Panel Labels
                label13.Text = DateTime.Now.ToString("HH:mm");
                label15.Text = ForecastList[1];
                label16.Text = ForecastList[2];
                label17.Text = ForecastList[3];
                label18.Text = ForecastList[4];
                #endregion
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error: {exception.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }

        #region Button Controls
        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
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
            shortcut.Save();
        }

        public void textBox1_TextChanged(object sender, EventArgs e)
        {
            InitializeComponent();
        }
    }

    #region Json Current
    public class WeatherData 
    {
        public TemperatureData Main { get; set; }

        [JsonProperty("name")]
        public static string Name { get; set; }

        public WeatherSysData Sys { get; set; }

        [JsonProperty("weather")]
        public static WeatherInfo[] WeatherInfo { get; set; }

        [JsonProperty("wind")]
        public WindInfo Wind { get; set; }
    }

    public class WindInfo
    {
        [JsonProperty("speed")]
        public float Speed { get; set; }
    }

    public class WeatherSysData
    {
        [JsonProperty("sunrise")]
        public int Sunrise { get; set; }

        [JsonProperty("sunset")]
        public int Sunset { get; set; }

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
        
        [JsonProperty("icon")]
        public string Icons { get; set; }
    }
    #endregion

    #region Json Forecast
    public class ForecastRoot
    {
        [JsonProperty("list")]
        public List<Forecast> Forecasts { get; set; }
    }


    public class Forecast
    {
        [JsonProperty("dt_txt")]
        public string DateText { get; set; }

        [JsonProperty("main")]
        public MainInfo Main { get; set; }
    }
    public class MainInfo
    {
        [JsonProperty("temp")]
        public double Temperature { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }
    }

    #endregion
}
