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
    public class RelevantRequestStatesController : Controller
    {
        private readonly MyDbContext _context;

        public RelevantRequestStatesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: RelevantRequestStates
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.RelevantRequestStates.Include(r => r.Request).Include(r => r.RequestState);
            return View(await myDbContext.ToListAsync());
        }

        // GET: RelevantRequestStates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relevantRequestState = await _context.RelevantRequestStates
                .Include(r => r.Request)
                .Include(r => r.RequestState)
                .FirstOrDefaultAsync(m => m.RelevantRequestStateId == id);
            if (relevantRequestState == null)
            {
                return NotFound();
            }

            return View(relevantRequestState);
        }

        // GET: RelevantRequestStates/Create
        public IActionResult Create()
        {
            ViewData["RequestId"] = new SelectList(_context.Requests, "RequestId", "RequestId");
            ViewData["RequestStateId"] = new SelectList(_context.RequestStates, "RequestStateId", "RequestStateId");
            return View();
        }

        // POST: RelevantRequestStates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RelevantRequestStateId,RequestId,RequestStateId,SetDate")] RelevantRequestState relevantRequestState)
        {
            if (ModelState.IsValid)
            {
                _context.Add(relevantRequestState);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RequestId"] = new SelectList(_context.Requests, "RequestId", "RequestId", relevantRequestState.RequestId);
            ViewData["RequestStateId"] = new SelectList(_context.RequestStates, "RequestStateId", "RequestStateId", relevantRequestState.RequestStateId);
            return View(relevantRequestState);
        }

        // GET: RelevantRequestStates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relevantRequestState = await _context.RelevantRequestStates.FindAsync(id);
            if (relevantRequestState == null)
            {
                return NotFound();
            }
            ViewData["RequestId"] = new SelectList(_context.Requests, "RequestId", "RequestId", relevantRequestState.RequestId);
            ViewData["RequestStateId"] = new SelectList(_context.RequestStates, "RequestStateId", "RequestStateId", relevantRequestState.RequestStateId);
            return View(relevantRequestState);
        }

        // POST: RelevantRequestStates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RelevantRequestStateId,RequestId,RequestStateId,SetDate")] RelevantRequestState relevantRequestState)
        {
            if (id != relevantRequestState.RelevantRequestStateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(relevantRequestState);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RelevantRequestStateExists(relevantRequestState.RelevantRequestStateId))
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
            ViewData["RequestId"] = new SelectList(_context.Requests, "RequestId", "RequestId", relevantRequestState.RequestId);
            ViewData["RequestStateId"] = new SelectList(_context.RequestStates, "RequestStateId", "RequestStateId", relevantRequestState.RequestStateId);
            return View(relevantRequestState);
        }

        // GET: RelevantRequestStates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var relevantRequestState = await _context.RelevantRequestStates
                .Include(r => r.Request)
                .Include(r => r.RequestState)
                .FirstOrDefaultAsync(m => m.RelevantRequestStateId == id);
            if (relevantRequestState == null)
            {
                return NotFound();
            }

            return View(relevantRequestState);
        }

        // POST: RelevantRequestStates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var relevantRequestState = await _context.RelevantRequestStates.FindAsync(id);
            if (relevantRequestState != null)
            {
                _context.RelevantRequestStates.Remove(relevantRequestState);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RelevantRequestStateExists(int id)
        {
            return _context.RelevantRequestStates.Any(e => e.RelevantRequestStateId == id);
        }
    }
}
