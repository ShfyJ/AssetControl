using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlActive.Data;
using ControlActive.Models;
using ControlActive.Constants;
using Microsoft.AspNetCore.Authorization;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class NotiMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotiMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/NotiMessages
        public async Task<IActionResult> Index()
        {
            return View(await _context.NotiMessageLibrary.ToListAsync());
        }

        // GET: Admin/NotiMessages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notiMessage = await _context.NotiMessageLibrary
                .FirstOrDefaultAsync(m => m.NotiMessageId == id);
            if (notiMessage == null)
            {
                return NotFound();
            }

            return View(notiMessage);
        }

        // GET: Admin/NotiMessages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NotiMessages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NotiMessageId,Content,MessageType")] NotiMessage notiMessage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notiMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notiMessage);
        }

        // GET: Admin/NotiMessages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notiMessage = await _context.NotiMessageLibrary.FindAsync(id);
            if (notiMessage == null)
            {
                return NotFound();
            }
            return View(notiMessage);
        }

        // POST: Admin/NotiMessages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NotiMessageId,Content,MessageType")] NotiMessage notiMessage)
        {
            if (id != notiMessage.NotiMessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notiMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotiMessageExists(notiMessage.NotiMessageId))
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
            return View(notiMessage);
        }

        // GET: Admin/NotiMessages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notiMessage = await _context.NotiMessageLibrary
                .FirstOrDefaultAsync(m => m.NotiMessageId == id);
            if (notiMessage == null)
            {
                return NotFound();
            }

            return View(notiMessage);
        }

        // POST: Admin/NotiMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notiMessage = await _context.NotiMessageLibrary.FindAsync(id);
            _context.NotiMessageLibrary.Remove(notiMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotiMessageExists(int id)
        {
            return _context.NotiMessageLibrary.Any(e => e.NotiMessageId == id);
        }
    }
}
