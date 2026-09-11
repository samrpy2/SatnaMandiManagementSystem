using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserLogin.Data;
using UserLogin.Models;
using System.Text;

namespace UserLogin.Controllers
{
    [Authorize] // 📌 बिना लॉगिन के कोई भी मंडी का स्टॉक नहीं बदल सकता!
    public class MandiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MandiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. आइटम जोड़ने का फॉर्म दिखाने के लिए (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 2. फॉर्म सबमिट होने पर डेटाबेस में सेव करने के लिए (POST)
        [HttpPost]
        public IActionResult Create(MandiItem item)
        {
            if (ModelState.IsValid)
            {
                item.LastUpdated = DateTime.UtcNow; // करंट टाइम सेट करें

                _context.MandiItems.Add(item);
                // ⏱️ ऑडिट लॉग रिकॉर्ड करें
                var log = new AuditLog
                {
                    UserEmail = User.Identity?.Name ?? "Unknown",
                    Action = "CREATE",
                    Details = $"नया स्टॉक जोड़ा गया: {item.ItemName} ({item.QuantityInQuintal} Qtl, ₹{item.TodayRatePerQuintal}/Qtl)"
                };
                _context.AuditLogs.Add(log);
                // 📌 EF Core में ऐड करें
                _context.SaveChanges();        // 📌 PostgreSQL में सेव करें

                ViewBag.SuccessMessage = $"{item.ItemName} मंडी स्टॉक में सफलतापूर्वक जोड़ दिया गया है!";
                return RedirectToAction("Create"); // वापस फॉर्म पर भेजें
            }

            ViewBag.ErrorMessage = "कृपया सभी डेटा सही-सही भरें!";
            return View(item);
        }
        [HttpGet]
        public IActionResult Index(string searchString, string categoryFilter, int? pageNumber)
        {
            // 1. Maintain existing filters in ViewData
            ViewData["CurrentFilter"] = searchString;
            ViewData["SelectedCategory"] = categoryFilter;

            // 2. Fetch basic query

            // 📌 केवल वही आइटम लाएं जो डिलीट नहीं हुए हैं (IsDeleted == false)
            var itemsQuery = from m in _context.MandiItems
                             where m.IsDeleted == false
                             select m;


            // 3. Apply search query filter
            if (!string.IsNullOrEmpty(searchString))
            {
                itemsQuery = itemsQuery.Where(s => s.ItemName.ToLower().Contains(searchString.ToLower()));
            }


            // 4. Apply category selection filter
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                itemsQuery = itemsQuery.Where(x => x.Category == categoryFilter);
            }

            // Order items so the newest or recently updated stock stays on top
            itemsQuery = itemsQuery.OrderByDescending(x => x.LastUpdated);

            // 5. Define page configuration (10 items per page)
            int pageSize = 10;
            int currentPage = pageNumber ?? 1;

            // 6. Return the paginated data structure instead of a raw list
            return View(PaginatedList<MandiItem>.Create(itemsQuery, currentPage, pageSize));
        }



        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Id के आधार पर उस सब्जी/फल को PostgreSQL में ढूंढो
            var item = _context.MandiItems.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return NotFound(); // अगर आइटम नहीं मिला तो 404 एरर पेज दिखाओ
            }

            // उस पुराने डेटा को एडिट फॉर्म (HTML View) पर भेज दो
            return View(item);
        }

        [HttpPost]
        public IActionResult Edit(MandiItem updatedItem)
        {
            if (ModelState.IsValid)
            {
                // 1. डेटाबेस से उस आइटम का पुराना रिकॉर्ड निकालें
                var existingItem = _context.MandiItems.AsNoTracking().FirstOrDefault(x => x.Id == updatedItem.Id);

                if (existingItem != null)
                {
                    // 2. पुराने आज के भाव को 'कल का भाव' बना दें
                    updatedItem.YesterdayRatePerQuintal = existingItem.TodayRatePerQuintal;
                }

                updatedItem.LastUpdated = DateTime.UtcNow;
                _context.MandiItems.Update(updatedItem);
                // ⏱️ ऑडिट लॉग रिकॉर्ड करें
                var log = new AuditLog
                {
                    UserEmail = User.Identity?.Name ?? "Unknown",
                    Action = "EDIT",
                    Details = $"स्टॉक अपडेट किया गया: {updatedItem.ItemName} (नया रेट: ₹{updatedItem.TodayRatePerQuintal}, नया स्टॉक: {updatedItem.QuantityInQuintal} Qtl)"
                };
                _context.AuditLogs.Add(log);

                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(updatedItem);
        }


        // 1. डिलीट करने से पहले यूजर को कन्फर्मेशन पेज दिखाना (GET)
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var item = _context.MandiItems.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item); // यह डेटा को Delete.cshtml पेज पर भेजेगा
        }

        // 2. जब यूजर 'Delete.cshtml' पेज पर जाकर "हाँ, डिलीट करें" बटन दबाएगा (POST)
        // 📌 सॉफ्ट डिलीट कन्फर्मेशन (Post)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // यह सुरक्षा टोकन की जांच करता है
        public IActionResult DeleteConfirmed(int id) // 👈 ध्यान दें: यहाँ 'id' ही लिखा होना चाहिए
        {
            // डेटाबेस से आइटम ढूंढें
            var item = _context.MandiItems.FirstOrDefault(x => x.Id == id);

            if (item != null)
            {
                // डेटाबेस से हमेशा के लिए डिलीट करने के बजाय सिर्फ IsDeleted को true करें
                item.IsDeleted = true;
                item.LastUpdated = DateTime.UtcNow;

                _context.MandiItems.Update(item);
                // ⏱️ ऑडिट लॉग रिकॉर्ड करें
                var log = new AuditLog
                {
                    UserEmail = User.Identity?.Name ?? "Unknown",
                    Action = "SOFT-DELETE",
                    Details = $"स्टॉक हटाया (Soft Delete) गया: {item.ItemName}"
                };
                _context.AuditLogs.Add(log);

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        // 📌 रसीद का पेज दिखाने के लिए (GET)
        [HttpGet]
        public IActionResult PrintReceipt(int id)
        {
            // डेटाबेस से आइटम खोजें
            var item = _context.MandiItems.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            // डेटा को व्यू (HTML) पर भेजें
            return View(item);
        }

        // 📌 लाइव स्टॉक डेटा को एक्सेल/CSV में डाउनलोड करने का इंजन
        [HttpGet]
        public IActionResult ExportToCSV()
        {
            // 1. केवल वही माल उठाएं जो सॉफ्ट डिलीट नहीं हुआ है
            var data = _context.MandiItems.Where(x => x.IsDeleted == false).ToList();

            // 2. CSV फ़ाइल का हेडर (कॉलम के नाम) तैयार करें
            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Sr No,Item Name,Category,Quantity (Qtl),Today Rate (Rs),Last Updated");

            // 3. लूप चलाकर सारा डेटा पंक्तियों (Rows) में भरें
            foreach (var item in data)
            {
                csvBuilder.AppendLine($"{item.Id},{item.ItemName},{item.Category},{item.QuantityInQuintal},{item.TodayRatePerQuintal},{item.LastUpdated}");
            }
            // 4. डेटा को बाइट एरे में बदलकर एक्सेल फ़ाइल के रूप में डाउनलोड कराएं
            var buffer = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            return File(buffer, "text/csv", $"Satna_Mandi_Stock_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}
