using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlActive.Data;
using ControlActive.Models;
using ControlActive.ViewModels;

namespace ControlActive.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RealEstatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RealEstatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RealEstates
        [HttpGet]
        public async Task<List<RealEstateShortDto>> GetRealEstates()
        {
            List<RealEstateShortDto> realEstatesInShort = new();
            var realEstates = await _context.RealEstates.Include(r => r.RegionOfObject).Include(r => r.DistrictOfObject).ToListAsync();

            foreach(var item in realEstates)
            {
                var cadastreRegDateStr = item.CadastreRegDate.ToShortDateString();
                RealEstateShortDto realEstateInShort = new()
                {
                    RealEstateId = item.RealEstateId,
                    RealEstateName = item.RealEstateName,
                    Activity = item.Activity,
                    CadastreNumber = item.CadastreNumber,
                    CadastreRegDate = cadastreRegDateStr,
                    Region = item.RegionOfObject.RegionName,
                    District = item.DistrictOfObject.DistrictName,
                    Address = item.Address,
                    AssetHolderName = item.AssetHolderName,
                    CadastreFileLink = item.CadastreFileLink,
                    Photo1Link = item.PhotoOfObjectLink1,
                    Photo2Link = item.PhotoOfObjectLink2,
                    Photo3Link = item.PhotoOfObjectLink3
                };

                realEstatesInShort.Add(realEstateInShort);
            }

            return realEstatesInShort;
        }

        // GET: api/RealEstates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RealEstate>> GetRealEstate(int id)
        {
            var realEstate = await _context.RealEstates.FindAsync(id);

            if (realEstate == null)
            {
                return NotFound();
            }

            return realEstate;
        }

        // PUT: api/RealEstates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRealEstate(int id, RealEstate realEstate)
        {
            if (id != realEstate.RealEstateId)
            {
                return BadRequest();
            }

            _context.Entry(realEstate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RealEstateExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RealEstates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RealEstate>> PostRealEstate(RealEstate realEstate)
        {
            _context.RealEstates.Add(realEstate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRealEstate", new { id = realEstate.RealEstateId }, realEstate);
        }

        // DELETE: api/RealEstates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRealEstate(int id)
        {
            var realEstate = await _context.RealEstates.FindAsync(id);
            if (realEstate == null)
            {
                return NotFound();
            }

            _context.RealEstates.Remove(realEstate);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RealEstateExists(int id)
        {
            return _context.RealEstates.Any(e => e.RealEstateId == id);
        }
    }
}
