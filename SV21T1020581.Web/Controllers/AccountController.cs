using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020581.BusinessLayers;

namespace SV21T1020581.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private const string USER_DATA = "UserData";


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            ViewBag.Username = username;

            //Kiểm tra thông tin đầu vào
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập tên và mật khẩu");
                return View();
            }

            ////TODO: Kiểm tra xem username và password (của Employee) có đúng không?
            //if (username != "admin")
            //{
            //    ModelState.AddModelError("Error", "Đăng nhập thât bại");
            //    return View();
            //}
            var userAccount = UserAccountService.Authorize(UserTypes.Employee, username, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            //Đăng nhập thành công
            //1. Tạo thông tin của người dùng
            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            //2. Ghi nhận trạng thái đăng nhập
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

            //3. Quay về trang chủ
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(string userName, string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError(nameof(confirmPassword), "Mật khẩu không trùng khớp. Vui lòng nhập lại");
            }
            if(UserAccountService.Authorize(UserTypes.Employee, userName, oldPassword) == null)
            {
                ModelState.AddModelError(nameof(oldPassword), "Mật khẩu cũ không đúng. Vui lòng nhập lại");
            }
            if(!ModelState.IsValid)
            {
                return View();
            }
            bool isChangedPassword = UserAccountService.ChangePassword(UserTypes.Employee,userName,newPassword);
            if(isChangedPassword)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }
            
            
        }
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
