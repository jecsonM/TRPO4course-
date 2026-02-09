using MachineServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MachineServices.Controllers.ForTables
{
    [Authorize]
    public class MachineServicesController : Controller
    {
        private readonly MyDbContext _context;

        public MachineServicesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: MachineServices
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.MachineServices.Include(m => m.Creators);
            return View(await myDbContext.ToListAsync());
        }

        // GET: MachineServices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineService = await _context.MachineServices
                .Include(m => m.Creators)
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            if (machineService == null)
            {
                return NotFound();
            }

            return View(machineService);
        }

        // GET: MachineServices/Create
        public IActionResult Create()
        {
            ViewData["CreatorsId"] = new SelectList(_context.Staff, "StaffId", "StaffId");
            return View();
        }

        // POST: MachineServices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceId,CreatorsId,MachineServiceName")] MachineService machineService)
        {
            if (ModelState.IsValid)
            {
                _context.Add(machineService);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorsId"] = new SelectList(_context.Staff, "StaffId", "StaffId", machineService.CreatorsId);
            return View(machineService);
        }

        // GET: MachineServices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineService = await _context.MachineServices.FindAsync(id);
            if (machineService == null)
            {
                return NotFound();
            }
            ViewData["CreatorsId"] = new SelectList(_context.Staff, "StaffId", "StaffId", machineService.CreatorsId);
            return View(machineService);
        }

        // POST: MachineServices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceId,CreatorsId,MachineServiceName")] MachineService machineService)
        {
            if (id != machineService.ServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(machineService);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MachineServiceExists(machineService.ServiceId))
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
            ViewData["CreatorsId"] = new SelectList(_context.Staff, "StaffId", "StaffId", machineService.CreatorsId);
            return View(machineService);
        }

        // GET: MachineServices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var machineService = await _context.MachineServices
                .Include(m => m.Creators)
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            if (machineService == null)
            {
                return NotFound();
            }

            return View(machineService);
        }

        // POST: MachineServices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var machineService = await _context.MachineServices.FindAsync(id);
            if (machineService != null)
            {
                _context.MachineServices.Remove(machineService);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MachineServiceExists(int id)
        {
            return _context.MachineServices.Any(e => e.ServiceId == id);
        }
    }
}
