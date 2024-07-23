using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_TiendaElectronica.ViewModels;
using Proyecto_TiendaElectronica.Models;
using Microsoft.AspNetCore.Identity;

namespace Proyecto_TiendaElectronica.Controllers
{
    public class LogueoController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public LogueoController(AppDBContext context, UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index() { 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserLogin model) {
            if (ModelState.IsValid) {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false,lockoutOnFailure: false);

                if (result.Succeeded) {
                    var usuario = await _userManager.FindByNameAsync(model.UserName);

                    var roles = await _userManager.GetRolesAsync(usuario);

                    var rol = roles.FirstOrDefault();

                    if (rol == "Administrador")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else {
                        return RedirectToAction("Index", "Home");
                    }


                }
            }

            return View(model);
        }
    }
}
