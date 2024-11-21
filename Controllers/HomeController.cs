using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PROG6212_New_POE.Data;
using PROG6212_New_POE.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace PROG6212_New_POE.Controllers
{
    public class HomeController : Controller
    {
        private readonly CMCSContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor with dependency injection
        public HomeController(CMCSContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
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
        public async Task<IActionResult> SubmitClaim(Claim claim, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".png", ".jpg" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                // Validate file extension
                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.Error = "File type must be .pdf, .docx, or .xlsx, or .png, or .jpg. ";
                    return View(claim);
                }

                // Validate file size (e.g., max 2 MB)
                if (file.Length > 2 * 1024 * 1024)
                {
                    ViewBag.Error = "File size cannot exceed 2 MB.";
                    return View(claim);
                }

                // Save the file to a folder in the server
                var uploadsPath = Path.Combine("wwwroot/uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Save the file name in the claim
                claim.FileName = file.FileName;
            }

            // Set default status and save claim to the database
            claim.Status = "Pending";
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction("TrackClaims");
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
            var userClaims = _context.Claims
                .Where(c => c.FirstName == firstName && c.LastName == lastName)
                .ToList();

            if (userClaims.Any())
            {
                return View(userClaims);
            }

            ViewBag.Message = "No claims found for the entered name.";
            return View(new List<Claim>());
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

            var claims = _context.Claims.ToList(); // Retrieve claims from the database
            return View(claims);
        }

        [HttpPost]
        public IActionResult VerifyClaims(int claimId, string action)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimID == claimId);
            if (claim != null)
            {
                claim.Status = action == "Approve" ? "Approved" : "Rejected";
                _context.SaveChanges(); // Save changes to the database
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
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
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
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View();
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
    }
}
