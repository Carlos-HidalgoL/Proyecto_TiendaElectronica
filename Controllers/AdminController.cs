using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_TiendaElectronica.Models;

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
            return View();
        }

        public async Task<IActionResult> Articulos() {

            var articulos = await _context.Articulo.Include("Categoria").Include("Imagen").ToListAsync();
            

            return View(articulos);

        }



        //Controllers de Usuario

        [HttpGet]
        public async Task<IActionResult> Usuarios() {
            try {
                var usuarios = await _context.Usuario.ToListAsync();

                return View(usuarios);

            }catch (Exception) {
                return NotFound();
            }
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
        public async Task<IActionResult> EditarUsuario(Usuario usuario)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Usuario.Update(usuario);
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
