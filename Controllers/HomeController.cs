using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace PROG6212_New_POE.Controllers
{
    public class HomeController : Controller
    {
        private static List<Claim> Claims = new List<Claim>();
        private static List<User> Users = new List<User>();

        #region Account Actions

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
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserName", user.Name);
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            Users.Add(user);
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        #endregion

        #region Claim Actions

        public IActionResult SubmitClaim()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(Claim claim, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.Error = "File type must be PDF, DOCX, or XLSX.";
                    return View();
                }

                if (file.Length > 2 * 1024 * 1024)
                {
                    ViewBag.Error = "File size cannot exceed 2 MB.";
                    return View();
                }

                var filePath = Path.Combine("wwwroot/uploads", file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                claim.FileName = file.FileName;
                claim.Status = "Pending";
                Claims.Add(claim);
            }

            return RedirectToAction("TrackClaim");
        }

        public IActionResult TrackClaim()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TrackClaim(string firstName, string lastName)
        {
            var claims = Claims.Where(c => c.FirstName == firstName && c.LastName == lastName).ToList();
            ViewBag.Claims = claims;
            return View();
        }

        public IActionResult VerifyClaims()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (role == "Programme Coordinator" || role == "Academic Manager")
            {
                ViewBag.Claims = Claims;
                return View();
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult VerifyClaims(int claimId, string action)
        {
            var claim = Claims.FirstOrDefault(c => c.Id == claimId);
            if (claim != null)
            {
                claim.Status = action;
            }

            return RedirectToAction("VerifyClaims");
        }

        #endregion
    }
}
