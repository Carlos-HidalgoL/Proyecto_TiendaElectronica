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

        [HttpPost]
        public IActionResult GuardarCarrito([FromBody] List<Articulo> carrito)
        {
            if (carrito == null || carrito.Count == 0)
            {
                return BadRequest("El carrito está vacío.");
            }

            try
            {
                // Crear una nueva instancia de Factura
                var nuevaFactura = new Factura
                {
                    FechaCrecion = DateTime.Now,
                    UltimaFechaImpresion = DateTime.Now,
                    SubTotal = calcularSubTotal(carrito),
                    MontoTotal = calcularMontoTotal(carrito),
                    IdUsuario = "1" // Aquí deberías tener la lógica para obtener el Id del usuario actual
                };

                // Guardar la nueva factura en la base de datos
                _context.Factura.Add(nuevaFactura);
                _context.SaveChanges();

                // Guardar cada artículo de la factura en la tabla ArticuloFactura
                foreach (var articulo in carrito)
                {
                    var articuloFactura = new ArticuloFactura
                    {
                        idFactura = nuevaFactura.FacturaId,
                        idArticulo = articulo.ArticuloId,
                        CantidadArticulo = articulo.Cantidad
                    };

                    _context.ArticuloFactura.Add(articuloFactura);
                }

                _context.SaveChanges();

                return Ok("Factura creada correctamente.");
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción y devolver un error
                return StatusCode(500, $"Error al guardar la factura: {ex.Message}");
            }
        }

        private decimal calcularSubTotal(List<Articulo> carrito)
        {
            decimal subtotal = 0;
            foreach (var articulo in carrito)
            {
                subtotal += articulo.Precio * articulo.Cantidad;
            }
            return subtotal;
        }

        private decimal calcularMontoTotal(List<Articulo> carrito)
        {
            decimal subtotal = calcularSubTotal(carrito);
            decimal iva = subtotal * 0.13m;
            return subtotal + iva;
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
