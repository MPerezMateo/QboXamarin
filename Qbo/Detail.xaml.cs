using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Qbo
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Detail : ContentPage
	{
		public Contenido contenido;
		public bool favorito = false;
		bool esPelicula;
        SqlConnection con = MainPage.con;

        public Detail (Contenido c)
		{
            InitializeComponent();
            contenido = c ;
			BindingContext = contenido;
			esPelicula = (c is Pelicula);
            favorito = esPelicula ? Home.pelisFavoritas.Contains(contenido.id) : Home.seriesFavoritas.Contains(contenido.id);
			if (favorito)
			{
				Fav.Text = "Eliminar de mi lista de favoritos";
			}
		}

		void modificarFavorito(object sender, EventArgs args) {
			modifyFav();
			if (favorito){
                (esPelicula ? Home.pelisFavoritas : Home.seriesFavoritas).Remove(contenido.id);
                Fav.Text = "Agregar a favoritos";
                favorito = false;
			}
			else
			{
                (esPelicula ? Home.pelisFavoritas : Home.seriesFavoritas).Add(contenido.id);
                Fav.Text = "Eliminar de mi lista de favoritos";
                favorito = true;
            }
        }

        void modifyFav() {
			var queryDelete = $"DELETE FROM dbo.Favoritos WHERE userId = {MainPage.user.id} AND {(esPelicula ? "peliculaId" : "serieId" )} = {contenido.id}";
			var queryInsert = $"INSERT INTO dbo.Favoritos VALUES({MainPage.user.id}, {(esPelicula ? contenido.id : "NULL")}, {(esPelicula ? "NULL" : contenido.id)}, GETDATE())";
            var query = favorito ? queryDelete : queryInsert;
            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"Success in the {(favorito ? "delete" : "insertion")} of the favorite.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                    DisplayAlert("Error modificando tu favorito", ex.ToString(), "Ok");
                }
            }
        }
		void valorar(object sender, EventArgs args) { }

        async void sugerir(object sender, EventArgs args) {
           await ShareUri();
        }
        public async Task ShareUri()
        {
            string url = null;
            var options = new RestClientOptions($"https://api.themoviedb.org/3/search/{(esPelicula ? "movie" : "tv")}?query={contenido.titulo.ToLower().Replace(" ", "+")}&include_adult=false&language=es-ES&page=1");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {App.tokenTMDB}");
            var response = client.Get(request);

            string pattern = @"""id"": (\d+)";

            Match match = Regex.Match(response.Content.ToString(), pattern);

            if (match.Success)
            {
                string id = match.Groups[1].Value;

                url = $"https://www.themoviedb.org/movie/{id}";
            }
            else
            {
                Console.WriteLine("No id found in the JSON string.");
            }
            if (url != null)
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = url,
                    Title = "Compártelo con tus amigos"
                });
            }
        }


    }
}