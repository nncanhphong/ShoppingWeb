using Dapper;
using SV21T1020581.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DataLayers.SQLServer
{
    public class EmployeeDAL : BaseDAL, iCommonDAL<Employee>
    {
        public EmployeeDAL(string connectionString) : base(connectionString) { }

        public int Add(Employee data)
        {
            int id = 0;
            using (var connection = OpenConnnection())
            {
                var sql = @"
                            if exists(select * from Employees where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Employees(FullName, BirthDate, Address, Phone, Email, Password, Photo, IsWorking)
                                    values (@FullName, @BirthDate, @Address, @Phone, @Email, @Password, @Photo, @IsWorking)
                            
                                    select SCOPE_IDENTITY()
                                end
                            ";
                var parameter = new
                {
                    FullName = data.FullName,
                    BirthDate = data.BirthDate,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    Password = data.Password,
                    Photo = data.Photo,
                    IsWorking = data.IsWorking
                };

                id = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
            }
            return id;
        }

        public int Count(string SearchValue = "")
        {
            int count = 0;
            SearchValue = $"%{SearchValue}%";
            using (var connection = OpenConnnection())
            {
                var sql =
                    "select count(*) from Employees where (FullName like @searchValue)";
                var parameter = new { searchValue = SearchValue };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
            }


            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnnection())
            {
                var sql = @"delete from Employees where EmployeeID = @EmployeeID";
                var parameter = new
                {
                    EmployeeID = id
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

   
        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnnection())
            {
                var sql = @"if exists (select * from Orders where EmployeeID = @EmployeeID)
	                select 1
                else
	                select 0
                ";
                var parameter = new
                {
                    EmployeeID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            searchValue = $"%{searchValue}%";
            List<Employee> data = new List<Employee>();

            using (var connection = OpenConnnection())
            {
                var sql = @"
                
                        select *
                        from(
                                select * , row_number() over(order by FullName) as RowNumber
                                from Employees where (FullName like @searchValue)
                            ) as t
                        where (@pageSize = 0)
                                or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                        order by RowNumber";
                var parameter = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue

                };
                data = connection.Query<Employee>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();

                connection.Close();
            }

            return data;
        }



        public bool Update(Employee data)
        {
            bool result = false;
            using (var connection = OpenConnnection())
            {
                var sql = @"
                            if not exists(select * from Employees where EmployeeID <> @EmployeeID and Email = @Email)
                                begin
                                    update Employees
                                    set FullName = @FullName,
	                                    BirthDate = @BirthDate,
	                                    Address = @Address,
	                                    Phone = @Phone, 
	                                    Email = @Email, 
	                                    Password = @Password, 
	                                    Photo = @Photo, 
	                                    IsWorking = @IsWorking
                                    where EmployeeID = @EmployeeID
                                end";
                var parameter = new
                {
                    EmployeeID = data.EmployeeID,
                    FullName = data.FullName,
                    BirthDate = data.BirthDate,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    Password = data.Password,
                    Photo = data.Photo,
                    IsWorking = data.IsWorking
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;

                connection.Close();
            }


            return result;
        }

        Employee? iCommonDAL<Employee>.Get(int id)
        {

            Employee? data = null;
            using (var connection = OpenConnnection())
            {
                var sql = @"select * from Employees where EmployeeID = @EmployeeID";
                var parameter = new
                {
                    EmployeeID = id
                };
                data = connection.QueryFirstOrDefault<Employee>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }
    }
}
