using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_TiendaElectronica.Models;

namespace Proyecto_TiendaElectronica.Controllers
{
    public class PruebaController : Controller
    {
        private readonly AppDBContext _context;

        public PruebaController(AppDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            
            var usuarios = _context.Usuario.ToList();
            Console.Write(usuarios);
            
            return View();
        }

        
    }
}
