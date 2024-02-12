using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Qbo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Perfil : ContentPage
    {
        SqlConnection con = MainPage.con;
        ObservableCollection<Contenido>  contenidoFavorito = new ObservableCollection<Contenido>();
        ObservableCollection<Plataforma> misPlataformas = new ObservableCollection<Plataforma>();
        public Perfil()
        {
            InitializeComponent();
            pfp.Source = "user.png";
            username.Text = MainPage.user.username;
            email.Text = MainPage.user.email;
            since.Text = MainPage.user.createdAt.ToString("dd-MMM-yyyy");
            checkPfp(MainPage.user.id);
            fetchUserPlatforms();
            listaPlataformas.ItemsSource = misPlataformas;
        }

        protected override void OnAppearing()
        {
            contenidoFavorito = new ObservableCollection<Contenido>();
            fetchFavoriteContent();
            listaFavoritos.ItemsSource = contenidoFavorito;
        }
        public void irASitio(object sender, EventArgs e)
        {
            string url = (string)((TappedEventArgs)e).Parameter;
            Browser.OpenAsync(new Uri(url), BrowserLaunchMode.SystemPreferred);
        }

        void goToDetail(object sender, EventArgs e)
        {
            Contenido c = (Contenido)((StackLayout)sender).BindingContext;
            Navigation.PushAsync(new Detail(c));
        }

        public async void removeFav(object sender, EventArgs e)
        {
            Contenido fav = (Contenido)((Image)sender).BindingContext;

            bool eliminar = await DisplayAlert("Cuidado", $"Estás a punto de eliminar {fav.titulo} de tu lista de favoritos", "Eliminar", "Cancelar");
            if (eliminar)
            {
                sqlRemoveFav(fav);
                contenidoFavorito.Remove(fav);
                listaFavoritos.ItemsSource = contenidoFavorito;
            }
        }

        void sqlRemoveFav(Contenido fav)
        {
            var query = $"DELETE FROM dbo.Favoritos WHERE userId = {MainPage.user.id} AND {(fav is Pelicula ? "peliculaId" : "serieId")} = {fav.id}";
            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"Success in the delete of the favorite.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    DisplayAlert("Error modificando tu favorito", ex.ToString(), "Ok");
                }
            }
        }
        public async void removePlataforma(object sender, EventArgs e)
        {
            Plataforma selPlat = (Plataforma)((Image)sender).BindingContext;

            bool eliminar = await DisplayAlert("Cuidado", $"Estás a punto de eliminar {selPlat.nombre} de tu lista de plataformas", "Eliminar", "Cancelar");
            if (eliminar)
            {
                sqlRemovePlatform(selPlat);
                misPlataformas.Remove(selPlat);
                listaPlataformas.ItemsSource = misPlataformas;
            }
        }

        private void sqlRemovePlatform(Plataforma selPlat)
        {
            string query = $"UPDATE Perfiles SET plataformas = REPLACE(plataformas, ' {selPlat.id} ' , ' ' ) WHERE UserId = {MainPage.user.id}";
            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} row(s) updated.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        public async void addPlataforma(object sender, EventArgs args)
        {
            string[] options = MainPage.plataformas.Except(misPlataformas).Select(p => p.nombre).ToArray();
            string selected = await DisplayActionSheet("Selecciona una plataforma a añadir", "Cancelar", null, options);
            Plataforma selPlat = MainPage.plataformas.First(p => p.nombre == selected);
            sqlAddPlatform(selPlat);
            misPlataformas.Add(selPlat);
            listaPlataformas.ItemsSource = misPlataformas;
        }

        public void sqlAddPlatform(Plataforma plat)
        {
            string query = $"UPDATE Perfiles SET plataformas = plataformas + '{plat.id} ' WHERE UserId = {MainPage.user.id}";
            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} row(s) updated.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                 }
            }
        }

        private void fetchUserPlatforms()
        {
            string query = $"SELECT plataformas from Perfiles WHERE UserId = {MainPage.user.id}";
            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            if (reader.HasRows)
                            {
                                if ((string)reader["plataformas"] == "")
                                {
                                    Debug.WriteLine("The user has no platforms");
                                }
                                else
                                {
                                    string[] platformIds= ((string)reader["plataformas"]).ToString().Split(' ');
                                    foreach (string platformId in platformIds)
                                    {
                                        if(MainPage.plataformas.Select(p => p.id.ToString()).Contains(platformId))
                                        {
                                            misPlataformas.Add(MainPage.plataformas.First(p => p.id.ToString() == platformId));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (InvalidOperationException ioe)
                {
                    Debug.WriteLine(ioe.ToString());
                    if (ioe.Message.Contains("data is present"))
                    {
                        DisplayAlert("Usuario no encontrado", "Revise que su correo electrónico y contraseña sean correctos.", "Ok");
                    }
                }
            }
        }

        public void fetchFavoriteContent()
        {
            string pelisFav = !Home.pelisFavoritas.Any() ? "null" : string.Join(", ", Home.pelisFavoritas.Select(item => $"'{item}'"));
            string seriesFav = !Home.seriesFavoritas.Any() ? "null" : string.Join(", ", Home.seriesFavoritas.Select(item => $"'{item}'"));
            string queryPelis = $"SELECT * FROM dbo.Peliculas WHERE IDpelicula IN ({pelisFav})";
            string querySeries = $"SELECT * FROM dbo.Series WHERE IDserie IN ({seriesFav})";
            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(queryPelis, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Pelicula p = new Pelicula(
                                   (string)reader["IDpelicula"],
                                   (string)reader["Titulo"],
                                   (string)reader["País"],
                                   Convert.ToUInt16(reader["Año"], CultureInfo.InvariantCulture.NumberFormat),
                                   (TimeSpan)reader["Duracion"],
                                   (string)reader["Sinopsis"],
                                   (string)reader["Director"],
                                   (string)reader["Guión"],
                                   (string)reader["Reparto"],
                                   (string)reader["Etiquetas"],
                                   (string)reader["Calificacion"],
                                   (string)reader["IDplatPeliculas"]
                                );
                                p.imagen = Home.LoadImage(p.titulo, "movie");
                                contenidoFavorito.Add(p);
                            }
                        }
                    }
                    using (SqlCommand command = new SqlCommand(querySeries, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Serie s = new Serie(
                                    (string)reader["IDserie"],
                                    (string)reader["Titulo"],
                                    (string)reader["País"],
                                    Convert.ToUInt16(reader["Año"], CultureInfo.InvariantCulture.NumberFormat),
                                    (TimeSpan)reader["Duracion"],
                                    (string)reader["Sinopsis"],
                                    (string)reader["Director"],
                                    (string)reader["Reparto"],
                                    (string)reader["Etiquetas"],
                                    (string)reader["Calificacion"],
                                    Convert.ToUInt16(reader["Temporada"], CultureInfo.InvariantCulture.NumberFormat),
                                    Convert.ToUInt16(reader["Capitulos"], CultureInfo.InvariantCulture.NumberFormat),
                                    (string)reader["IDplatSeries"]
                                    );
                                s.imagen = Home.LoadImage(s.titulo,"tv");
                                contenidoFavorito.Add(s);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    DisplayAlert("ERROR:", ex.ToString(), "Ok");
                }
            }
        }

        void cambiaPfp(object sender, EventArgs args)
        {
            PickPhotoAsync();
        }

        private async void PickPhotoAsync()
        {
            try
            {
                bool hasPermission = await CheckAndRequestStoragePermission();
                if (!hasPermission)
                    return;

                var foto = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Elige tu foto de perfil"
                });

                if (foto != null)
                {
                    byte[] imageData = File.ReadAllBytes(foto.FullPath);
                    updateImage(imageData);
                    pfp.Source = foto.FullPath;
                }
            }
            catch (FeatureNotSupportedException)
            {
                // MediaPicker is not supported on this device
                await DisplayAlert("Not Supported", "MediaPicker is not supported on this device.", "OK");
            }
            catch (PermissionException)
            {
                // Missing required permissions
                await DisplayAlert("Permission Denied", "Storage permission is required to pick a photo.", "OK");
            }
            catch (Exception ex)
            {
                // Other exceptions
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        private async Task<bool> CheckAndRequestStoragePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();

            if (status != PermissionStatus.Granted)
            {
                var requestResult = await Permissions.RequestAsync<Permissions.Photos>();

                if (requestResult != PermissionStatus.Granted)
                {
                return false;
                }
            }

            return true;
        }
        private void updateImage(byte[] img)
        {
            var query = $"UPDATE Perfiles SET pfp = @ImageData WHERE UserId = {MainPage.user.id}";
            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@ImageData", img);
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} row(s) updated.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    DisplayAlert("Error subiendo tu imagen", "Ha ocurrido un error por favor prueba tu conexión", "Ok");
                }
            }
        }

        private void checkPfp(int id)
        {
            var query = $"SELECT pfp from Perfiles WHERE UserId = {id}";
            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            reader.Read();
                            if (reader.HasRows)
                            {
                                if (reader["pfp"] == null)
                                {
                                    Debug.WriteLine("The user has no profile pic");
                                }
                                else
                                {
                                    try
                                    {
                                        byte[] byteArray = (byte[])reader["pfp"];
                                        MemoryStream memoryStream = new MemoryStream(byteArray);
                                        pfp.Source = ImageSource.FromStream(() => memoryStream);
                                    }
                                    catch(Exception ex)
                                    {
                                        Debug.WriteLine(ex.ToString());
                                    }
                                }
                            }
                        }
                    }
                }
                catch (InvalidOperationException ioe)
                {
                    Debug.WriteLine(ioe.ToString());
                    if (ioe.Message.Contains("data is present"))
                    {
                        DisplayAlert("Usuario no encontrado", "Revise que su correo electrónico y contraseña sean correctos.", "Ok");
                    }
                }
            }
        }
    }
}