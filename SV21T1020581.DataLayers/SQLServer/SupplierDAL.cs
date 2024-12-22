using Dapper;
using SV21T1020581.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020581.DataLayers.SQLServer
{
    public class SupplierDAL : BaseDAL, iCommonDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnnection())
            {
                var sql = @"
                            if exists(select * from Suppliers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Suppliers(SupplierName, ContactName, Provice, Address, Phone, Email)
                                    values(@SupplierName, @ContactName, @Provice, @Address, @Phone, @Email)
                            
                                    select SCOPE_IDENTITY()
                                end
                            ";
                var parameter = new
                {
                    SupplierName = data.SupplierName,
                    ContactName = data.ContactName,
                    Provice = data.Provice,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email
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
                    "select * from Suppliers where (SupplierName like @searchValue)";
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
                var sql = @"delete from Suppliers where SupplierID = @SupplierID";
                var parameter = new
                {
                    SupplierID = id
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;
            }
            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var connection = OpenConnnection())
            {
                var sql = @"select * from Suppliers where SupplierID = @SupplierID";
                var parameter = new
                {
                    SupplierID = id
                };
                data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnnection())
            {
                var sql = @"if exists (select * from Products where SupplierID = @SupplierID)
	                select 1
                else
	                select 0
                ";
                var parameter = new
                {
                    SupplierID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            searchValue = $"%{searchValue}%";
            List<Supplier> data = new List<Supplier>();

            using (var connection = OpenConnnection())
            {
                var sql = @"
                
                        select *
                        from(
                                select * , row_number() over(order by SupplierName) as RowNumber
                                from Suppliers where (SupplierName like @searchValue) or (ContactName like @searchValue)
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
                data = connection.Query<Supplier>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();

                connection.Close();
            }

            return data;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnnection())
            {
                var sql = @"
                            if not exists(select * from Suppliers where SupplierID <> @SupplierID and Email = @Email)
                                begin
                                    update Suppliers
                                    set	SupplierName = @SupplierName,
	                                    ContactName = @ContactName,
	                                    Provice = @Provice,
	                                    Address = @Address,
	                                    Phone = @Phone,
	                                    Email = @Email
                                    where SupplierID = @SupplierID
                                end";

                var parameter = new
                {
                    SupplierName = data.SupplierName,
                    ContactName = data.ContactName,
                    Provice = data.Provice,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    SupplierID = data.SupplierID
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;

                connection.Close();
            }


            return result;
        }
    }
}
