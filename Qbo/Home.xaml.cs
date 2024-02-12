using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Qbo
{
    public partial class Home : ContentPage
    {
        public ObservableCollection<Contenido> contenido = new ObservableCollection<Contenido>();
        public ObservableCollection<Contenido> contenidoMostrar = new ObservableCollection<Contenido>();

        SqlConnection con = MainPage.con;
        List<Plataforma> plataformas = MainPage.plataformas;
        Plataforma plataformaSeleccionada = new Plataforma(-999999,"Todas las plataformas", 0.0f, "", "", "");
        public static ObservableCollection<string> pelisFavoritas = new ObservableCollection<string>(); // Recuperamos de la bbdd los favoritos de nuestro usuario
        public static ObservableCollection<string> seriesFavoritas = new ObservableCollection<string>(); // Recuperamos de la bbdd los favoritos de nuestro usuario

        List<string> generos = new List<string>() { "Todos los géneros", "Acción", "Adolescencia", "Alpinismo", "Amistad", "Animación", "Años 50", "Aventuras", "Basado hechos reales", "Bélico", "Biográfico", "Casas encantadas", "Ciencia ficción", "Cine épico", "Cine familiar", "Cine negro", "Comeda", "Comedia", "Cómic", "Crimen", "Documental", "Documental sobre cine", "Drama", "Drama judicial", "Espionaje", "Extraterrestres", "Fantasía", "Fantástico", "IA", "II Guerra Mundial", "Infantil", "Intriga", "Magia", "Medieval", "Monstruos", "Musical", "Musical Navidad", "Natación", "Nazismo", "Periodismo", "Pokémon", "Política", "Prehistoria", "Religión", "Romance", "Secuela", "Sobrenatural", "Terror", "Thriller", "Vida rural", "Videojuego", "Western" };
        string generoSeleccionado = "Todos los géneros";

        List<string> media = new List<string>() { "Películas y Series", "Solo Series", "Solo Películas" };
        string mediaSeleccionado = "Películas y Series";

        List<string> criterios = new List<string>() { "Mejor Valoración", "Más reciente", "Más antiguo" };
        string criterioSeleccionado = "Mejor Valoración";

        public Home()
        {
            InitializeComponent();
            List<Plataforma> ps = plataformas;
            ps.Add(plataformaSeleccionada);
            platPicker.ItemsSource = ps;
            platPicker.SelectedItem = plataformaSeleccionada;
            etqPicker.ItemsSource = generos;
            etqPicker.SelectedItem = generoSeleccionado;
            mediaPicker.ItemsSource = media;
            mediaPicker.SelectedItem= mediaSeleccionado;
            orderPicker.ItemsSource = criterios;
            orderPicker.SelectedItem = criterioSeleccionado;
            fetchMovies();
            fetchSeries();
            fetchFavourites();
            contenido = new ObservableCollection<Contenido>(contenido.OrderByDescending(x => x.anio).ThenBy(x => x.calificacion));
            contenidoMostrar = contenido;
            listaContenidos.ItemsSource = contenidoMostrar;
        }

        
        private void fetchMoviesSP()
        {
            string plat = "Netflix";
            string etq = "Drama";
            using (con = new SqlConnection(MainPage.constring))
            {
                con.Open();

                using (SqlCommand command = new SqlCommand("BuscaPeliculas", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PlataformasSuscrito", plat);
                    command.Parameters.AddWithValue("@Etiqueta", etq);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        DisplayAlert("Exito ", reader.GetString(1) + " por: " + reader.GetString(6), "Ok");
                    }

                }
            }
        }


        private void fetchMovies()
        {
            var query = "SELECT TOP 25 * FROM dbo.Peliculas";

            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader =  command.ExecuteReader())
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
                                    //float.Parse((string)reader["Calificacion"], CultureInfo.InvariantCulture.NumberFormat),
                                    (string)reader["IDplatPeliculas"]
                                   
                                    //toImageSource((string)reader["Imagen"]
                                );
                                p.imagen = LoadImage(p.titulo,"movie");
                                contenido.Add(p);
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

        private void fetchSeries()
        {
            var query = "SELECT TOP 25 * FROM dbo.Series";

            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
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
                                    //float.Parse((string)reader["Calificacion"], CultureInfo.InvariantCulture.NumberFormat),   
                                    (string)reader["Calificacion"],
                                    Convert.ToUInt16(reader["Temporada"], CultureInfo.InvariantCulture.NumberFormat),
                                    Convert.ToUInt16(reader["Capitulos"], CultureInfo.InvariantCulture.NumberFormat),
                                    (string)reader["IDplatSeries"]
                                    //toImageSource((string)reader["Imagen"])
                                );
                                s.imagen = LoadImage(s.titulo,"tv");
                                contenido.Add(s);
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

        private  void fetchMoviesHeader()
        {
            var query = "SELECT TOP 100 Titulo, Año, Director, Imagen FROM dbo.Peliculas";

            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                     con.Open();
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    Pelicula p = new Pelicula(
                                        (string)reader["id"],
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
                                        (string)reader["IDplatPeliculas"],
                                        toImageSource((string)reader["Imagen"])
                                    );
                                    contenido.Add(p);
                                }
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

        private void fetchFavourites()
        {
            var query = "SELECT * FROM dbo.Favoritos WHERE userId = @userId";

            using (con = new SqlConnection(MainPage.constring))
            {
                try
                {
                    con.Open();

                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        command.Parameters.AddWithValue("@userId", MainPage.user.id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string peliculaId = reader["peliculaId"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("peliculaId")) : null;
                                    string serieId = reader["serieId"] != DBNull.Value ? reader.GetString(reader.GetOrdinal("serieId")) : null;

                                    if (!string.IsNullOrEmpty(peliculaId))
                                    {
                                        pelisFavoritas.Add(peliculaId);
                                    }
                                    else if (!string.IsNullOrEmpty(serieId))
                                    {
                                        seriesFavoritas.Add(serieId);
                                    }
                                }
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


        void goToDetail(object sender, EventArgs e)
        {
            Contenido c = (Contenido)((StackLayout)sender).BindingContext;
            Navigation.PushAsync(new Detail(c));
        }

        void busca(object sender, EventArgs e)
        {
            string textoBusqueda = searchBar.Text.ToLower().Trim();
            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                contenidoMostrar = contenido;
            }
            else
            {
                contenidoMostrar = new ObservableCollection<Contenido>(
                    contenido.Where(p => p.titulo.ToLower().Contains(textoBusqueda) || p.director.ToLower().Contains(textoBusqueda)
                || p.reparto.ToLower().Contains(textoBusqueda))); // || p.guionistas.ToLower().Contains(textoBusqueda)
            }
            listaContenidos.ItemsSource = contenidoMostrar;
        }

        void plataformaSeleccion(object sender, EventArgs e)
        {
            plataformaSeleccionada = (Plataforma)platPicker.SelectedItem;
            refrescaContenido();    
        }

        void etiquetaSeleccion(object sender, EventArgs e)
        {
            generoSeleccionado = (string)etqPicker.SelectedItem;
            refrescaContenido();
        }

        void mediaSeleccion(object sender, EventArgs e)
        {
            mediaSeleccionado = (string)mediaPicker.SelectedItem;
            refrescaContenido();
        }

        void criterioSeleccion(object sender, EventArgs e)
        {
            criterioSeleccionado = (string)orderPicker.SelectedItem;
            refrescaContenido();
        }
        
        void refrescaContenido()
        {
            // Partimos del contenido original.
            this.contenidoMostrar = this.contenido;
            // Filtramos por media:
             if (mediaSeleccionado == "Solo Películas")
                this.contenidoMostrar = new ObservableCollection<Contenido>(this.contenidoMostrar.Where(c => c.GetType() == typeof(Pelicula)));
            else if (mediaSeleccionado == "Solo Series")
                this.contenidoMostrar = new ObservableCollection<Contenido>(this.contenidoMostrar.Where(c => c.GetType() == typeof(Serie)));
            // Filtramos por plataforma:
            this.contenidoMostrar = new ObservableCollection<Contenido>(
                this.contenidoMostrar.Where(c => c.idPlataforma.Contains(plataformaSeleccionada.idPelicula) || c.idPlataforma.Contains(plataformaSeleccionada.idSerie)));
            // Filtramos por etiqueta:
            if (generoSeleccionado == "Todos los géneros")
                generoSeleccionado = "";
            this.contenidoMostrar = new ObservableCollection<Contenido>(
                this.contenidoMostrar.Where(p => p.etiquetas.Contains(generoSeleccionado)));
            // Ordenamos:
            if (criterioSeleccionado == "Mejor Valoración")
                contenidoMostrar = new ObservableCollection<Contenido>(contenidoMostrar.OrderByDescending(x => x.calificacion));
            else if (criterioSeleccionado == "Más reciente")
                contenidoMostrar = new ObservableCollection<Contenido>(contenidoMostrar.OrderByDescending(x => x.anio));
            else if (criterioSeleccionado == "Más antiguo")
                contenidoMostrar = new ObservableCollection<Contenido>(contenidoMostrar.OrderBy(x => x.anio));
            // Refrescamos la lista de contenidos
            listaContenidos.ItemsSource = contenidoMostrar;
        }
        public static ImageSource toImageSource(string url)
        {
            if (! url.StartsWith("https://"))
            {
                url = "http://" + url;
            }
            return ImageSource.FromUri(new Uri(url));
        }

        public static string LoadImage(string titulo, string media)
        {
            var options = new RestClientOptions($"https://api.themoviedb.org/3/search/{media}?query={titulo.Replace(" ", "+")}&include_adult=false&language=en-US&page=1");
            var client = new RestClient(options);
            var request = new RestRequest("");
            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {App.tokenTMDB}");
            var response = client.Get(request);

            string pattern = @"""poster_path"":\s*""([^""]+)""";

            Match match = Regex.Match(response.Content.ToString(), pattern);

            if (match.Success)
            {
                string posterPath = match.Groups[1].Value;

                return $"https://image.tmdb.org/t/p/w185{posterPath}";
            }
            else
            {
                Console.WriteLine("No poster_path found in the JSON string.");
                return null;
            }
        }
        
    }
}