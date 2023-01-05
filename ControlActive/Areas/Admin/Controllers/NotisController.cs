using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlActive.Data;
using ControlActive.Models;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using DocumentFormat.OpenXml.Drawing.Charts;
using ControlActive.Constants;
using Microsoft.AspNetCore.Authorization;

namespace ControlActive.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DefaultRoles.Role_Admin)]
    public class NotisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Notis
        public async Task<IActionResult> Index()
        {
            return View(await _context.Notifications.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> GetNotifications([FromBody]JObject data)
        {
            if(data["userId"] == null)
            {
                return Json(new { sucess = false, message = "Хабарномаларни олишда хатолик юз берди!" });
            }
            List<Noti> notis = new();
            var userId = data["userId"].ToString();

            if ((bool)data["isAll"])
            {
                notis = await _context.Notifications.Where(n => n.ToUserId == userId).OrderByDescending(n => n.CreatedDate).ToListAsync();
                
            }
            else if ((bool)data["isRead"])
            {
                notis = await _context.Notifications.Where(n => n.ToUserId == userId && n.IsRead).OrderByDescending(n => n.CreatedDate).ToListAsync();
            }

            else
            {
                notis = await _context.Notifications.Where(n => n.ToUserId == userId && !n.IsRead).OrderByDescending(n => n.CreatedDate).ToListAsync();
            }

            foreach(var item in notis)
            {
                if (item.CreatedDate.ToShortDateString().Equals(DateTime.Now.ToShortDateString()))
                    item.CreatedTimeStr = item.CreatedDate.ToShortTimeString();
                else
                    item.CreatedTimeStr = item.CreatedDate.ToShortDateString();
            }
            
            return Json(new { data = notis, success = true });
        }

        // GET: Admin/Notis/Details/5
        public async Task<IActionResult> Details(int? id, int? type, int? objectId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noti = await _context.Notifications
                .FirstOrDefaultAsync(m => m.NotiId == id);
            if (noti == null)
            {
                return NotFound();
            }

            ViewBag.Type = type;

            if(type == 1)
            {
                ViewBag.Target = "Кўчмас мулк объекти";
            }

            if (type == 2)
            {
                ViewBag.Target = "Улуш активлари";
            }

            if (type == 3)
            {
                ViewBag.Target = "Бериб юборилган кўчмас мулк объекти";
            }

            if (type == 4)
            {
                ViewBag.Target = "Бериб юборилган улуш активлари";
            }

            if (type == 5)
            {
                ViewBag.Target = "Аукциондаги кўчмас мулк объекти";
            }

            if (type == 6)
            {
                ViewBag.Target = "Аукциондаги улуш активлари";
            }

            if (type == 7)
            {
                ViewBag.Target = "Бир марталик тўлов асосида сотиладиган кўчмас мулк объекти";
            }

            if (type == 8)
            {
                ViewBag.Target = "Бир марталик тўлов асосида сотилган кўчмас мулк объекти шартномаси";
            }

            if (type == 9)
            {
                ViewBag.Target = "Бир марталик тўлов асосида сотилган кўчмас мулк объекти акти";
            }

            if (type == 10)
            {
                ViewBag.Target = "Бир марталик тўлов асосида сотиладиган улуш активлари";
            }

            if (type == 11)
            {
                ViewBag.Target = "Бир марталик тўлов асосида сотилган улуш активлари шартномаси";
            }

            if (type == 12)
            {
                ViewBag.Target = "Бир марталик тўлов асосида сотилган улуш активлари акти";
            }

            if (type == 13)
            {
                ViewBag.Target = "Бўлиб-бўлиб тўлашга сотилган кўчмас мулк объекти";
            }

            if (type == 14)
            {
                ViewBag.Target = "Бўлиб-бўлиб тўлашга сотилган кўчмас мулк объекти шартномаси";
            }


            if (type == 15)
            {
                ViewBag.Target = "Бўлиб-бўлиб тўлашга сотилган улуш активлари";
            }

            if (type == 16)
            {
                ViewBag.Target = "Бўлиб-бўлиб тўлашга сотилган улуш активлари шартномаси";
            }

            var fromUser = await _context.ApplicationUsers.Include(u => u.Organization).FirstOrDefaultAsync(u => u.Id == noti.FromUserId);
            if (fromUser != null)
                ViewBag.OrgName = fromUser.Organization.OrganizationName;
            if (objectId != null)
                ViewBag.ObjectId = objectId;

            return View(noti);
        }

        [HttpPost]
        public async Task<IActionResult> MakeRead([FromBody] int? id)
        {
            if (id == null)
            {
                return Json(new {success = false, message ="Хатолик юз берди!"});
            }

            var noti = await _context.Notifications
                .FirstOrDefaultAsync(m => m.NotiId == id);
            if (noti == null)
            {
                return Json(new { success = false, message = "Хабар топилмади!" });
            }

            noti.IsRead = true;
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> MakeReadAll([FromBody] int[] data)
        {
            if (data == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            if (data.Length == 0)
            {
                return Json(new { success = false, message = "Хабар танланмаган!" });
            }

            Noti noti = new();
            foreach (var id in data)
            {
                noti = await _context.Notifications
                .FirstOrDefaultAsync(m => m.NotiId == id);
                if (noti == null)
                {
                    return Json(new { success = false, message = "Хабар топилмади!" });
                }

                try
                {
                    noti.IsRead = true;
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Хатолик юз берди!," + ex.Message });
                }
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> MakeUnRead([FromBody] int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            var noti = await _context.Notifications
                .FirstOrDefaultAsync(m => m.NotiId == id);
            if (noti == null)
            {
                return Json(new { success = false, message = "Хабар топилмади!" });
            }

            noti.IsRead = false;
            await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> MakeUnReadAll([FromBody] int[] data)
        {
            if (data == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            if (data.Length == 0)
            {
                return Json(new { success = false, message = "Хабар танланмаган!" });
            }

            Noti noti = new();
            foreach (var id in data)
            {
                noti = await _context.Notifications
                .FirstOrDefaultAsync(m => m.NotiId == id);
                if (noti == null)
                {
                    return Json(new { success = false, message = "Хабар топилмади!" });
                }

                try
                {
                    noti.IsRead = false;
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Хатолик юз берди!," + ex.Message });
                }
            }

            return Json(new { success = true });
        }
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Admin/Notis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("NotiId,FromUserId,ToUserId,NotiHeader,NotiBody,IsRead,Url,CreatedDate,Message,FromUserName,ToUserName")] Noti noti)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(noti);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(noti);
        //}

        //// GET: Admin/Notis/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var noti = await _context.Notifications.FindAsync(id);
        //    if (noti == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(noti);
        //}

        //// POST: Admin/Notis/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("NotiId,FromUserId,ToUserId,NotiHeader,NotiBody,IsRead,Url,CreatedDate,Message,FromUserName,ToUserName")] Noti noti)
        //{
        //    if (id != noti.NotiId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(noti);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!NotiExists(noti.NotiId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(noti);
        //}

        // GET: Admin/Notis/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new {success = false, message="Хатолик юз берди!"});
            }

            var noti = await _context.Notifications
                .FirstOrDefaultAsync(m => m.NotiId == id);
            if (noti == null)
            {
                return Json(new { success = false, message = "Хабар топилмади!" });
            }
            try
            {
                _context.Notifications.Remove(noti);
                await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "Хатолик юз берди!,"+ex.Message });
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAll([FromBody] int[] data)
        {
            
            if (data == null)
            {
                return Json(new { success = false, message = "Хатолик юз берди!" });
            }

            if (data.Length == 0)
            {
                return Json(new { success = false, message = "Ўчириладиган хабар танланмаган!" });
            }

            Noti noti = new();
            foreach(var id in data)
            {
                noti = await _context.Notifications
                .FirstOrDefaultAsync(m => m.NotiId == id);
                if (noti == null)
                {
                    return Json(new { success = false, message = "Хабар топилмади!" });
                }

                try
                {
                    _context.Notifications.Remove(noti);
                    await _context.SaveChangesAsync(User?.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Хатолик юз берди!," + ex.Message });
                }
            }
           
            return Json(new { success = true });
        }

        // POST: Admin/Notis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var noti = await _context.Notifications.FindAsync(id);
            _context.Notifications.Remove(noti);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotiExists(int id)
        {
            return _context.Notifications.Any(e => e.NotiId == id);
        }
    }
}
