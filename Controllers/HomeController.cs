using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_TiendaElectronica.Models;
using System.Diagnostics;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Proyecto_TiendaElectronica.ViewModels;
using System.Security.Claims;




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
            QuestPDF.Settings.License = LicenseType.Community;
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

                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (usuarioId == null)
                {
                    return Unauthorized("Usuario no autenticado.");
                }

                var usuario = await _context.Usuario.FirstOrDefaultAsync( u => u.Id == usuarioId);
                var nuevaFactura = new Factura
                {
                    FechaCreacion = DateTime.Now,
                    SubTotal = calcularSubTotal(carrito),
                    MontoTotal = calcularMontoTotal(carrito),
                    UsuarioId = usuario.Id
                };

                //nuevaFactura.Usuario = usuario;

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

                return Json(new { success = true, facturaId = nuevaFactura.FacturaId });

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

        [HttpGet]
        public async Task<IActionResult> DescargarPDF(int facturaId)
        {
            // Recupera la factura incluyendo los detalles de los artículos comprados
            var factura = await _context.Factura
                .Include(f => f.articulosFactura)
                .ThenInclude(af => af.Articulo)
                .FirstOrDefaultAsync(f => f.FacturaId == facturaId);

            if (factura == null)
            {
                return NotFound("Factura no encontrada.");
            }

            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Id == factura.UsuarioId);

            // Verifica si la factura fue encontrada
           

            // Crea el documento PDF usando QuestPDF
            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    // Cabecera de la página
                    page.Header().Row(row =>
                    {

                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Factura").Bold().FontSize(22).FontColor("#333333");
                            col.Item().Text($"Número de Factura: {factura.FacturaId}").FontSize(14).FontColor("#555555");
                            col.Item().Text($"Fecha: {factura.FechaCreacion.ToShortDateString()}").FontSize(12).FontColor("#777777");
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"Cliente: {usuario.Id}").FontSize(14).FontColor("#000000");
                            col.Item().Text($"Nombre: {usuario.UserName}").FontSize(14).FontColor("#000000");
                            col.Item().Text($"Número: {usuario.PhoneNumber}").FontSize(14).FontColor("#000000");
                            col.Item().Text($"Email: {usuario.Email}").FontSize(14).FontColor("#000000");

                        });
                    });

                    // Contenido de la factura
                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Item().Text("Detalles de la Factura").Bold().FontSize(16);

                        col.Item().Table(table =>
                        {
                            // Definición de columnas
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);  // Nombre del producto
                                columns.RelativeColumn();   // Precio unitario
                                columns.RelativeColumn();   // Cantidad
                                columns.RelativeColumn();   // SubTotal
                            });
                            
                            // Encabezado de la tabla
                            table.Header(header =>
                            {
                                header.Cell().Background("#E14848").Padding(5).Text("Producto").Bold().FontColor("#ffffff");
                                header.Cell().Background("#E14848").Padding(5).Text("Precio Unitario").Bold().FontColor("#ffffff");
                                header.Cell().Background("#E14848").Padding(5).Text("Cantidad").Bold().FontColor("#ffffff");
                                header.Cell().Background("#E14848").Padding(5).Text("SubTotal").Bold().FontColor("#ffffff");
                            });

                            // Agrega las filas de productos
                            foreach (var articuloFactura in factura.articulosFactura)
                            {
                                var articulo = articuloFactura.Articulo;
                                var cantidad = articuloFactura.CantidadArticulo;
                                var precio = articulo.Precio;
                                var total = cantidad * precio;

                                table.Cell().Text(articulo.Nombre);
                                table.Cell().Text($"₡ {precio}");
                                table.Cell().Text(cantidad.ToString());
                                table.Cell().Text($"₡ {total}");
                            }
                        });

                        // Monto total de la factura
                        col.Item().PaddingTop(20)
                      .AlignRight().Text($"Total Iva: ₡ {factura.MontoTotal:N2}").Bold().FontSize(16);
                    });

                    // Pie de página con el número de página
                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Text("Gracias por su compra").FontSize(12).FontColor("#777777");
                        row.RelativeItem().AlignRight().Text(txt =>
                        {
                            txt.Span("Página ").FontSize(10).FontColor("#777777");
                            txt.CurrentPageNumber().FontSize(10).FontColor("#777777");
                            txt.Span(" de ").FontSize(10).FontColor("#777777");
                            txt.TotalPages().FontSize(10).FontColor("#777777");
                        });
                    });
                });
            }).GeneratePdf();

            // Devuelve el archivo PDF generado
            using (var stream = new MemoryStream(pdf))
            {
                return File(stream.ToArray(), "application/pdf", "factura.pdf");
            }
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
