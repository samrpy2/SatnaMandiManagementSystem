using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using UserLogin.Data; // 📌 आपके प्रोजेक्ट के डेटाबेस कॉन्टेक्स्ट का नेमस्पेस
using UserLogin.Models;

namespace UserLogin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context; // 👈 आपके DB Context का नाम

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 1. वैरायटी काउंट (सब्जी और फल अलग-अलग)
            int totalItemsCount = _context.MandiItems.Count(x => x.IsDeleted == false);
            int vegetableCount = _context.MandiItems.Count(x => x.Category == "Vegetable" && x.IsDeleted == false);
            int fruitCount = _context.MandiItems.Count(x => x.Category == "Fruit" && x.IsDeleted == false);

            // 2. उपलब्ध स्टॉक वजन (केवल लाइव माल का)
            double totalQuantity = _context.MandiItems.Where(x => x.IsDeleted == false).Any()
                ? _context.MandiItems.Where(x => x.IsDeleted == false).Sum(x => x.QuantityInQuintal)
                : 0;

            double vegQuantity = _context.MandiItems.Where(x => x.Category == "Vegetable" && x.IsDeleted == false).Any()
                ? _context.MandiItems.Where(x => x.Category == "Vegetable" && x.IsDeleted == false).Sum(x => x.QuantityInQuintal)
                : 0;

            double fruitQuantity = _context.MandiItems.Where(x => x.Category == "Fruit" && x.IsDeleted == false).Any()
                ? _context.MandiItems.Where(x => x.Category == "Fruit" && x.IsDeleted == false).Sum(x => x.QuantityInQuintal)
                : 0;

            // 3. उपलब्ध स्टॉक का कुल मूल्य (केवल लाइव माल का)
            double totalStockValue = _context.MandiItems.Where(x => x.IsDeleted == false).Any()
                ? _context.MandiItems.Where(x => x.IsDeleted == false).Sum(x => x.QuantityInQuintal * x.TodayRatePerQuintal)
                : 0;

            double vegStockValue = _context.MandiItems.Where(x => x.Category == "Vegetable" && x.IsDeleted == false).Any()
                ? _context.MandiItems.Where(x => x.Category == "Vegetable" && x.IsDeleted == false).Sum(x => x.QuantityInQuintal * x.TodayRatePerQuintal)
                : 0;

            double fruitStockValue = _context.MandiItems.Where(x => x.Category == "Fruit" && x.IsDeleted == false).Any()
                ? _context.MandiItems.Where(x => x.Category == "Fruit" && x.IsDeleted == false).Sum(x => x.QuantityInQuintal * x.TodayRatePerQuintal)
                : 0;

            // 📌 4. मुनाफ़ा और नुकसान कैलकुलेटर लॉजिक
            var liveItems = _context.MandiItems.Where(x => x.IsDeleted == false).ToList();
            double totalProfitOrLoss = 0;

            foreach (var item in liveItems)
            {
                if (item.YesterdayRatePerQuintal > 0)
                {
                    double diff = item.TodayRatePerQuintal - item.YesterdayRatePerQuintal;
                    totalProfitOrLoss += (item.QuantityInQuintal * diff);
                }
            }

            // 5. सभी वैल्यूज को ViewBag के ज़रिए व्यू पर सुरक्षित भेजना
            ViewBag.TotalItemsCount = totalItemsCount;
            ViewBag.VegetableCount = vegetableCount;
            ViewBag.FruitCount = fruitCount;
            ViewBag.TotalQuantity = totalQuantity;
            ViewBag.VegQuantity = vegQuantity;
            ViewBag.FruitQuantity = fruitQuantity;
            ViewBag.TotalStockValue = totalStockValue;
            ViewBag.VegStockValue = vegStockValue;
            ViewBag.FruitStockValue = fruitStockValue;
            ViewBag.TotalProfitOrLoss = totalProfitOrLoss;

            // 📌 यह वो आखरी रिटर्न लाइन है जो एरर को खत्म करेगी
            return View();
        }
    }
}
