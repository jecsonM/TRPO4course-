using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MachineServices.Models;

namespace MachineServices.Controllers.ForTables
{
    public class ServiceProvisionsController : Controller
    {
        private readonly MyDbContext _context;

        public ServiceProvisionsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: ServiceProvisions
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.ServiceProvisions.Include(s => s.Masters).Include(s => s.Order).Include(s => s.Service);
            return View(await myDbContext.ToListAsync());
        }

        // GET: ServiceProvisions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceProvision = await _context.ServiceProvisions
                .Include(s => s.Masters)
                .Include(s => s.Order)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (serviceProvision == null)
            {
                return NotFound();
            }

            return View(serviceProvision);
        }

        // GET: ServiceProvisions/Create
        public IActionResult Create()
        {
            ViewData["MastersId"] = new SelectList(_context.Staff, "StaffId", "StaffId");
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            ViewData["ServiceId"] = new SelectList(_context.MachineServices, "ServiceId", "ServiceId");
            return View();
        }

        // POST: ServiceProvisions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ServiceId,MastersId,Amount")] ServiceProvision serviceProvision)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceProvision);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MastersId"] = new SelectList(_context.Staff, "StaffId", "StaffId", serviceProvision.MastersId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", serviceProvision.OrderId);
            ViewData["ServiceId"] = new SelectList(_context.MachineServices, "ServiceId", "ServiceId", serviceProvision.ServiceId);
            return View(serviceProvision);
        }

        // GET: ServiceProvisions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceProvision = await _context.ServiceProvisions.FindAsync(id);
            if (serviceProvision == null)
            {
                return NotFound();
            }
            ViewData["MastersId"] = new SelectList(_context.Staff, "StaffId", "StaffId", serviceProvision.MastersId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", serviceProvision.OrderId);
            ViewData["ServiceId"] = new SelectList(_context.MachineServices, "ServiceId", "ServiceId", serviceProvision.ServiceId);
            return View(serviceProvision);
        }

        // POST: ServiceProvisions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,ServiceId,MastersId,Amount")] ServiceProvision serviceProvision)
        {
            if (id != serviceProvision.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceProvision);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceProvisionExists(serviceProvision.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MastersId"] = new SelectList(_context.Staff, "StaffId", "StaffId", serviceProvision.MastersId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", serviceProvision.OrderId);
            ViewData["ServiceId"] = new SelectList(_context.MachineServices, "ServiceId", "ServiceId", serviceProvision.ServiceId);
            return View(serviceProvision);
        }

        // GET: ServiceProvisions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceProvision = await _context.ServiceProvisions
                .Include(s => s.Masters)
                .Include(s => s.Order)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (serviceProvision == null)
            {
                return NotFound();
            }

            return View(serviceProvision);
        }

        // POST: ServiceProvisions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceProvision = await _context.ServiceProvisions.FindAsync(id);
            if (serviceProvision != null)
            {
                _context.ServiceProvisions.Remove(serviceProvision);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceProvisionExists(int id)
        {
            return _context.ServiceProvisions.Any(e => e.OrderId == id);
        }
    }
}
