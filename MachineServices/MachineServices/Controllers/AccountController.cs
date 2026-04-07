using MachineServices.Interfaces;
using MachineServices.Models;
using MachineServices.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

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

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DownloadReport()
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
                var earliestState = request.Relevantrequeststates
                    .OrderBy(rrs => rrs.SetDate)
                    .FirstOrDefault();

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

            // Генерируем HTML содержимое
            string htmlContent = GenerateReportHtml(profileViewModel);

            // Конвертируем в байты для скачивания
            byte[] fileBytes = Encoding.UTF8.GetBytes(htmlContent);

            // Возвращаем файл
            return File(fileBytes, "text/html", $"Сводка_{client.CompanyName}_{DateTime.Now:yyyyMMdd_HHmmss}.html");
        }

        private string GenerateReportHtml(ProfileViewModel model)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='ru'>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset='UTF-8'>");
            sb.AppendLine("    <title>Сводка по заявкам и заказам</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 40px; color: #333; }");
            sb.AppendLine("        h1 { color: #2c3e50; border-bottom: 2px solid #3498db; padding-bottom: 10px; }");
            sb.AppendLine("        h2 { color: #2c3e50; margin-top: 30px; background: #ecf0f1; padding: 10px; border-radius: 5px; }");
            sb.AppendLine("        h3 { color: #3498db; margin: 20px 0 10px 0; }");
            sb.AppendLine("        h4 { color: #2c3e50; margin: 15px 0 10px 0; }");
            sb.AppendLine("        .company-info { background: #f8f9fa; padding: 15px; border-radius: 8px; margin-bottom: 20px; }");
            sb.AppendLine("        .company-info p { margin: 5px 0; }");
            sb.AppendLine("        .request-card { border: 1px solid #dee2e6; border-radius: 8px; margin-bottom: 20px; overflow: hidden; }");
            sb.AppendLine("        .request-header { background: #2c3e50; color: white; padding: 12px 15px; display: flex; justify-content: space-between; }");
            sb.AppendLine("        .request-body { padding: 15px; }");
            sb.AppendLine("        .info-row { margin-bottom: 10px; }");
            sb.AppendLine("        .info-row label { font-weight: bold; min-width: 120px; display: inline-block; }");
            sb.AppendLine("        .request-status { color: #3498db; font-weight: bold; }");
            sb.AppendLine("        .order-card { background: #f8f9fa; border-left: 3px solid #3498db; padding: 12px; margin-bottom: 12px; border-radius: 5px; }");
            sb.AppendLine("        .order-header { display: flex; justify-content: space-between; margin-bottom: 10px; padding-bottom: 5px; border-bottom: 1px solid #dee2e6; }");
            sb.AppendLine("        .services-list ul { margin: 5px 0 0 20px; }");
            sb.AppendLine("        .services-list li { margin: 3px 0; }");
            sb.AppendLine("        .no-data { color: #7f8c8d; font-style: italic; padding: 10px; }");
            sb.AppendLine("        .footer { margin-top: 30px; text-align: center; font-size: 12px; color: #7f8c8d; border-top: 1px solid #dee2e6; padding-top: 20px; }");
            sb.AppendLine("        .badge { display: inline-block; padding: 3px 8px; border-radius: 4px; font-size: 12px; font-weight: bold; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.AppendLine($"    <h1>Сводка по заявкам и заказам</h1>");
            sb.AppendLine($"    <p>Дата формирования: {DateTime.Now:dd.MM.yyyy HH:mm:ss}</p>");

            // Информация о компании
            sb.AppendLine("    <div class='company-info'>");
            sb.AppendLine($"        <p><strong>Компания:</strong> {model.Client.CompanyName}</p>");
            sb.AppendLine($"        <p><strong>Контактное лицо:</strong> {model.Client.ContactPersonFullname}</p>");
            sb.AppendLine($"        <p><strong>Телефон:</strong> {model.Client.ContactPhone}</p>");
            sb.AppendLine($"        <p><strong>Email:</strong> {model.Client.Email}</p>");
            sb.AppendLine($"        <p><strong>ИНН:</strong> {model.Client.Inn} | <strong>КПП:</strong> {model.Client.Kpp}</p>");
            sb.AppendLine("    </div>");

            // Заявки
            sb.AppendLine("    <h2>Мои заявки</h2>");

            if (model.Requests != null && model.Requests.Any())
            {
                foreach (var request in model.Requests)
                {
                    sb.AppendLine("    <div class='request-card'>");
                    sb.AppendLine("        <div class='request-header'>");
                    sb.AppendLine($"            <h3 style='margin:0; color:white;'>Заявка №{request.RequestId}</h3>");
                    sb.AppendLine($"            <span>от {request.CreationDate:dd.MM.yyyy}</span>");
                    sb.AppendLine("        </div>");
                    sb.AppendLine("        <div class='request-body'>");
                    sb.AppendLine("            <div class='request-info'>");
                    sb.AppendLine("                <div class='info-row'>");
                    sb.AppendLine($"                    <label>Адрес:</label>");
                    sb.AppendLine($"                    <span>{System.Security.SecurityElement.Escape(request.ServiceAddress)}</span>");
                    sb.AppendLine("                </div>");
                    sb.AppendLine("                <div class='info-row'>");
                    sb.AppendLine("                    <label>Текущий статус:</label>");
                    sb.AppendLine($"                    <span class='request-status'>");
                    if (!string.IsNullOrEmpty(request.LastStateName))
                    {
                        sb.AppendLine($"                        {request.LastStateName} от {request.LastStateDate?.ToString("dd.MM.yyyy") ?? "не указана"}");
                    }
                    else
                    {
                        sb.AppendLine("                        Статус не определен");
                    }
                    sb.AppendLine("                    </span>");
                    sb.AppendLine("                </div>");
                    sb.AppendLine("            </div>");

                    if (request.Orders != null && request.Orders.Any())
                    {
                        sb.AppendLine("            <div class='orders-section'>");
                        sb.AppendLine("                <h4>Заказы по заявке:</h4>");
                        foreach (var order in request.Orders)
                        {
                            sb.AppendLine("                <div class='order-card'>");
                            sb.AppendLine("                    <div class='order-header'>");
                            sb.AppendLine($"                        <strong>Заказ №{order.OrderId}</strong>");
                            sb.AppendLine($"                        <span>от {order.CreationDate:dd.MM.yyyy}</span>");
                            sb.AppendLine("                    </div>");
                            sb.AppendLine("                    <div class='order-body'>");
                            sb.AppendLine("                        <div class='info-row'>");
                            sb.AppendLine($"                            <label>Адрес доставки:</label>");
                            sb.AppendLine($"                            <span>{System.Security.SecurityElement.Escape(order.ServiceAddress)}</span>");
                            sb.AppendLine("                        </div>");
                            if (order.Services != null && order.Services.Any())
                            {
                                sb.AppendLine("                        <div class='services-list'>");
                                sb.AppendLine("                            <label>Услуги:</label>");
                                sb.AppendLine("                            <ul>");
                                foreach (var service in order.Services)
                                {
                                    sb.AppendLine($"                                <li>{System.Security.SecurityElement.Escape(service.ServiceName)} — {service.Amount} шт.</li>");
                                }
                                sb.AppendLine("                            </ul>");
                                sb.AppendLine("                        </div>");
                            }
                            else
                            {
                                sb.AppendLine("                        <p class='no-data'>Услуги не указаны</p>");
                            }
                            sb.AppendLine("                    </div>");
                            sb.AppendLine("                </div>");
                        }
                        sb.AppendLine("            </div>");
                    }
                    else
                    {
                        sb.AppendLine("            <p class='no-data'>Нет заказов по этой заявке</p>");
                    }
                    sb.AppendLine("        </div>");
                    sb.AppendLine("    </div>");
                }
            }
            else
            {
                sb.AppendLine("    <div class='empty-state'>");
                sb.AppendLine("        <p>У вас пока нет заявок</p>");
                sb.AppendLine("    </div>");
            }

            sb.AppendLine("    <div class='footer'>");
            sb.AppendLine($"        <p>© {DateTime.Now.Year} ООО СтанкоСтрой. Все права защищены.</p>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

    }
}