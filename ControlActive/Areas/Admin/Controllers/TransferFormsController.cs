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
    public class TransferFormsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransferFormsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TransferForms
        public async Task<IActionResult> Index()
        {
            return View(await _context.TransferForms.ToListAsync());
        }

        // GET: Admin/TransferForms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferForm = await _context.TransferForms
                .FirstOrDefaultAsync(m => m.TransferFormId == id);
            if (transferForm == null)
            {
                return NotFound();
            }

            return View(transferForm);
        }

        // GET: Admin/TransferForms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/TransferForms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TransferFormId,TransferFormName,Status")] TransferForm transferForm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transferForm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transferForm);
        }

        // GET: Admin/TransferForms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferForm = await _context.TransferForms.FindAsync(id);
            if (transferForm == null)
            {
                return NotFound();
            }
            return View(transferForm);
        }

        // POST: Admin/TransferForms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransferFormId,TransferFormName,Status")] TransferForm transferForm)
        {
            if (id != transferForm.TransferFormId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transferForm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferFormExists(transferForm.TransferFormId))
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
            return View(transferForm);
        }

        // GET: Admin/TransferForms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferForm = await _context.TransferForms
                .FirstOrDefaultAsync(m => m.TransferFormId == id);
            if (transferForm == null)
            {
                return NotFound();
            }

            return View(transferForm);
        }

        // POST: Admin/TransferForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transferForm = await _context.TransferForms.FindAsync(id);
            _context.TransferForms.Remove(transferForm);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransferFormExists(int id)
        {
            return _context.TransferForms.Any(e => e.TransferFormId == id);
        }
    }
}
