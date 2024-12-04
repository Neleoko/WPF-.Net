using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace WPF_.Net.Crud;

public partial class AnimeCrud : Window
{
    public AnimeCrud()
    {
        InitializeComponent();
        InitializeDataAsync();
    }

    public ObservableCollection<AnimeType> Animes { get; set; }

    private async Task InitializeDataAsync()
    {
        var animeDatas = await LoadData();
        Animes = new ObservableCollection<AnimeType>(animeDatas);
        AnimeDataGrid.ItemsSource = Animes;
    }

    private async Task<List<AnimeType>> LoadData()
    {
        using (var client = new HttpClient())
        {
            var response = await client.GetAsync("https://localhost:44304/api/anime/GetAnime");
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<AnimeType>>();
            throw new Exception($"Erreur du serveur : {response.StatusCode}");
        }
    }

    private async void Add_Button(object sender, RoutedEventArgs e)
    {
        if (NomTextBox.Text == "" || AcronymeTextBox.Text == "")
        {
            MessageBox.Show("Veuillez remplir les champs Nom et Acronyme");
            return;
        }

        var newAnime = new AnimeType
        {
            Nom = NomTextBox.Text,
            Acronyme = AcronymeTextBox.Text
        };

        using (var client = new HttpClient())
        {
            var response = await client.PostAsJsonAsync("https://localhost:44304/api/Anime/InsertAnime", newAnime);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                // Refresh the data
                var animeDatas = await LoadData();
                Animes.Clear();
                foreach (var anime in animeDatas) Animes.Add(anime);
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
                var animeDatas = await LoadData();
                Animes.Clear();
                foreach (var anime in animeDatas) Animes.Add(anime);
            };

            editWindow.ShowDialog();
        }
    }

    private void Delete_Button(object sender, RoutedEventArgs e)
    {
        var ButtonDelete = sender as Button;
        var selectedAnime = ButtonDelete?.Tag as AnimeType;

        using (var client = new HttpClient())
        {
            var response = client.DeleteAsync("https://localhost:44304/api/Anime/DeleteAnime?Id=" + selectedAnime.ID)
                .Result;
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
                Animes.Remove(selectedAnime);
            else
                MessageBox.Show("Error Code: " + response.StatusCode);
        }
    }
}