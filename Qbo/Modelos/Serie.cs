using System;
using Xamarin.Forms;

namespace Qbo
{
    public class Serie : Contenido
    {
        public ushort temporadas { get; set; }
        public uint capitulos { get; set; }

        public Serie(string id, string titulo, string pais, ushort anio, TimeSpan duracion, string sinopsis, string director,
            string reparto, string etiquetas, string calificacion, ushort temporadas, uint capitulos,
            string idPlataforma, ImageSource imagen = null) : base(id, titulo, pais, anio, duracion, sinopsis, director, reparto, etiquetas, calificacion, idPlataforma, imagen)

        {
            this.temporadas = temporadas;
            this.capitulos = capitulos;
        }
    }
}
