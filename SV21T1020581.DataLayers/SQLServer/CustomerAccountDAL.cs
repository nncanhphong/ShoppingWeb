using Dapper;
using SV21T1020581.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DataLayers.SQLServer
{
    public class CustomerAccountDAL : BaseDAL, IUserAccountDAL
    {
        public CustomerAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string userName, string password)
        {
            UserAccount? data = null;
            using (var connection = OpenConnnection())
            {
                var sql = @"select CustomerID as UserId,
                               Email as UserName,
                               CustomerName as DisplayName,
                               N'' as Photo,
                               N'' as RolesNames
                        from Customers
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
                var sql = @"update Customers
                            set Password = @Password
                            where Email = @Email";
                var parameters = new
                {
                    Email = userName,
                    Password = password
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }
    }
}
