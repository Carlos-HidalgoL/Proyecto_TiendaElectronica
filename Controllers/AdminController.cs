using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_TiendaElectronica.Models;
using Proyecto_TiendaElectronica.ModelBinder;
using Microsoft.AspNetCore.Identity;
using Proyecto_TiendaElectronica.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_TiendaElectronica.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public AdminController(AppDBContext context, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager) { 
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

		[Authorize(Roles = "Administrador")]
		public ActionResult Index()
		{
			ViewBag.Pagina = "Index";
			return View();
		}

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Articulos()
		{

			var articulos = await _context.Articulo.Include("Categoria").Include("Imagen").Take(10).ToListAsync();
			ViewBag.Pagina = "Articulos";

			return View(articulos);

		}

        [Authorize(Roles = "Administrador")]
        public ActionResult CrearArticulo()
		{

			var categorias = _context.Categoria.ToList();

			ViewBag.Categorias = new SelectList(categorias, "CategoriaId", "Nombre");

			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CrearArticulo(Articulo articulo)
		{

			Imagen imagen = new Imagen
			{
				Imagen1 = await FormFileToByteArrayAsync(articulo.Imagen1),
				Imagen2 = await FormFileToByteArrayAsync(articulo.Imagen2),
				Imagen3 = await FormFileToByteArrayAsync(articulo.Imagen3)
			};

			_context.Imagen.Add(imagen);
			await _context.SaveChangesAsync();


			Articulo articuloNuevo = new Articulo()
			{
				Nombre = articulo.Nombre,
				Precio = articulo.Precio,
				Descripcion = articulo.Descripcion,
				Marca = articulo.Marca,
				Cantidad = articulo.Cantidad,
				codigoImagen = imagen.ImagenId,
				idCategoria = articulo.idCategoria
			};

			_context.Articulo.Add(articuloNuevo);
			await _context.SaveChangesAsync();

			return RedirectToAction("Articulos");


			return View(articulo);
		}

		private async Task<byte[]> FormFileToByteArrayAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)
				return null;

			using (var memoryStream = new MemoryStream())
			{
				await file.CopyToAsync(memoryStream);
				return memoryStream.ToArray();
			}
		}

		[HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> VerArticulo(int id)
		{
			if (id == null)
			{
				return NotFound();
			}
			try
			{
				var articulo = await _context.Articulo.FindAsync(id);
				if (articulo == null)
				{
					return NotFound();
				}

				var imagen = await _context.Imagen.FindAsync(articulo.codigoImagen);
				var categoria = await _context.Categoria.FindAsync(articulo.idCategoria);

				ViewBag.Imagen1 = imagen.Imagen1;
				ViewBag.Imagen2 = imagen.Imagen2;
				ViewBag.Imagen3 = imagen.Imagen3;

				ViewBag.Categoria = categoria.Nombre.ToString();


				return View(articulo);
			}
			catch (Exception)
			{
				return RedirectToAction(nameof(Articulos));
			}
		}

		[HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditarArticulo(int id)
		{
			var articulo = await _context.Articulo.FindAsync(id);
			if (articulo == null)
			{
				return NotFound();
			}



			var categorias = _context.Categoria.ToList();
			ViewBag.Categorias = new SelectList(categorias, "CategoriaId", "Nombre");

			return View(articulo);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditarArticulo(Articulo articulo)
		{
			Articulo articuloDB = new Articulo
			{
				ArticuloId = articulo.ArticuloId,
				Nombre = articulo.Nombre,
				Descripcion = articulo.Descripcion,
				Marca = articulo.Marca,
				Precio = articulo.Precio,
				Cantidad = articulo.Cantidad,
				idCategoria = articulo.idCategoria,
				codigoImagen = articulo.codigoImagen,
			};



			try
			{
				_context.Articulo.Update(articuloDB);
				await _context.SaveChangesAsync();


				return RedirectToAction(nameof(Articulos));
			}

			catch (Exception)
			{
				return View(articuloDB);
			}

		}
		[HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ModificaImagen(int codigoImagen, int ArticuloID)
		{
			var Imagen = await _context.Imagen.FindAsync(codigoImagen);

			if (Imagen == null)
			{
				return NotFound();
			}
			ViewBag.ArticuloId = ArticuloID;
			return View(Imagen);
		}

		[HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ModificaImagen(Imagen imagen, IFormFile Imagen1, IFormFile Imagen2, IFormFile Imagen3)
		{

			var Imagen = await _context.Imagen.FindAsync(imagen.ImagenId);
			if (Imagen1 != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await Imagen1.CopyToAsync(memoryStream);
					Imagen.Imagen1 = memoryStream.ToArray();
				}
			}

			if (Imagen2 != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await Imagen2.CopyToAsync(memoryStream);
					Imagen.Imagen2 = memoryStream.ToArray();
				}
			}

			if (Imagen3 != null)
			{
				using (var memoryStream = new MemoryStream())
				{
					await Imagen3.CopyToAsync(memoryStream);
					Imagen.Imagen3 = memoryStream.ToArray();
				}
			}

			_context.Update(Imagen);
			await _context.SaveChangesAsync();
			return RedirectToAction("Articulos");

		}

		[HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EliminarArticulo(int id)
		{
			if (id == null)
			{
				return Json(new { success = false, message = "ID de Articulo no proporcionado." });
			}

			try
			{
				var articulo = await _context.Articulo.FindAsync(id);

				if (articulo == null)
				{
					return Json(new { success = false, message = "articulo no encontrado." });
				}
				_context.Articulo.Remove(articulo);

				_context.SaveChanges();

				return Json(new { success = true, message = "Articulo eliminado con éxito." });
			}
			catch (Exception)
			{
				return Json(new { success = false, message = "Ocurrió un error al eliminar el articulo." });
			}



		}


		[HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Usuarios()
		{

			var usuarios = await _context.Usuario.ToListAsync();
			var datos = new List<UserViewModel>();

			foreach (var usuario in usuarios) {
				var roles = await _userManager.GetRolesAsync(usuario);

				var rol = roles.FirstOrDefault();

				var user = Conversion(usuario, rol);

				datos.Add(user);
			}

			ViewBag.Pagina = "Usuarios";

			return View(datos);

		}


		[HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult CrearUsuario()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CrearUsuario(UserViewModel model)
		{

			if (ModelState.IsValid)
			{
				var usuario = new Usuario { Id = model.UsuarioId, UserName = model.Nombre, Email = model.Correo, PhoneNumber = model.Telefono };

				var result = await _userManager.CreateAsync(usuario, model.Contrasena);

				if (result.Succeeded)
				{

					await _userManager.AddToRoleAsync(usuario, model.Rol);

					return RedirectToAction("Usuarios");
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}

			}

			return View(model);


		}


		[HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> VerUsuario(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			try
			{
				var usuario = await _context.Usuario.FindAsync(id);

				if (usuario == null)
				{
					return NotFound();
				}

				var roles = await _userManager.GetRolesAsync(usuario);

				var rol = roles.FirstOrDefault();

				var user = Conversion(usuario, rol);

				return View(user);
			}
			catch (Exception)
			{
				return RedirectToAction(nameof(Usuarios));
			}


		}

		[HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EditarUsuario(string id)
		{
			if (id == null)
			{
				return NotFound();
			}


			try
			{
				var usuario = await _context.Usuario.FindAsync(id);

				if (usuario == null)
				{
					return NotFound();
				}

				var roles = await _userManager.GetRolesAsync(usuario);

				var rol = roles.FirstOrDefault();

				var user = Conversion(usuario, rol);

				return View(user);
			}
			catch (Exception)
			{
				return RedirectToAction(nameof(Usuarios));
			}


		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditarUsuario([ModelBinder(BinderType = typeof(ModelBinder.UsuarioModelBinder))] UserViewModel model)
		{

			if (ModelState.IsValid)
			{
				var usuario = await _userManager.FindByIdAsync(model.UsuarioId);

				if (usuario == null) {
					return NotFound();
				}

				usuario.UserName = model.Nombre;
				usuario.Email = model.Correo;
				usuario.PhoneNumber = model.Telefono;

				var result = await _userManager.UpdateAsync(usuario);

				if (result.Succeeded)
				{
					var rolActual = await _userManager.GetRolesAsync(usuario);

					await _userManager.RemoveFromRolesAsync(usuario, rolActual);

					await _userManager.AddToRoleAsync(usuario, model.Rol);

					return RedirectToAction("Usuarios");
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}

			}

			return View(model);

		}

		[HttpGet]
        [Authorize(Roles = "Administrador")]
        public async  Task<IActionResult> EditarContrasena(string id) {
			if (id == null) {
				return NotFound();
			}

			var usuario = await _userManager.FindByIdAsync(id);

			if (usuario == null) {
				return NotFound();
			}

			var user = Conversion(usuario);

			return View(user);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async  Task<IActionResult> EditarContrasena(UserViewModel model)
		{
			if (model.UsuarioId != null && model.Contrasena != null && model.ConfirmarContrasena != null) {
				var usuario = await _userManager.FindByIdAsync(model.UsuarioId);

				if (usuario == null) {
					return NotFound();
				}

                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var result = await _userManager.ResetPasswordAsync(usuario, token, model.Contrasena);

                if (result.Succeeded)
                {
					return RedirectToAction("EditarUsuario", new { id = model.UsuarioId });
					
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

			return View(model);


			
		}


		public async Task<IActionResult> EliminarUsuario(string id)
		{
			if (id == null)
			{
				return Json(new { success = false, message = "ID de usuario no proporcionado." });
			}

			var usuario = await _userManager.FindByIdAsync(id);

			if (usuario == null)
			{
				return Json(new { success = false, message = "Usuario no encontrado." });
			}
			var result = await _userManager.DeleteAsync(usuario);

			if (result.Succeeded)
			{
				return Json(new { success = true, message = "Usuario eliminado con éxito." });
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
			
			return Json(new { success = false, message = "Ocurrió un error al eliminar el usuario." });
			



		}
















		

		[HttpGet]
		public async Task<IActionResult> ObtenerArticulos()
		{

			var articulos = await _context.Articulo.ToListAsync();
			var imagenes = await _context.Imagen.ToListAsync();
			var categorias = await _context.Categoria.ToListAsync();

			foreach (var articulo in articulos)
			{
				articulo.Imagen = imagenes.FirstOrDefault(a => a.ImagenId == articulo.codigoImagen);
				articulo.Categoria = categorias.FirstOrDefault(a => a.CategoriaId == articulo.idCategoria);
			}


			return Json(articulos);

		}

		private UserViewModel Conversion(Usuario usuario, string rol = "") {
			var usuarioConvertido = new UserViewModel
            {
				UsuarioId = usuario.Id,
				Nombre = usuario.UserName,
				Correo = usuario.Email,
				Rol = rol,
				Telefono = usuario.PhoneNumber,
			};

			return usuarioConvertido;
		}
	}
}
