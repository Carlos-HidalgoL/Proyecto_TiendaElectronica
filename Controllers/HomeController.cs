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


       
        public async Task<IActionResult> Index()
        {
            
            var articulos = await _context.Articulo.Include("Imagen").Include("Categoria").ToListAsync();
            
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

		public async Task<IActionResult> Producto(int id)
		{   
            

			var articulo = await _context.Articulo.Include("Imagen").Include("Categoria").FirstOrDefaultAsync( art => art.ArticuloId == id );

            var articulosSimilares = await _context.Articulo.Include("Categoria").Include("Imagen").Where( art => art.idCategoria == articulo.idCategoria && art.ArticuloId != articulo.ArticuloId).ToListAsync();
			
            ViewBag.Articulos = articulosSimilares;

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
        public async Task<IActionResult> GuardarCarrito([FromBody] List<Articulo> carrito)
        {
            if (carrito == null || carrito.Count == 0)
            {
                return BadRequest("El carrito está vacío.");
            }

            try
            {
                var usuario = await _context.Usuario.FirstOrDefaultAsync( u => u.UsuarioId == "123456789");
                var nuevaFactura = new Factura
                {
                    FechaCreacion = DateTime.Now,
                    SubTotal = calcularSubTotal(carrito),
                    MontoTotal = calcularMontoTotal(carrito),
                    UsuarioId = "123456789"
                };

                nuevaFactura.Usuario = usuario;

                _context.Factura.Add(nuevaFactura);
                //Se guarda aquí y no al final por que se necesita el id que se genera
                await _context.SaveChangesAsync();

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

                await _context.SaveChangesAsync();

                return Ok("Ok");
            }
            catch (Exception ex)
            {
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
