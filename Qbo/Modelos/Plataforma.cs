using System.ComponentModel;

namespace Qbo
{
    public class Plataforma
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public float precio { get; set; }
        public string idPelicula { get; set; }
        public string idSerie { get; set; }
        public string siteUrl { get; set; }
        public Plataforma(int id, string nombre, float precio, string idPelicula, string idSerie, string siteUrl)
        {
            this.id = id;
            this.nombre = nombre;
            this.precio = precio;
            this.idPelicula = idPelicula;
            this.idSerie = idSerie;
            this.siteUrl = siteUrl;
        }

        public override string ToString()
        {
            return $"{nombre} {precio}";
        }
    }
}
