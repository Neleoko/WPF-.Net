using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;

namespace WPF_.Net.Crud;

public partial class EditTypeWindow : Window
{
   public int TypeID { get; set; }
   
           public EditTypeWindow(int typeId)
           {
               InitializeComponent();
               TypeID = typeId;
               InitializeDataAsync(typeId);
           }
   
           private async void InitializeDataAsync(int typeId)
           {
               var type = await LoadTypeData(typeId);
               if (type != null)
               {
                   NomTextBox.Text = type.Nom;
                   AcronymeTextBox.Text = type.Nom;
               }
           }
           private async Task<TypeType> LoadTypeData(int typeID)
           {
               using (HttpClient client = new HttpClient())
               {
                   var response = await client.GetAsync("https://localhost:44304/api/type/GetTypeById/?ID=" + typeID);
                   response.EnsureSuccessStatusCode();
   
                   if (response.IsSuccessStatusCode)
                   {
                       return await response.Content.ReadFromJsonAsync<TypeType>();
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
                   var response = await client.PutAsJsonAsync($"https://localhost:44304/api/type/UpdateType/", new TypeType
                   {
                       ID = TypeID,
                       Nom = NomTextBox.Text,
                   });
                   response.EnsureSuccessStatusCode();
               }
   
               DataUpdated?.Invoke(this, EventArgs.Empty);
               Close();
           }
           public event EventHandler DataUpdated;
}