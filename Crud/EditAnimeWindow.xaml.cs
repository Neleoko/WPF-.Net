using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace WPF_.Net.Crud
{
    public partial class EditAnimeWindow : Window
    {
        public int AnimeID { get; set; }

        public EditAnimeWindow(int animeID)
        {
            InitializeComponent();
            AnimeID = animeID;
            InitializeDataAsync(animeID);
        }

        private async void InitializeDataAsync(int animeID)
        {
            var anime = await LoadAnimeData(animeID);
            if (anime != null)
            {
                NomTextBox.Text = anime.Nom;
                AcronymeTextBox.Text = anime.Acronyme;
            }
        }
        private async Task<AnimeType> LoadAnimeData(int animeID)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync("https://localhost:44304/api/anime/GetAnimeById/?ID=" + animeID);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AnimeType>();
                }
                else
                {
                    throw new Exception($"Erreur du serveur : {response.StatusCode}");
                }
            }
        }

        private async void Save_Button(object sender, RoutedEventArgs e)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PutAsJsonAsync($"https://localhost:44304/api/Anime/UpdateAnime/", new AnimeType
                {
                    ID = AnimeID,
                    Nom = NomTextBox.Text,
                    Acronyme = AcronymeTextBox.Text
                });
                response.EnsureSuccessStatusCode();
            }

            DataUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }
        public event EventHandler DataUpdated;
    }
}