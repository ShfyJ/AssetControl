using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlActive.Data;
using ControlActive.Models;
using Microsoft.AspNetCore.Authorization;
using ControlActive.Constants;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class InfrastucturesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InfrastucturesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Infrastuctures
        public async Task<IActionResult> Index()
        {
            return View(await _context.Infrastuctures.ToListAsync());
        }

        // GET: Admin/Infrastuctures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infrastucture = await _context.Infrastuctures
                .FirstOrDefaultAsync(m => m.InfrastructureId == id);
            if (infrastucture == null)
            {
                return NotFound();
            }

            return View(infrastucture);
        }

        // GET: Admin/Infrastuctures/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Infrastuctures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InfrastructureId,InfrastructureName,Status")] Infrastucture infrastucture)
        {
            if (ModelState.IsValid)
            {
                _context.Add(infrastucture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(infrastucture);
        }

        // GET: Admin/Infrastuctures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infrastucture = await _context.Infrastuctures.FindAsync(id);
            if (infrastucture == null)
            {
                return NotFound();
            }
            return View(infrastucture);
        }

        // POST: Admin/Infrastuctures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InfrastructureId,InfrastructureName,Status")] Infrastucture infrastucture)
        {
            if (id != infrastucture.InfrastructureId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(infrastucture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InfrastuctureExists(infrastucture.InfrastructureId))
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
            return View(infrastucture);
        }

        // GET: Admin/Infrastuctures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var infrastucture = await _context.Infrastuctures
                .FirstOrDefaultAsync(m => m.InfrastructureId == id);
            if (infrastucture == null)
            {
                return NotFound();
            }

            return View(infrastucture);
        }

        // POST: Admin/Infrastuctures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var infrastucture = await _context.Infrastuctures.FindAsync(id);
            _context.Infrastuctures.Remove(infrastucture);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InfrastuctureExists(int id)
        {
            return _context.Infrastuctures.Any(e => e.InfrastructureId == id);
        }
    }
}
