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

            var articulos = await _context.Articulo.ToListAsync();
            var imagenes = await _context.Imagen.ToListAsync();
            var categorias = await _context.Categoria.ToListAsync();

            foreach (var articulo in articulos)
            {
                articulo.Imagen = imagenes.FirstOrDefault(a => a.ImagenId == articulo.codigoImagen);
                articulo.Categoria = categorias.FirstOrDefault(a => a.CategoriaId == articulo.idCategoria);
            }

            return View(articulos);

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
