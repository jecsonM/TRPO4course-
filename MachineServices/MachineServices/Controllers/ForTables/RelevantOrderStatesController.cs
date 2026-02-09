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
    public class RelevantOrderStatesController : Controller
    {
        private readonly MyDbContext _context;

        public RelevantOrderStatesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: RelevantOrderStates
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.RelevantOrderStates.Include(r => r.Order).Include(r => r.OrderState);
            return View(await myDbContext.ToListAsync());
        }

        // GET: RelevantOrderStates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relevantOrderState = await _context.RelevantOrderStates
                .Include(r => r.Order)
                .Include(r => r.OrderState)
                .FirstOrDefaultAsync(m => m.RelevantOrderStateId == id);
            if (relevantOrderState == null)
            {
                return NotFound();
            }

            return View(relevantOrderState);
        }

        // GET: RelevantOrderStates/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            ViewData["OrderStateId"] = new SelectList(_context.OrderStates, "OrderStateId", "OrderStateId");
            return View();
        }

        // POST: RelevantOrderStates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RelevantOrderStateId,OrderId,OrderStateId,SetDate")] RelevantOrderState relevantOrderState)
        {
            if (ModelState.IsValid)
            {
                _context.Add(relevantOrderState);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", relevantOrderState.OrderId);
            ViewData["OrderStateId"] = new SelectList(_context.OrderStates, "OrderStateId", "OrderStateId", relevantOrderState.OrderStateId);
            return View(relevantOrderState);
        }

        // GET: RelevantOrderStates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relevantOrderState = await _context.RelevantOrderStates.FindAsync(id);
            if (relevantOrderState == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", relevantOrderState.OrderId);
            ViewData["OrderStateId"] = new SelectList(_context.OrderStates, "OrderStateId", "OrderStateId", relevantOrderState.OrderStateId);
            return View(relevantOrderState);
        }

        // POST: RelevantOrderStates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RelevantOrderStateId,OrderId,OrderStateId,SetDate")] RelevantOrderState relevantOrderState)
        {
            if (id != relevantOrderState.RelevantOrderStateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(relevantOrderState);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RelevantOrderStateExists(relevantOrderState.RelevantOrderStateId))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", relevantOrderState.OrderId);
            ViewData["OrderStateId"] = new SelectList(_context.OrderStates, "OrderStateId", "OrderStateId", relevantOrderState.OrderStateId);
            return View(relevantOrderState);
        }

        // GET: RelevantOrderStates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relevantOrderState = await _context.RelevantOrderStates
                .Include(r => r.Order)
                .Include(r => r.OrderState)
                .FirstOrDefaultAsync(m => m.RelevantOrderStateId == id);
            if (relevantOrderState == null)
            {
                return NotFound();
            }

            return View(relevantOrderState);
        }

        // POST: RelevantOrderStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var relevantOrderState = await _context.RelevantOrderStates.FindAsync(id);
            if (relevantOrderState != null)
            {
                _context.RelevantOrderStates.Remove(relevantOrderState);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RelevantOrderStateExists(int id)
        {
            return _context.RelevantOrderStates.Any(e => e.RelevantOrderStateId == id);
        }
    }
}
