﻿using System.ComponentModel.DataAnnotations;

namespace Proyecto_TiendaElectronica.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

    }
}
