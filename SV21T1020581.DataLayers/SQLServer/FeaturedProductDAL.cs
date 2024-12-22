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
    public class FeaturedProductDAL : BaseDAL, iAnalysProductDAL<FeaturedProduct>
    {
        public FeaturedProductDAL(string connectionString) : base(connectionString)
        {
        }

        public List<FeaturedProduct> Featured()
        {
            List<FeaturedProduct> data = new List<FeaturedProduct>();
            using (var connection = OpenConnnection())
            {
                var sql = @"
                            WITH ProductSales AS (
                                SELECT 
                                    p.ProductID,
                                    p.CategoryID,
                                    p.Price,
                                    p.ProductName,
                                    p.Photo,
                                    SUM(od.Quantity) AS TotalSales,
                                    SUM(od.Quantity * p.Price) AS TotalRevenue -- Calculate total revenue using Price
                                FROM 
                                    Products p
                                INNER JOIN 
                                    OrderDetails od ON p.ProductID = od.ProductID
                                GROUP BY 
                                    p.ProductID, p.CategoryID, p.Price, p.ProductName, p.Photo
                            ),

                            CategorySales AS (
                                SELECT 
                                    ps.CategoryID,
                                    SUM(ps.TotalRevenue) AS CategoryTotalRevenue
                                FROM 
                                    ProductSales ps
                                GROUP BY 
                                    ps.CategoryID
                            ),

                            TopCategories AS (
                                SELECT TOP 4
                                    cs.CategoryID
                                FROM 
                                    CategorySales cs
                                ORDER BY 
                                    cs.CategoryTotalRevenue DESC
                            ),
                            RankedProducts AS (
                                SELECT 
                                    ps.Photo,
                                    ps.ProductID,
                                    ps.CategoryID,
                                    ps.ProductName,
                                    ps.Price,
                                    ps.TotalSales,
                                    ps.TotalRevenue,
                                    ROW_NUMBER() OVER (PARTITION BY ps.CategoryID ORDER BY ps.TotalRevenue DESC) AS RankInCategory
                                FROM 
                                    ProductSales ps
                                WHERE 
                                    ps.CategoryID IN (SELECT CategoryID FROM TopCategories)
                            )

                            SELECT 
                                rp.Photo,
	                            rp.CategoryID,
	                            rp.ProductID,
                                c.CategoryName,
                                rp.ProductName,
                                rp.Price
                            FROM 
                                RankedProducts rp
                            INNER JOIN 
                                Categories c ON rp.CategoryID = c.CategoryID
                            WHERE 
                                rp.RankInCategory <= 4
                            ORDER BY 
                                c.CategoryName, rp.TotalRevenue DESC

                            ";
                var paremeter = new { };
                data = connection.Query<FeaturedProduct>(sql: sql, param: paremeter, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public List<FeaturedProduct> HighRate()
        {
            List<FeaturedProduct> data = new List<FeaturedProduct>();
            using (var connection = OpenConnnection())
            {
                var sql = @"
                            select top 6 c.CategoryID, p.ProductID, p.Photo, c.CategoryName, p.ProductName, p.Price
                            from Products p join Categories c on p.CategoryID = c.CategoryID join OrderDetails od on od.ProductID = p.ProductID order by Quantity desc
                            ";
                var paremeter = new { };
                data = connection.Query<FeaturedProduct>(sql: sql, param: paremeter, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public List<FeaturedProduct> List()
        {
            List<FeaturedProduct> data = new List<FeaturedProduct>();
            using(var connection = OpenConnnection())
            {
                var sql = @"
                            WITH ProductSales AS (
                                SELECT 
                                    p.ProductID,
                                    p.CategoryID,
                                    p.Price,
                                    p.ProductName,
                                    p.Photo,
                                    SUM(od.Quantity) AS TotalSales,
                                    SUM(od.Quantity * p.Price) AS TotalRevenue -- Calculate total revenue using Price
                                FROM 
                                    Products p
                                INNER JOIN 
                                    OrderDetails od ON p.ProductID = od.ProductID
                                GROUP BY 
                                    p.ProductID, p.CategoryID, p.Price, p.ProductName, p.Photo
                            ),

                            CategorySales AS (
                                SELECT 
                                    ps.CategoryID,
                                    SUM(ps.TotalRevenue) AS CategoryTotalRevenue
                                FROM 
                                    ProductSales ps
                                GROUP BY 
                                    ps.CategoryID
                            ),

                            TopCategories AS (
                                SELECT TOP 4
                                    cs.CategoryID
                                FROM 
                                    CategorySales cs
                                ORDER BY 
                                    cs.CategoryTotalRevenue DESC
                            ),
                            RankedProducts AS (
                                SELECT 
                                    ps.Photo,
                                    ps.ProductID,
                                    ps.CategoryID,
                                    ps.ProductName,
                                    ps.Price,
                                    ps.TotalSales,
                                    ps.TotalRevenue,
                                    ROW_NUMBER() OVER (PARTITION BY ps.CategoryID ORDER BY ps.TotalRevenue DESC) AS RankInCategory
                                FROM 
                                    ProductSales ps
                                WHERE 
                                    ps.CategoryID IN (SELECT CategoryID FROM TopCategories)
                            )

                            SELECT 
                                rp.Photo,
	                            rp.CategoryID,
	                            rp.ProductID,
                                c.CategoryName,
                                rp.ProductName,
                                rp.Price
                            FROM 
                                RankedProducts rp
                            INNER JOIN 
                                Categories c ON rp.CategoryID = c.CategoryID
                            WHERE 
                                rp.RankInCategory <= 4
                            ORDER BY 
                                c.CategoryName, rp.TotalRevenue DESC

                            ";
                var paremeter = new { };
                data = connection.Query<FeaturedProduct>(sql: sql, param: paremeter, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public List<FeaturedProduct> New()
        {
            List<FeaturedProduct> data = new List<FeaturedProduct>();
            using(var connection = OpenConnnection())
            {
                var sql = @"select top 6 c.CategoryID, p.ProductID, p.Photo, c.CategoryName, p.ProductName, p.Price
                            from Products p join Categories c on p.CategoryID = c.CategoryID 
                            order by ProductID desc";
                var paremeter = new { };
                data = connection.Query<FeaturedProduct>(sql: sql, param: paremeter, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
    }
}
