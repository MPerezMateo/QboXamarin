using System;
using Xamarin.Forms;

namespace Qbo
{
    public class Contenido
    {
        public string id { get; set; }
        public string titulo { get; set; }
        public string pais { get; set; }
        public ushort anio { get; set; }
        public TimeSpan duracion { get; set; }
        public string sinopsis { get; set; }
        public string director { get; set; }
        public string reparto { get; set; }
        public string etiquetas { get; set; }
        public string calificacion { get; set; }
        public string idPlataforma { get; set; }
        public ImageSource imagen { get; set; }

        public Contenido(string id,string titulo, string pais, ushort anio, TimeSpan duracion, string sinopsis, string director,
            string reparto, string etiquetas, string calificacion, string idPlataforma, ImageSource imagen = null) {
            this.id = id;
            this.titulo = titulo;
            this.pais = pais;
            this.anio = anio;
            this.duracion = duracion;
            this.sinopsis = sinopsis;
            this.director = director;
            this.reparto = reparto;
            this.etiquetas = etiquetas;
            this.calificacion = calificacion;
            this.idPlataforma = idPlataforma;
            this.imagen = imagen;
        }

        public override string ToString()
        {
            return $"{titulo} {director}";
        }
    }
}
