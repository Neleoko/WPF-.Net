using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace WPF_.Net.Crud;

public partial class TypeCrud : Window
{
    public TypeCrud()
    {
        InitializeComponent();
        InitializeDataAsync();
    }
    
    public ObservableCollection<TypeType> Types { get; set; }

    private async Task InitializeDataAsync()
    {
        var typeDatas = await LoadData();
        Types = new ObservableCollection<TypeType>(typeDatas);
        TypeDataGrid.ItemsSource = Types;
    }

    private async Task<List<TypeType>> LoadData()
    {
        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetAsync("https://localhost:44304/api/Type/GetType");
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<TypeType>>();
            throw new Exception($"Erreur du serveur : {response.StatusCode}");
        }
    }

    private async void Add_Button(object sender, RoutedEventArgs e)
    {
        var newType = new TypeType
        {
            Nom = NomTextBox.Text,
        };

        using (HttpClient client = new HttpClient())
        {
            var response = await client.PostAsJsonAsync("https://localhost:44304/api/Type/InsertType", newType);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show("Type ajouté avec succès !");
                // Refresh the data
                var typeDatas = await LoadData();
                Types.Clear();
                foreach (var type in typeDatas) Types.Add(type);
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
        var selectedType = ButtonUpdate?.Tag as TypeType;

        if (selectedType != null)
        {
            var editWindow = new EditAnimeWindow(selectedType.ID);
            editWindow.DataUpdated += async (s, args) =>
            {
                var typeDatas = await LoadData();
                Types.Clear();
                foreach (var type in typeDatas) Types.Add(type);
            };

            editWindow.ShowDialog();
        }
    }

    private void Delete_Button(object sender, RoutedEventArgs e)
    {
        var ButtonDelete = sender as Button;
        var selectedType = ButtonDelete?.Tag as TypeType;

        using (HttpClient client = new HttpClient())
        {
            var response = client.DeleteAsync("https://localhost:44304/api/Type/DeleteType?Id=" + selectedType.ID)
                .Result;
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
                Types.Remove(selectedType);
            else
                MessageBox.Show("Error Code: " + response.StatusCode);
        }
    }
}