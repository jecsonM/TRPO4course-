using MachineServices.Interfaces;
using MachineServices.Models;
using MachineServices.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MachineServices.Controllers
{
    public class AccountController : Controller
    {
        private readonly MachineServicesDbContext _context;
        private readonly IPasswordService _passwordService;

        public AccountController(MachineServicesDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Если уже авторизован, перенаправляем в профиль
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Profile");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var client = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Email == model.Email);

                if (client != null && client.PasswordHash != null)
                {
                    if (_passwordService.ValidatePassword(model.Password, client.PasswordHash))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, client.ClientId.ToString()),
                            new Claim(ClaimTypes.Email, client.Email),
                            new Claim(ClaimTypes.Name, client.CompanyName),
                            new Claim("ContactPerson", client.ContactPersonFullname)
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                            new AuthenticationProperties { IsPersistent = model.RememberMe });

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction("Profile");
                    }
                }

                ModelState.AddModelError("", "Неверный email или пароль");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Если уже авторизован, перенаправляем в профиль
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Profile");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Проверка на существующего пользователя по email
                var existingClientByEmail = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Email == model.Email);

                if (existingClientByEmail != null)
                {
                    ModelState.AddModelError("Email", "Пользователь с таким email уже существует");
                    return View(model);
                }

                // Проверка на существующего пользователя по ИНН
                var existingClientByInn = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Inn == model.Inn);

                if (existingClientByInn != null)
                {
                    ModelState.AddModelError("Inn", "Компания с таким ИНН уже зарегистрирована");
                    return View(model);
                }

                // Создание нового клиента
                var newClient = new Client
                {
                    CompanyName = model.CompanyName,
                    ContactPersonFullname = model.ContactPersonFullname,
                    ContactPhone = model.ContactPhone,
                    Email = model.Email,
                    MainAddress = model.MainAddress,
                    Inn = model.Inn,
                    Kpp = model.Kpp,
                    Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes,
                    PasswordHash = _passwordService.HashPassword(model.Password)
                };

                _context.Clients.Add(newClient);
                await _context.SaveChangesAsync();

                // Автоматический вход после регистрации
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, newClient.ClientId.ToString()),
                    new Claim(ClaimTypes.Email, newClient.Email),
                    new Claim(ClaimTypes.Name, newClient.CompanyName),
                    new Claim("ContactPerson", newClient.ContactPersonFullname)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Profile");
            }

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int clientId))
            {
                return RedirectToAction("Login");
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == clientId);

            if (client == null)
            {
                return RedirectToAction("Login");
            }

            // Получаем все заявки клиента
            var requests = await _context.Requests
                .Where(r => r.ClientId == clientId)
                .Include(r => r.Relevantrequeststates)
                    .ThenInclude(rrs => rrs.RequestState)
                .Include(r => r.Orders)
                    .ThenInclude(o => o.Serviceprovisions)
                        .ThenInclude(sp => sp.Service)
                .OrderByDescending(r => r.CreationDate)
                .ToListAsync();

            var requestViewModels = new List<RequestViewModel>();

            foreach (var request in requests)
            {
                // Находим самое раннее состояние (по дате)
                var earliestState = request.Relevantrequeststates
                    .OrderBy(rrs => rrs.SetDate)
                    .FirstOrDefault();

                // Находим последнее состояние (по дате)
                var latestState = request.Relevantrequeststates
                    .OrderByDescending(rrs => rrs.SetDate)
                    .FirstOrDefault();

                var orderViewModels = new List<OrderViewModel>();

                foreach (var order in request.Orders)
                {
                    var serviceItems = order.Serviceprovisions
                        .Select(sp => new ServiceItemViewModel
                        {
                            ServiceName = sp.Service.MachineServiceName,
                            Amount = sp.Amount
                        })
                        .ToList();

                    orderViewModels.Add(new OrderViewModel
                    {
                        OrderId = order.OrderId,
                        RequestId = order.RequestId,
                        ServiceAddress = request.ServiceAddress,
                        CreationDate = order.CreationDate,
                        Services = serviceItems
                    });
                }

                requestViewModels.Add(new RequestViewModel
                {
                    RequestId = request.RequestId,
                    CreationDate = earliestState?.SetDate ?? request.CreationDate,
                    ServiceAddress = request.ServiceAddress,
                    LastStateDate = latestState?.SetDate,
                    LastStateName = latestState?.RequestState?.RequestStateName,
                    Orders = orderViewModels
                });
            }

            var profileViewModel = new ProfileViewModel
            {
                Client = client,
                Requests = requestViewModels
            };

            // Передаем сообщения через ViewBag или TempData
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }
            if (TempData["ErrorMessage"] != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
            }

            return View(profileViewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest([FromForm] CreateRequestViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ServiceAddress))
            {
                TempData["ErrorMessage"] = "Пожалуйста, укажите адрес обслуживания";
                return RedirectToAction("Profile");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int clientId))
            {
                return RedirectToAction("Login");
            }

            // Находим статус "Создана"
            var createdState = await _context.Requeststates
                .FirstOrDefaultAsync(rs => rs.RequestStateName == "Создана");

            if (createdState == null)
            {
                createdState = new Requeststate { RequestStateName = "Создана" };
                _context.Requeststates.Add(createdState);
                await _context.SaveChangesAsync();
            }

            // Создаем заявку
            var newRequest = new Request
            {
                ClientId = clientId,
                CreationDate = DateTime.UtcNow,
                ServiceAddress = model.ServiceAddress.Trim(),
                MasterId = null
            };

            _context.Requests.Add(newRequest);
            await _context.SaveChangesAsync();

            // Добавляем состояние
            var relevantRequestState = new Relevantrequeststate
            {
                RequestId = newRequest.RequestId,
                RequestStateId = createdState.RequestStateId,
                SetDate = DateTime.UtcNow
            };

            _context.Relevantrequeststates.Add(relevantRequestState);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заявка успешно создана!";
            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}