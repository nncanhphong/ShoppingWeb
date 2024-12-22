using Dapper;
using SV21T1020581.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21T1020581.DataLayers.SQLServer
{
    public class ShipperDAL : BaseDAL, iCommonDAL<Shipper>
    {
        public ShipperDAL(string connectionString) : base(connectionString) { }

        public int Add(Shipper data)
        {
            int id = 0;
            using (var connection = OpenConnnection())
            {
                var sql = @"
                            if exists(select * from Shippers where Phone = @Phone)
                                select -1
                            else
                                begin
                                    insert into Shippers(ShipperName, Phone)
                                    values(@ShipperName, @Phone)

                                    select SCOPE_IDENTITY()
                                end
                            ";
                var parameter = new
                {
                    ShipperName = data.ShipperName,
                    Phone = data.Phone
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
                    "select count(*) from Shippers where (ShipperName like @searchValue)";
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
                var sql = @"delete from Shippers where ShipperID = @ShipperID";
                var parameter = new
                {
                    ShipperID = id
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
                var sql = @"if exists (select * from Orders where ShipperID = @ShipperID)
	                select 1
                else
	                select 0
                ";
                var parameter = new
                {
                    ShipperID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public List<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            searchValue = $"%{searchValue}%";
            List<Shipper> data = new List<Shipper>();

            using (var connection = OpenConnnection())
            {
                var sql = @"
                
                        select *
                        from(
                                select * , row_number() over(order by ShipperName) as RowNumber
                                from Shippers where (ShipperName like @searchValue)
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
                data = connection.Query<Shipper>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text).ToList();

                connection.Close();
            }

            return data;
        }



        public bool Update(Shipper data)
        {
            bool result = false;
            using (var connection = OpenConnnection())
            {
                var sql = @"
                            if not exists(select * from Shippers where ShipperID <> @ShipperID and Phone = @Phone)
                                begin
                                    update Shippers
                                    set ShipperName = @ShipperName,
	                                    Phone = @Phone
                                    where ShipperID = @ShipperID
                                end
                            ";
                            
                var parameter = new
                {
                    ShipperID = data.ShipperID,
                    ShipperName = data.ShipperName,
                    Phone = data.Phone
                };
                result = connection.Execute(sql: sql, param: parameter, commandType: System.Data.CommandType.Text) > 0;

                connection.Close();
            }


            return result;
        }

        Shipper? iCommonDAL<Shipper>.Get(int id)
        {
            Shipper? data = null;
            using (var connection = OpenConnnection())
            {
                var sql = @"select * from Shippers where ShipperID = @ShipperID";
                var parameter = new
                {
                    ShipperID = id
                };
                data = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return data;
        }

    }
}
