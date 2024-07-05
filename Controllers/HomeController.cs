using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_TiendaElectronica.Models;
using System.Diagnostics;

namespace Proyecto_TiendaElectronica.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDBContext _context;

        public HomeController(ILogger<HomeController> logger, AppDBContext context)
        {
            _context = context;
            _logger = logger;
        }


       
        public IActionResult Index()
        {
            
            var articulos = _context.Articulo.ToList();
            var imagenes = _context.Imagen.ToList();

            foreach (var articulo in articulos)
            {
                articulo.Imagen = imagenes.FirstOrDefault(i => i.ImagenId == articulo.codigoImagen);
            }


            return View(articulos);
        }
		public IActionResult Tienda()
		{

			var articulos = _context.Articulo.ToList();
			var imagenes = _context.Imagen.ToList();

			foreach (var articulo in articulos)
			{
				articulo.Imagen = imagenes.FirstOrDefault(i => i.ImagenId == articulo.codigoImagen);
			}


			return View(articulos);
		}

		public IActionResult Producto(int id)
		{
			var articulo = _context.Articulo.FirstOrDefault(a => a.ArticuloId == id);
			if (articulo == null)
			{
				return NotFound();
			}

			var imagen = _context.Imagen.FirstOrDefault(i => i.ImagenId == articulo.codigoImagen);
			articulo.Imagen = imagen;

			return View(articulo);
		}


        public IActionResult SobreNosotros()
        {
            return View();

        }
        public IActionResult Carrito()
		{
			return View();

		}


       
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
