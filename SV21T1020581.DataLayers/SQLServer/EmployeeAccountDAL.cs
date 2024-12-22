using Dapper;
using SV21T1020581.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DataLayers.SQLServer
{
    public class EmployeeAccountDAL : BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string userName, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnnection())
            {
                var sql = @"select EmployeeID as UserId,
                               Email as UserName,
                               FullName as DisplayName,
                               Photo,
                               RoleNames
                        from Employees
                        where Email = @Email and Password = @Password";
                var parameters = new
                {
                    Email = userName,
                    Password = password
                };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            }
            return data;
        }

        public bool ChangePassword(string userName, string password)
        {
            bool result = false;
            using (var connection = OpenConnnection())
            {
                var sql = @"update Employees
                            set Password = @Password
                            where Email = @Email";
                var parameters = new {
                    Email = userName,
                    Password = password
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }
    }
}
