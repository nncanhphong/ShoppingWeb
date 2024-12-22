﻿using System;
using System.Buffers;
using System.Net;
using System.Numerics;
using Dapper;
using SV21T1020581.DomainModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020581.DataLayers.SQLServer
{
    public class CustomerDAL : BaseDAL, iCommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {

        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked)
                                    values(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked)

                                    select SCOPE_IDENTITY()
                                end";
                                
                var parameter = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text); 
                connection.Close();
            }

             return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            searchValue = $"%{searchValue}%";
            using (var connection = OpenConnnection())
            {
                var sql =
                    "select count(*) from Customers where (CustomerName like @searchValue) or (ContactName like @searchValue)";
                var parameter = new {searchValue = searchValue};
                count = connection.ExecuteScalar<int>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
            }


            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnnection())
            {
                var sql = @"delete from Customers where CustomerID = @CustomerID";
                var parameter = new
                {
                    CustomerID = id
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnnection())
            {
                var sql = @"select * from Customers where CustomerID = @CustomerID";
                var parameter = new
                {
                    CustomerID = id
                };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnnection())
            {
                var sql = @"if exists (select * from Orders where CustomerID = @CustomerID)
	                            select 1
                            else
	                            select 0
                            ";
                var parameter = new
                {
                    CustomerID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {

            searchValue = $"%{searchValue}%";
            List<Customer> data = new List<Customer> ();

            using (var connection = OpenConnnection())
            {
                var sql = @"
                
                        select *
                        from(
                                select * , row_number() over(order by CustomerName) as RowNumber
                                from Customers
                                where (CustomerName like @searchValue) or (ContactName like @searchValue)
                            ) as t
                        where (@pageSize = 0)
                                or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                        order by RowNumber";
                var parameter = new
                {
                    page = page, 
                    pageSize = pageSize,
                    searchValue=searchValue

                };
                data = connection.Query<Customer>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList(); 

                connection.Close();
            }

            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using (var connection = OpenConnnection()) 
            {
                var sql = @"
                            if not exists(select * from Customers where CustomerID <> @CustomerID and Email = @Email)
                                begin
                                    update Customers
                                    set		CustomerName = @CustomerName,
		                                    ContactName = @ContactName,
		                                    Province = @Province,
		                                    Address = @Address,
		                                    Phone = @Phone,
		                                    Email = @Email,
		                                    IsLocked = @IsLocked
                                    where CustomerID = @CustomerID
                                end
                            ";
                var parameter = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked,
                    CustomerID = data.CustomerID
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;

                connection.Close();
            }


            return result;

        }
    }
}
