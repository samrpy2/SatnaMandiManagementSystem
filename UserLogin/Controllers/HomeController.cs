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
            // 1. वैरायटी काउंट (सब्जी और फल अलग-अलग)
            int totalItemsCount = _context.MandiItems.Count();
            int vegetableCount = _context.MandiItems.Count(x => x.Category == "Vegetable");
            int fruitCount = _context.MandiItems.Count(x => x.Category == "Fruit");

            // 2. उपलब्ध स्टॉक वजन (सब्जी और फल अलग-अलग)
            double totalQuantity = _context.MandiItems.Any() ? _context.MandiItems.Sum(x => x.QuantityInQuintal) : 0;
            double vegQuantity = _context.MandiItems.Any(x => x.Category == "Vegetable") ? _context.MandiItems.Where(x => x.Category == "Vegetable").Sum(x => x.QuantityInQuintal) : 0;
            double fruitQuantity = _context.MandiItems.Any(x => x.Category == "Fruit") ? _context.MandiItems.Where(x => x.Category == "Fruit").Sum(x => x.QuantityInQuintal) : 0;

            // 3. कुल उपलब्ध स्टॉक का कुल मूल्य
            double totalStockValue = _context.MandiItems.Any()
                ? _context.MandiItems.Sum(x => x.QuantityInQuintal * x.TodayRatePerQuintal)
                : 0;

            // 📌 4. नया अपडेट: केवल सब्जियों का कुल अनुमानित मूल्य (मात्रा * भाव)
            double vegStockValue = _context.MandiItems.Any(x => x.Category == "Vegetable")
                ? _context.MandiItems.Where(x => x.Category == "Vegetable").Sum(x => x.QuantityInQuintal * x.TodayRatePerQuintal)
                : 0;

            // 📌 5. नया अपडेट: केवल फलों का कुल अनुमानित मूल्य (मात्रा * भाव)
            double fruitStockValue = _context.MandiItems.Any(x => x.Category == "Fruit")
                ? _context.MandiItems.Where(x => x.Category == "Fruit").Sum(x => x.QuantityInQuintal * x.TodayRatePerQuintal)
                : 0;

            // सभी वैल्यूज को ViewBag के ज़रिए व्यू पर भेजना
            ViewBag.TotalItemsCount = totalItemsCount;
            ViewBag.VegetableCount = vegetableCount;
            ViewBag.FruitCount = fruitCount;
            ViewBag.TotalQuantity = totalQuantity;
            ViewBag.VegQuantity = vegQuantity;
            ViewBag.FruitQuantity = fruitQuantity;
            ViewBag.TotalStockValue = totalStockValue;
            ViewBag.VegStockValue = vegStockValue;
            ViewBag.FruitStockValue = fruitStockValue;

            return View();
        }



    }
}
