using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserLogin.Data;
using System.Linq;

namespace UserLogin.Controllers
{
    [Authorize] // 📌 बिना लॉगिन के डैशबोर्ड नहीं दिखेगा
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 1. मंडी में कुल कितने प्रकार के आइटम्स हैं
            int totalItemsCount = _context.MandiItems.Count();

            // 2. कुल कितने क्विंटल स्टॉक मौजूद है (LINQ Sum)
            double totalQuantity = _context.MandiItems.Any()
                ? _context.MandiItems.Sum(x => x.QuantityInQuintal)
                : 0;

            // 3. पूरे स्टॉक की कुल अनुमानित कीमत कितनी है (मात्रा * भाव का कुल जोड़)
            double totalStockValue = _context.MandiItems.Any()
                ? _context.MandiItems.Sum(x => x.QuantityInQuintal * x.TodayRatePerQuintal)
                : 0;

            // 4. इन तीनों वैल्यूज को View पर भेजने के लिए ViewBag का उपयोग करेंगे
            ViewBag.TotalItemsCount = totalItemsCount;
            ViewBag.TotalQuantity = totalQuantity;
            ViewBag.TotalStockValue = totalStockValue;

            return View();
        }
    }
}
