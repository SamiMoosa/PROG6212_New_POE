using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PROG6212_New_POE.Models;
using System.Collections.Generic;
using System.Linq;

namespace PROG6212_New_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        // In-memory lists to simulate a database
        private static List<User> Users = new List<User>();
        private static List<Claim> Claims = new List<Claim>();

        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Home page
        public IActionResult Index()
        {
            var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");
            ViewData["UserRole"] = userRole;
            return View();
        }

        // Privacy page
        public IActionResult Privacy()
        {
            return View();
        }

        // Submit Claim Page
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitClaim(Claim claim, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                if (file.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("File", "File size exceeds 2 MB.");
                    return View();
                }

                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xlsx" };
                var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("File", "File type must be .pdf, .doc, .docx, or .xlsx.");
                    return View();
                }

                // Save file (you may save it on the server or simulate storage)
                claim.FileName = file.FileName;
            }

            claim.Status = "Pending";
            Claims.Add(claim);

            return RedirectToAction("Index");
        }

        // Track Claims Page
        [HttpGet]
        public IActionResult TrackClaims()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TrackClaims(string firstName, string lastName)
        {
            var userClaims = Claims.Where(c => c.FirstName == firstName && c.LastName == lastName).ToList();
            if (userClaims.Any())
            {
                return View("ClaimList", userClaims);
            }
            ViewBag.Message = "No claims found for the entered name.";
            return View();
        }

        // Verify Claims Page (for Programme Coordinators and Academic Managers)
        [HttpGet]
        public IActionResult VerifyClaims()
        {
            var userRole = _httpContextAccessor.HttpContext?.Session.GetString("UserRole");

            if (userRole != "Programme Coordinator" && userRole != "Academic Manager")
            {
                return RedirectToAction("Login");
            }

            return View(Claims);
        }

        [HttpPost]
        public IActionResult VerifyClaims(int claimId, string action)
        {
            var claim = Claims.FirstOrDefault(c => c.ClaimID == claimId);
            if (claim != null)
            {
                claim.Status = action == "Approve" ? "Approved" : "Rejected";
            }

            return RedirectToAction("VerifyClaims");
        }

        // Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                _httpContextAccessor.HttpContext?.Session.SetString("UserRole", user.Role);
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Invalid login credentials.";
            return View();
        }

        // Logout
        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
            return RedirectToAction("Index");
        }

        // Sign Up Page
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            if (Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View();
            }

            Users.Add(user);
            return RedirectToAction("Login");
        }
    }
}
