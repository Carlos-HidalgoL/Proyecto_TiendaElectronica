using System.ComponentModel.DataAnnotations;

namespace Proyecto_TiendaElectronica.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

    }
}
