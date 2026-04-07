using MachineServices.Models;
using MachineServices.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MachineServices.Controllers
{
    public class ServicesController : Controller
    {
        private readonly MachineServicesDbContext _context;

        public ServicesController(MachineServicesDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Получаем все услуги с актуальной ценой (последней по дате)
            var services = await _context.Machineservices
                .Include(s => s.Relevantcosts)
                .Include(s => s.Creators)
                .Select(s => new ServiceListViewModel
                {
                    ServiceId = s.ServiceId,
                    MachineServiceName = s.MachineServiceName,
                    CurrentPrice = s.Relevantcosts
                        .OrderByDescending(r => r.SetDate)
                        .Select(r => r.RelevantCost1)
                        .FirstOrDefault(),
                    PriceSetDate = s.Relevantcosts
                        .OrderByDescending(r => r.SetDate)
                        .Select(r => r.SetDate)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return View(services);
        }
    }
}
