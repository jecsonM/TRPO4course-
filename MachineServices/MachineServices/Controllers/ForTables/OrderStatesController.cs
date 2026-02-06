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
    public class OrderStatesController : Controller
    {
        private readonly MyDbContext _context;

        public OrderStatesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: OrderStates
        public async Task<IActionResult> Index()
        {
            return View(await _context.OrderStates.ToListAsync());
        }

        // GET: OrderStates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderState = await _context.OrderStates
                .FirstOrDefaultAsync(m => m.OrderStateId == id);
            if (orderState == null)
            {
                return NotFound();
            }

            return View(orderState);
        }

        // GET: OrderStates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderStates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderStateId,OrderStateName")] OrderState orderState)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderState);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderState);
        }

        // GET: OrderStates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderState = await _context.OrderStates.FindAsync(id);
            if (orderState == null)
            {
                return NotFound();
            }
            return View(orderState);
        }

        // POST: OrderStates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderStateId,OrderStateName")] OrderState orderState)
        {
            if (id != orderState.OrderStateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderState);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderStateExists(orderState.OrderStateId))
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
            return View(orderState);
        }

        // GET: OrderStates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderState = await _context.OrderStates
                .FirstOrDefaultAsync(m => m.OrderStateId == id);
            if (orderState == null)
            {
                return NotFound();
            }

            return View(orderState);
        }

        // POST: OrderStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderState = await _context.OrderStates.FindAsync(id);
            if (orderState != null)
            {
                _context.OrderStates.Remove(orderState);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderStateExists(int id)
        {
            return _context.OrderStates.Any(e => e.OrderStateId == id);
        }
    }
}
