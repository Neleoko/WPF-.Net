using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF_.Net.Crud
{
    public partial class AnimeCrud : Window
    {
        public ObservableCollection<AnimeType> Animes { get; set; }

        public AnimeCrud()
        {
            InitializeComponent();
            InitializeDataAsync();
        }

        private async Task InitializeDataAsync()
        {
            List<AnimeType> animeDatas = await LoadData();
            Animes = new ObservableCollection<AnimeType>(animeDatas);
            AnimeDataGrid.ItemsSource = Animes;
        }

        private async Task<List<AnimeType>> LoadData()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:44304/api/anime/GetAnime");
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<AnimeType>>();
                }
                else
                {
                    throw new Exception($"Erreur du serveur : {response.StatusCode}");
                }
            }
        }

        private async void Add_Button(object sender, RoutedEventArgs e)
        {
            var newAnime = new AnimeType
            {
                Nom = NomTextBox.Text,
                Acronyme = AcronymeTextBox.Text
            };

            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostAsJsonAsync("https://localhost:44304/api/Anime/InsertAnime", newAnime);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    // Refresh the data
                    List<AnimeType> animeDatas = await LoadData();
                    Animes.Clear();
                    foreach (var anime in animeDatas)
                    {
                        Animes.Add(anime);
                    }
                }
                else
                {
                    MessageBox.Show("Error Code: " + response.StatusCode);
                }
            }
        }

        private async void Update_Button(object sender, RoutedEventArgs e)
        {
            var ButtonUpdate = sender as Button;
            var selectedAnime = ButtonUpdate?.Tag as AnimeType;

            if (selectedAnime != null)
            {
                var editWindow = new EditAnimeWindow(selectedAnime.ID);
                editWindow.DataUpdated += async (s, args) =>
                {
                    List<AnimeType> animeDatas = await LoadData();
                    Animes.Clear();
                    foreach (var anime in animeDatas)
                    {
                        Animes.Add(anime);
                    }
                };

                editWindow.ShowDialog();
            }
        }

        private void Delete_Button(object sender, RoutedEventArgs e)
        {
            var ButtonDelete = sender as Button;
            var selectedAnime = ButtonDelete?.Tag as AnimeType;
            
            using (HttpClient client = new HttpClient())
            {
                var response = client.DeleteAsync("https://localhost:44304/api/Anime/DeleteAnime?Id=" + selectedAnime.ID).Result;
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    Animes.Remove(selectedAnime);
                }
                else
                {
                    MessageBox.Show("Error Code: " + response.StatusCode);
                }
            }
            
        }
    }
}