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
    public class RequestStatesController : Controller
    {
        private readonly MyDbContext _context;

        public RequestStatesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: RequestStates
        public async Task<IActionResult> Index()
        {
            return View(await _context.RequestStates.ToListAsync());
        }

        // GET: RequestStates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestState = await _context.RequestStates
                .FirstOrDefaultAsync(m => m.RequestStateId == id);
            if (requestState == null)
            {
                return NotFound();
            }

            return View(requestState);
        }

        // GET: RequestStates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RequestStates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestStateId,RequestStateName")] RequestState requestState)
        {
            if (ModelState.IsValid)
            {
                _context.Add(requestState);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(requestState);
        }

        // GET: RequestStates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestState = await _context.RequestStates.FindAsync(id);
            if (requestState == null)
            {
                return NotFound();
            }
            return View(requestState);
        }

        // POST: RequestStates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestStateId,RequestStateName")] RequestState requestState)
        {
            if (id != requestState.RequestStateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestState);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestStateExists(requestState.RequestStateId))
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
            return View(requestState);
        }

        // GET: RequestStates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestState = await _context.RequestStates
                .FirstOrDefaultAsync(m => m.RequestStateId == id);
            if (requestState == null)
            {
                return NotFound();
            }

            return View(requestState);
        }

        // POST: RequestStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requestState = await _context.RequestStates.FindAsync(id);
            if (requestState != null)
            {
                _context.RequestStates.Remove(requestState);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestStateExists(int id)
        {
            return _context.RequestStates.Any(e => e.RequestStateId == id);
        }
    }
}
