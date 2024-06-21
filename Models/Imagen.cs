using System.ComponentModel.DataAnnotations;

namespace Proyecto_TiendaElectronica.Models
{
    public class Imagen
    {
        [Key]
        public int CodigoImagen { get; set; }
        public byte[] Imagen1 { get; set; }
        public byte [] Imagen2 { get; set; }
        public byte[] Imagen3 { get; set; }
    }
}
