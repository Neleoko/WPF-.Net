using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace WPF_.Net.Crud
{
    public partial class AnimeCrud : Window
    {
        public ObservableCollection<AnimeData> Animes { get; set; }

        public AnimeCrud()
        {
            InitializeComponent();
            InitializeDataAsync();
        }

        public class AnimeData
        {
            public int ID { get; set; }
            public string Nom { get; set; }
            public string Acronyme { get; set; }
        }
        private async Task InitializeDataAsync()
        {
            List<AnimeData> animeDatas = await LoadData();
            Animes = new ObservableCollection<AnimeData>(animeDatas);
            AnimeDataGrid.ItemsSource = Animes;
        }
        private async Task<List<AnimeData>> LoadData()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:44304/api/anime/GetAnime");
                response.EnsureSuccessStatusCode();
                
                if (response.IsSuccessStatusCode)
                { 
                    return await response.Content.ReadFromJsonAsync<List<AnimeData>>();
                }
                else
                {
                    throw new Exception($"Erreur du serveur : {response.StatusCode}");
                }
            }
        }

        private async void Add_Button(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Update_Button(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Delete_Button(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}