using MachineServices.Interfaces;
using MachineServices.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;


namespace MachineServices.Controllers
{
    public class AccountController : Controller
    {

        private readonly MyDbContext _context;
        private readonly IPasswordService _passwordService;

        public AccountController(MyDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<IActionResult> Login(string login, string password)
        {
            
            Staff user = await _context.Staff
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user != null)
            {
                if (user.PasswordHash == null)
                {
                    user.PasswordHash = _passwordService.HashPassword(password);
                    _context.SaveChanges();
                }

                

                if (_passwordService.ValidatePassword(password, user.PasswordHash))

                {
                    var claims = new List<Claim>
                    {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim("UserID", user.StaffId.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.Error = "Неверный логин или пароль";
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
