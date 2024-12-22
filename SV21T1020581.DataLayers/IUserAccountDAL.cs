﻿using SV21T1020581.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DataLayers
{
    public interface IUserAccountDAL
    {
        /// <summary>
        /// Xác thực tài khoản đăng nhập của người dùng
        /// Hàm trả về thông tin tài khoản nếu xác thực thành công,
        /// ngược lại hàm trả về null
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount? Authorize(string userName, string password);
        /// <summary>
        /// Đổi mật khẩu
        /// </summary>
        bool ChangePassword(string userName, string password);
    }
}
