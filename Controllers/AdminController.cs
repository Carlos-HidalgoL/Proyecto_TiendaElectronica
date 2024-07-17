using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_TiendaElectronica.Models;
using Proyecto_TiendaElectronica.wwwroot.funtions;
using Proyecto_TiendaElectronica.ModelBinder;

namespace Proyecto_TiendaElectronica.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDBContext _context;

        public AdminController(AppDBContext context) { 
            _context = context;
        }

        // GET: AdminController
        public ActionResult Index()
        {
            ViewBag.Pagina = "Index";
            return View();
        }

        public async Task<IActionResult> Articulos() {

            var articulos = await _context.Articulo.Include("Categoria").Include("Imagen").ToListAsync();
            ViewBag.Pagina = "Articulos";

            return View(articulos);

        }

		//Controller Articulo
		public ActionResult CrearArticulo()
		{

			var categorias = _context.Categoria.ToList();

			ViewBag.Categorias = new SelectList(categorias, "CategoriaId", "Nombre");

			return View();
		}

		[HttpPost]
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

			return RedirectToAction("Index");


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

		//Controllers de Usuario

		[HttpGet]
        public async Task<IActionResult> Usuarios() {

            var usuarios = await _context.Usuario.ToListAsync();

            ViewBag.Pagina = "Usuarios";

            return View(usuarios);

            
        }


        [HttpGet]
        public IActionResult CrearUsuario() { 

            var usuario = new Usuario();

            usuario.Estado = true;

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario(Usuario usuario) {

            if (ModelState.IsValid) {
                try
                {
                    usuario.Contrasena = Hash.HashPassword(usuario.Contrasena);

                    _context.Usuario.Add(usuario);
                    await _context.SaveChangesAsync();


                    return RedirectToAction(nameof(Usuarios));
                }
                catch (DbUpdateException) {
                    ViewBag.Error = "El usuario con la cédula "+ usuario.UsuarioId + " ya existe.";
                    return View(usuario);
                }
                catch (Exception)
                {
                    return View(usuario);
                }
            }

            return View(usuario);

        }


        [HttpGet]
        public async Task<IActionResult> VerUsuario(string id)
        {
            if (id == null) {
                return NotFound();
            }


            try
            {
                var usuario = await _context.Usuario.FindAsync(id);

                if (usuario == null)
                {
                    return NotFound();
                }

                return View(usuario);
            }
            catch (Exception) { 
                return RedirectToAction(nameof(Usuarios));
            }


        }

        [HttpGet]
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

                return View(usuario);
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Usuarios));
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario([ModelBinder(BinderType = typeof(ModelBinder.UsuarioModelBinder))]Usuario usuario)
        {

            if (ModelState.IsValid)
            {
                try
                {

                    var obtenido = await _context.Usuario.FirstOrDefaultAsync(u => u.UsuarioId == usuario.UsuarioId);

                    obtenido.Nombre = usuario.Nombre;
                    obtenido.Telefono = usuario.Telefono;
                    obtenido.Estado = usuario.Estado;
                    obtenido.Rol = usuario.Rol;
                    obtenido.Correo = usuario.Correo;
                    await _context.SaveChangesAsync();


                    return RedirectToAction(nameof(Usuarios));
                }
                catch (DbUpdateException)
                {
                    ViewBag.Error = "El usuario con la cédula " + usuario.UsuarioId + " ya existe.";
                    return View(usuario);
                }
                catch (Exception)
                {
                    return View(usuario);
                }
            }

            return View(usuario);

        }

        public async Task<IActionResult> EliminarUsuario(string id) {
            if (id == null) {
                return Json(new { success = false, message = "ID de usuario no proporcionado." });
            }

            try {
                var usuario = await _context.Usuario.FindAsync(id);

                if (usuario == null) {
                    return Json(new { success = false, message = "Usuario no encontrado." });
                }
                _context.Usuario.Remove(usuario);

                _context.SaveChanges();

                return Json(new { success = true, message = "Usuario eliminado con éxito." });
            } catch (Exception) {
                return Json(new { success = false, message = "Ocurrió un error al eliminar el usuario." });
            }

            

        }
















        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerArticulos() { 

            var articulos = await _context.Articulo.ToListAsync();
            var imagenes = await _context.Imagen.ToListAsync();
            var categorias = await _context.Categoria.ToListAsync();

            foreach (var articulo in articulos) {
                articulo.Imagen = imagenes.FirstOrDefault( a => a.ImagenId == articulo.codigoImagen );
                articulo.Categoria = categorias.FirstOrDefault(a => a.CategoriaId == articulo.idCategoria);
            }
            

            return Json(articulos);

        }
    }
}
