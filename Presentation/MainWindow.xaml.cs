using System.Net.Http;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BattleShip.Presentation;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");

        public MainWindow()
        {
            InitializeComponent();
        }
        private async void InitGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.PostAsync("https://localhost:7190/game/init", null);
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(responseString);
                ResultTextBlock.Text = result.message;
                InitButton.IsEnabled = false;
                FireButton.IsEnabled = true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                //throw;
            }
        }

        private async void FireMissile_Click(object sender, RoutedEventArgs e)
        {
            Match regExpressResult = re.Match(CoordinatesTextBox.Text);

            if(regExpressResult.Groups.Count != 3)
                FireResponseResult.Text = string.Concat(FireResponseResult.Text, Environment.NewLine, CoordinatesTextBox.Text + " are Invalid Coordinates");
            else
            {
                char x = Char.ToUpper(regExpressResult.Groups[1].Value.First());
                string yStringValue = regExpressResult.Groups[2].Value;
                var y = int.Parse(yStringValue);

                if (x > 'J' || y > 10 || y < 1)
                {
                    ResultTextBlock.Text =
                        FireResponseResult.Text = string.Concat(FireResponseResult.Text, Environment.NewLine, CoordinatesTextBox.Text + " are Invalid Coordinates");
                }
                else
                {
                    var payload = new { x, y };

                    var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                        "application/json");

                    var response = await client.PostAsync("https://localhost:7190/game/fire", content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(responseString);
                    FireResponseResult.Text = string.Concat(FireResponseResult.Text, Environment.NewLine,
                        CoordinatesTextBox.Text, " ", result.message);
                }
            }
        }
    }
}