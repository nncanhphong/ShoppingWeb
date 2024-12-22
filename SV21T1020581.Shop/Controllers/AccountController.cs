using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020581.BusinessLayers;
using SV21T1020581.DomainModels;


namespace SV21T1020581.Shop.Controllers
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
        public async Task<IActionResult> Login(String email, String password) 
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("Error", "Nhập email và mật khẩu");
                return View();
            }

            var userAccount = UserAccountService.Authorize(UserTypes.Customer, email, password);
            if (userAccount == null)
            {
                ModelState.AddModelError("Error", "Đăng nhập thất bại");
                return View();
            }

            var userData = new WebUserData()
            {
                UserId = userAccount.UserId,
                UserName = userAccount.UserName,
                DisplayName = userAccount.DisplayName,
                Photo = userAccount.Photo,
                Roles = userAccount.RoleNames.Split(',').ToList()
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userData.CreatePrincipal());

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Đăng kí tài khoản
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var data = new Customer()
            {
                CustomerID = 0,
                IsLocked = false
            };
            return View("Edit", data);
        }

        /// <summary>
        /// Thay đổi thông tin tài khoản
        /// </summary>
        /// <returns></returns>
        public IActionResult Edit(int id = 0)
        {
            var data = CommonDataService.GetCustomer(id);
            if(data == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Customer data)
        {
            if (string.IsNullOrWhiteSpace(data.CustomerName))
            {
                ModelState.AddModelError(nameof(data.CustomerName), "Tên khách hàng không được để rỗng");
            }

            if (string.IsNullOrWhiteSpace(data.ContactName))
            {
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được rông");
            }

            if (string.IsNullOrWhiteSpace(data.Address))
            {
                ModelState.AddModelError(nameof(data.Address), "Địa chỉ không được rông");
            }

            if (string.IsNullOrWhiteSpace(data.Province))
            {
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh thành");
            }

            if (string.IsNullOrWhiteSpace(data.Phone))
            {
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            }

            if (string.IsNullOrWhiteSpace(data.Email))
            {
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email");
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (data.CustomerID == 0)
            {
                int id = CommonDataService.AddCustomer(data);

                if (id <= 0)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = CommonDataService.UpdateCustomer(data);

                if (result == false)
                {
                    ModelState.AddModelError(nameof(data.Email), "Email bị trùng");
                    return View("Edit", data);
                }

            }
            return RedirectToAction("Index", "Home");
        }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    /// <returns></returns>
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
            if (UserAccountService.Authorize(UserTypes.Employee, userName, oldPassword) == null)
            {
                ModelState.AddModelError(nameof(oldPassword), "Mật khẩu cũ không đúng. Vui lòng nhập lại");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool isChangedPassword = UserAccountService.ChangePassword(UserTypes.Employee, userName, newPassword);
            if (isChangedPassword)
            {
                return RedirectToAction("Login");
            }
            else
            {
                return View();
            }


        }
    }
}
