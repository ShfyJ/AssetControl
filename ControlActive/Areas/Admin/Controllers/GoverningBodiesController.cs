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
    public class GoverningBodiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GoverningBodiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/GoverningBodies
        public async Task<IActionResult> Index()
        {
            return View(await _context.GoverningBodies.ToListAsync());
        }

        // GET: Admin/GoverningBodies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var governingBody = await _context.GoverningBodies
                .FirstOrDefaultAsync(m => m.GoverningBodyId == id);
            if (governingBody == null)
            {
                return NotFound();
            }

            return View(governingBody);
        }

        // GET: Admin/GoverningBodies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/GoverningBodies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GoverningBodyId,GoverningBodyName,Status")] GoverningBody governingBody)
        {
            if (ModelState.IsValid)
            {
                _context.Add(governingBody);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(governingBody);
        }

        // GET: Admin/GoverningBodies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var governingBody = await _context.GoverningBodies.FindAsync(id);
            if (governingBody == null)
            {
                return NotFound();
            }
            return View(governingBody);
        }

        // POST: Admin/GoverningBodies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GoverningBodyId,GoverningBodyName,Status")] GoverningBody governingBody)
        {
            if (id != governingBody.GoverningBodyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(governingBody);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoverningBodyExists(governingBody.GoverningBodyId))
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
            return View(governingBody);
        }

        // GET: Admin/GoverningBodies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var governingBody = await _context.GoverningBodies
                .FirstOrDefaultAsync(m => m.GoverningBodyId == id);
            if (governingBody == null)
            {
                return NotFound();
            }

            return View(governingBody);
        }

        // POST: Admin/GoverningBodies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var governingBody = await _context.GoverningBodies.FindAsync(id);
            _context.GoverningBodies.Remove(governingBody);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GoverningBodyExists(int id)
        {
            return _context.GoverningBodies.Any(e => e.GoverningBodyId == id);
        }
    }
}
