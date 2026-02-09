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
    public class RelevantCostsController : Controller
    {
        private readonly MyDbContext _context;

        public RelevantCostsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: RelevantCosts
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.RelevantCosts.Include(r => r.Creators).Include(r => r.Service);
            return View(await myDbContext.ToListAsync());
        }

        // GET: RelevantCosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relevantCost = await _context.RelevantCosts
                .Include(r => r.Creators)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.RelevantCostId == id);
            if (relevantCost == null)
            {
                return NotFound();
            }

            return View(relevantCost);
        }

        // GET: RelevantCosts/Create
        public IActionResult Create()
        {
            ViewData["CreatorsId"] = new SelectList(_context.Staff, "StaffId", "StaffId");
            ViewData["ServiceId"] = new SelectList(_context.MachineServices, "ServiceId", "ServiceId");
            return View();
        }

        // POST: RelevantCosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RelevantCostId,ServiceId,CreatorsId,RelevantCost1,SetDate")] RelevantCost relevantCost)
        {
            if (ModelState.IsValid)
            {
                _context.Add(relevantCost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorsId"] = new SelectList(_context.Staff, "StaffId", "StaffId", relevantCost.CreatorsId);
            ViewData["ServiceId"] = new SelectList(_context.MachineServices, "ServiceId", "ServiceId", relevantCost.ServiceId);
            return View(relevantCost);
        }

        // GET: RelevantCosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relevantCost = await _context.RelevantCosts.FindAsync(id);
            if (relevantCost == null)
            {
                return NotFound();
            }
            ViewData["CreatorsId"] = new SelectList(_context.Staff, "StaffId", "StaffId", relevantCost.CreatorsId);
            ViewData["ServiceId"] = new SelectList(_context.MachineServices, "ServiceId", "ServiceId", relevantCost.ServiceId);
            return View(relevantCost);
        }

        // POST: RelevantCosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RelevantCostId,ServiceId,CreatorsId,RelevantCost1,SetDate")] RelevantCost relevantCost)
        {
            if (id != relevantCost.RelevantCostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(relevantCost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RelevantCostExists(relevantCost.RelevantCostId))
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
            ViewData["CreatorsId"] = new SelectList(_context.Staff, "StaffId", "StaffId", relevantCost.CreatorsId);
            ViewData["ServiceId"] = new SelectList(_context.MachineServices, "ServiceId", "ServiceId", relevantCost.ServiceId);
            return View(relevantCost);
        }

        // GET: RelevantCosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relevantCost = await _context.RelevantCosts
                .Include(r => r.Creators)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(m => m.RelevantCostId == id);
            if (relevantCost == null)
            {
                return NotFound();
            }

            return View(relevantCost);
        }

        // POST: RelevantCosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var relevantCost = await _context.RelevantCosts.FindAsync(id);
            if (relevantCost != null)
            {
                _context.RelevantCosts.Remove(relevantCost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RelevantCostExists(int id)
        {
            return _context.RelevantCosts.Any(e => e.RelevantCostId == id);
        }
    }
}
