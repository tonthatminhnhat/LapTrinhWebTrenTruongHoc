using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using System.Reflection;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace SV20T1020493.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username = "", string password = "")
        {
            ViewBag.Username = username;
            // ViewBag.Password = password;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập đủ tên và mật khẩu");
                return View();
            }
            //ktr thong tin
            var userAccount = UserAccountService.Authorize(username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }
            // dang nhap thanh cong, tao du lieu de luu cookie
            WebUserData userData = new WebUserData()
            {
                UserId = userAccount.UserID,
                UserName = userAccount.UserName,
                DisplayName = userAccount.FullName,
                Email = userAccount.Email,
                Photo = userAccount.Photo,
                ClientIP = HttpContext.Connection.RemoteIpAddress?.ToString(),
                SessionId = HttpContext.Session.Id,
                AdditionalData = "",
                Roles = userAccount.RoleNames.Split(',').ToList()
            };
           
            // thiet lam phien dang nhap cho tai khoan
            await HttpContext.SignInAsync(userData.CreatePrincipal());         
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult ChangePassword(string oldPassword, string newPassword1, string newPassword2)
        {
            if (Request.Method == "POST")
            {
                if (string.IsNullOrWhiteSpace(oldPassword)) ModelState.AddModelError("oldPassword", "Mật khẩu cũ không được để trống!");
                if (string.IsNullOrWhiteSpace(newPassword1)) ModelState.AddModelError("newPassword1", "Mật khẩu mới không được để trống!");
                if (string.IsNullOrWhiteSpace(newPassword2)) ModelState.AddModelError("newPassword2", "Nhập lại mật khẩu không được để trống!");

                if (!ModelState.IsValid) return View();

                if (newPassword1 != newPassword2)
                {
                    ModelState.AddModelError("newPassword2", "Nhập lại mật khẩu không đúng!");
                    return View();
                }

                var userEmail = HttpContext.User.FindFirst("Email")?.Value;
                bool result = UserAccountService.ChangePassword(userEmail, oldPassword, newPassword1);
                if (!result) {

                    ModelState.AddModelError("Error", "Đổi mật khẩu không thành công, có thể mật khẩu cũ chưa đúng!");
                    return View();
                }
                return RedirectToAction("index","Home");

            }
            return View();
        }


    }
}
