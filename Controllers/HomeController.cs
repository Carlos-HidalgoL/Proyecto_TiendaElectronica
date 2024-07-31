using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_TiendaElectronica.Models;
using Proyecto_TiendaElectronica.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_TiendaElectronica.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDBContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public HomeController(ILogger<HomeController> logger, AppDBContext context, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
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
                var usuario = await _context.Usuario.FirstOrDefaultAsync( u => u.Id == "123456789");
                var nuevaFactura = new Factura
                {
                    FechaCreacion = DateTime.Now,
                    SubTotal = calcularSubTotal(carrito),
                    MontoTotal = calcularMontoTotal(carrito),
                    UsuarioId = "123456789"
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


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Perfil() {
            if (!User.Identity.IsAuthenticated) { 
                return NotFound();
            }

            var usuario = await _userManager.FindByNameAsync(User.Identity.Name);

            if (usuario == null) {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            var rol = roles.FirstOrDefault();

            var user = Conversion(usuario, rol);


            return View(user);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ActualizarPerfil()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NotFound();
            }

            var usuario = await _userManager.FindByNameAsync(User.Identity.Name);

            if (usuario == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(usuario);

            var rol = roles.FirstOrDefault();

            var user = Conversion(usuario, rol, false);


            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarPerfil(UpdateUserPerfil model)
        {
            if (ModelState.IsValid) {
                var usuario = await _userManager.FindByIdAsync(model.UsuarioId);

                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.Email = model.Correo;
                usuario.PhoneNumber = model.Telefono;

                var result = await _userManager.UpdateAsync(usuario);

                if (result.Succeeded)
                {
                    return RedirectToAction("Perfil");
                }


                TempData["SweetAlertScript"] = "<script>Swal.fire({\r\n  title: \"Error\",\r\n  text: \"No se pudo editar la información.\",\r\n  icon: \"error\"\r\n, confirmButtonColor: \"#E14848\"});;</script>";
                
            }


            return View(model);
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

        public Object Conversion(Usuario usuario, string rol = "", bool proceso = true)
        {
            object usuarioConvertido;
            if (proceso)
            {
                usuarioConvertido = new UserViewModel
                {
                    UsuarioId = usuario.Id,
                    Nombre = usuario.UserName,
                    Correo = usuario.Email,
                    Rol = rol,
                    Telefono = usuario.PhoneNumber,
                };
                
            }
            else {
                usuarioConvertido = new UpdateUserPerfil
                {
                    UsuarioId = usuario.Id,
                    Correo = usuario.Email,
                    Telefono = usuario.PhoneNumber,
                };

            }

            return usuarioConvertido;



        }
    }
}
