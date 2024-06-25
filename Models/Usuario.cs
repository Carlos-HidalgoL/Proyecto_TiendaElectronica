using System.ComponentModel.DataAnnotations;

namespace Proyecto_TiendaElectronica.Models
{
    public class Usuario
    {
        [Key]
        public string UsuarioId { get; set; }

        public string Nombre { get; set; }
        
        public string Correo { get; set; }
        
        public string Contrasena { get; set; }

        public bool Estado { get; set; }

        public string Telefono { get; set; }



    }
}
