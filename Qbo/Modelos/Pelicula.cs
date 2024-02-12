using System;
using Xamarin.Forms;

namespace Qbo
{
    public class Pelicula : Contenido
    {
        public string guionistas { get; set; }
        public Pelicula(string id, string titulo, string pais, ushort anio, TimeSpan duracion, string sinopsis, string director,
            string guionistas, string reparto, string etiquetas, string calificacion, string idPlataforma, 
            ImageSource imagen = null) : base(id, titulo, pais, anio, duracion, sinopsis, director, reparto, etiquetas, calificacion, idPlataforma, imagen)
        {
             this.guionistas = guionistas;
        }
    }
}
