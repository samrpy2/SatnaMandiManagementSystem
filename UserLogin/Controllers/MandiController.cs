using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserLogin.Data;
using UserLogin.Models;

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

                _context.MandiItems.Add(item); // 📌 EF Core में ऐड करें
                _context.SaveChanges();        // 📌 PostgreSQL में सेव करें

                ViewBag.SuccessMessage = $"{item.ItemName} मंडी स्टॉक में सफलतापूर्वक जोड़ दिया गया है!";
                return RedirectToAction("Create"); // वापस फॉर्म पर भेजें
            }

            ViewBag.ErrorMessage = "कृपया सभी डेटा सही-सही भरें!";
            return View(item);
        }
        // 3. सारा मंडी स्टॉक स्क्रीन पर दिखाने के लिए (Read/Index)
        // 📌 लाइव स्टॉक बोर्ड - सर्च बार के साथ (Read/Index)
        [HttpGet]
        public IActionResult Index(string searchString)
        {
            // 1. डेटाबेस से सारे आइटम्स की क्वेरी तैयार करें (अभी रन नहीं होगी)
            var itemsQuery = from m in _context.MandiItems
                             select m;

            // 2. यदि यूजर ने सर्च बॉक्स में कुछ टाइप किया है (Null या खाली नहीं है)
            if (!string.IsNullOrEmpty(searchString))
            {
                // LINQ का उपयोग करके नाम मैच करें (PostgreSQL में Contains केस-सेंसिटिव हो सकता है, इसलिए Lower कर सकते हैं)
                itemsQuery = itemsQuery.Where(s => s.ItemName.ToLower().Contains(searchString.ToLower()));
            }

            // 3. सर्च की हुई स्ट्रिंग को वापस व्यू पर भेजें ताकि सर्च बॉक्स में वो नाम लिखा रहे
            ViewData["CurrentFilter"] = searchString;

            // 4. अंत में डेटाबेस से लिस्ट बनाकर व्यू (HTML) की तरफ भेजें
            return View(itemsQuery.ToList());
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

        // 2. फॉर्म सबमिट होने पर नया डेटा डेटाबेस में सुरक्षित करना (POST)
        [HttpPost]
        public IActionResult Edit(MandiItem updatedItem)
        {
            if (ModelState.IsValid)
            {
                updatedItem.LastUpdated = DateTime.UtcNow; // नया टाइमस्टैम्प सेट करें

                // EF Core को बताओ कि इस आइटम का डेटा बदल चुका है
                _context.MandiItems.Update(updatedItem);

                // 📌 यह जादुई लाइन आपके PostgreSQL में डेटा अपडेट कर देगी!
                _context.SaveChanges();

                // काम पूरा होने के बाद वापस लाइव स्टॉक बोर्ड (Index) पर चले जाओ
                return RedirectToAction("Index");
            }

            return View(updatedItem); // अगर डेटा सही नहीं भरा तो वापस उसी पेज पर एरर के साथ रहो
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
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _context.MandiItems.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                _context.MandiItems.Remove(item);
                _context.SaveChanges(); // डेटाबेस से हमेशा के लिए डिलीट
            }
            return RedirectToAction("Index"); // वापस लाइव स्टॉक बोर्ड पर भेजें
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


    }
}
