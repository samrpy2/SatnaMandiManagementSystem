using BCrypt.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using UserLogin.Data;
using UserLogin.Models;


namespace UserLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        // DbContext को यहाँ इंजेक्ट कर रहे हैं
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. लॉगिन पेज दिखाने के लिए (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                // 📌 2. यूजर का परिचय पत्र (Claims) बनाना
                var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.Username)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 📌 3. कंप्यूटर में ब्राउज़र के अंदर सुरक्षित कुकी/सेशन सेव करना
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity)).Wait();

                // लॉगिन सफल! होमपेज पर भेजें
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "गलत यूजरनेम या पासवर्ड!";
            return View();
        }


        // 1. रजिस्ट्रेशन पेज दिखाने के लिए (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 2. साइन अप बटन दबाने पर नया यूजर डेटाबेस में सेव करने के लिए (POST)
        [HttpPost]
        
        public IActionResult Register(string username, string password)
        {
            var alreadyExists = _context.Users.Any(u => u.Username == username);
            if (alreadyExists)
            {
                ViewBag.ErrorMessage = "यह यूजरनेम पहले से मौजूद है!";
                return View();
            }

            // 📌 जादुई लाइन: पासवर्ड को हैश (लॉक) करना
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var newUser = new UserLogin.Models.User
            {
                Username = username,
                Password = hashedPassword // अब डेटाबेस में सुरक्षित हैश सेव होगा
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            ViewBag.SuccessMessage = "अकाउंट सफलतापूर्वक बन गया!";
            return View();
        }
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return RedirectToAction("Login");
        }



        // 📌 1. पासवर्ड बदलने का पेज दिखाने के लिए (GET)
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // 📌 2. पासवर्ड बदलने का लॉजिक प्रोसेस करने के लिए (POST)
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // गिटहब/कुकी सेशन से वर्तमान लॉगिन यूजर का नाम (Email/Username) निकालें
                var username = User.Identity?.Name;

                // डेटाबेस से उस यूजर का असली रिकॉर्ड ढूंढें
                var user = _context.Users.FirstOrDefault(u => u.Username == username || u.Username == username);

                if (user != null)
                {
                    // 🔒 BCrypt की मदद से चेक करें कि फॉर्म में डाला गया पुराना पासवर्ड सही है या नहीं
                    if (BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password))
                    {
                        // पुराने पासवर्ड को नए हैश पासवर्ड से बदलें
                        user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

                        _context.Users.Update(user);
                        _context.SaveChanges();

                        // सफलता का मैसेज फ्रंटएंड पर भेजने के लिए TempData का उपयोग
                        TempData["SuccessMessage"] = "🎉 पासवर्ड सफलतापूर्वक बदल गया है!";
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("OldPassword", "पुराना पासवर्ड गलत है!");
                    }
                }
            }
            return View(model);
        }

    }
}
