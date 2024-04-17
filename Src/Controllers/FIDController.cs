using EFID_V2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFID_V2.Controllers
{
    [Authorize]
    public class FIDController : Controller
    {
        private readonly EFIDContext _context;

        public FIDController(EFIDContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> PengajuanInvestasi(string category)
        {
            if (category == "bd")
            {
                category = "BD";
            }
            else
            {
                category = "NON BD";
            }

            var query = await _context.vFID.Where(x => x.Jenisfid == category).ToListAsync();

            return View(query);
        }
    }
}
