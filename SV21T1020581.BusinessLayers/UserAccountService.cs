using SV21T1020581.DataLayers.SQLServer;
using SV21T1020581.DataLayers;
using SV21T1020581.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountBD;
        private static readonly IUserAccountDAL customerAccountBD;
        static UserAccountService()
        {
            string connectionString = Configuration.ConnectionString;
            employeeAccountBD = new EmployeeAccountDAL(Configuration.ConnectionString);
            customerAccountBD = new CustomerAccountDAL(Configuration.ConnectionString);
        }
        public static UserAccount? Authorize(UserTypes userTypes, string username, string password)
        {
            if (userTypes == UserTypes.Employee)
                return employeeAccountBD.Authorize(username, password);
            else
                return customerAccountBD.Authorize(username, password);
        }
        public static bool ChangePassword(UserTypes userTypes, string username, string password)
        {
            if(userTypes == UserTypes.Employee)
                return employeeAccountBD.ChangePassword(username, password);
            else
                return customerAccountBD.ChangePassword(username, password);
        }
    }
    /// <summary>
    /// Loại tài khoản
    /// </summary>
    public enum UserTypes
    {
        Employee,
        Customer,
    }
}
