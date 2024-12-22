using SV21T1020581.DataLayers.SQLServer;
using SV21T1020581.DataLayers;
using SV21T1020581.DomainModels;
using Dapper;

public class ProductDAL : BaseDAL, IProductDAL
{
    public ProductDAL(string connectionString) : base(connectionString)
    {

    }
    public int Add(Product data)
    {
        int id = 0;
        using (var connection = OpenConnnection())
        {
            var sql = @"if exists(select * from Products where ProductName = @ProductName)
                            select -1
                        else
                            begin
                                insert into Products(CategoryID, SupplierID, ProductName, ProductDescription, Unit, Price, Photo, IsSelling)
                                values (@CategoryID, @SupplierID, @ProductName, @ProductDescription, @Unit, @Price, @Photo, @IsSelling);
                                select SCOPE_IDENTITY();
                            end";
            var paramenters = new
            {
                CategoryID = data.CategoryID,
                SupplierID = data.SupplierID,
                ProductName = data.ProductName ?? "",
                ProductDescription = data.ProductDescription ?? "",
                Unit = data.Unit ?? "",
                Price = data.Price,
                Photo = data.Photo ?? "",
                IsSelling = data.IsSelling
            };
            id = connection.ExecuteScalar<int>(sql: sql, param: paramenters, commandType: System.Data.CommandType.Text);
            connection.Close();
        }
        return id;
    }
    public long AddAttribute(ProductAttribute data)
    {
        int id = 0;
        using (var connection = OpenConnnection())
        {
            var sql = @"if exists(select * from ProductAttributes where ProductID = @ProductID and DisplayOrder = @DisplayOrder)
                                select -1
                            else
                                begin
                                    insert into ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                                    values (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                                    select SCOPE_IDENTITY();
                                end";
            var paramenters = new
            {
                ProductID = data.ProductID,
                AttributeName = data.AttributeName ?? "",
                AttributeValue = data.AttributeValue ?? "",
                DisplayOrder = data.DisplayOrder,
            };
            id = connection.ExecuteScalar<int>(sql: sql, param: paramenters, commandType: System.Data.CommandType.Text);
            connection.Close();
        }
        return id;
    }
    public long AddPhoto(ProductPhoto data)
    {
        int id = 0;
        using (var connection = OpenConnnection())
        {
            var sql = @"if exists (select * from ProductPhotos where ProductID = @ProductID and DisplayOrder = @DisplayOrder)
                                select -1
                            else
                                begin
                                insert into ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                                values (@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                                select SCOPE_IDENTITY();
                            end";
            var paramenters = new
            {
                ProductID = data.ProductID,
                Photo = data.Photo ?? "",
                Description = data.Description ?? "",
                DisplayOrder = data.DisplayOrder,
                IsHidden = data.IsHidden
            };
            id = connection.ExecuteScalar<int>(sql: sql, param: paramenters, commandType: System.Data.CommandType.Text);
            connection.Close();
        }
        return id;
    }
    public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
    {
        int count = 0;
        searchValue = $"%{searchValue}%";

        using (var connection = OpenConnnection())
        {
            var sql = @"select count(*)
                from Products
                where (ProductName like @searchValue)
                    and (@CategoryID = 0 or CategoryID = @CategoryID)
                    and (@SupplierID = 0 or SupplierID = @SupplierID)
                    and (Price >= @MinPrice)
                    and (@MaxPrice <= 0 or Price <= @MaxPrice)";
            var parameters = new
            {
                searchValue = searchValue,
                categoryID = categoryID,
                supplierID = supplierID,
                minPrice = minPrice,
                maxPrice = maxPrice
            };
            count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            connection.Close();
        }
        return count;
    }
    public bool Delete(int productID)
    {
        bool result = false;
        using (var connection = OpenConnnection())
        {
            var sql = @"delete from Products where ProductID = @ProductID";
            var parameters = new
            {
                productID = productID
            };
            result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            connection.Close();
        }
        return result;
    }
    public bool DeleteAttribute(long attributeID)
    {
        bool result = false;
        using (var connection = OpenConnnection())
        {
            var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
            var parameters = new
            {
                attributeID = attributeID
            };
            result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            connection.Close();
        }
        return result;
    }
    public bool DeletePhoto(long photoID)
    {
        bool result = false;
        using (var connection = OpenConnnection())
        {
            var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
            var parameters = new
            {
                photoID = photoID
            };
            result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            connection.Close();
        }
        return result;
    }
    public Product? Get(int productID)
    {
        Product? data = null;
        using (var connection = OpenConnnection())
        {
            var sql = @"select * from Products where ProductID = @ProductID";
            var paraments = new
            {
                productID = productID
            };
            data = connection.QueryFirstOrDefault<Product>(sql: sql, param: paraments, commandType: System.Data.CommandType.Text);
            connection.Close();

        }
        return data;
    }
    public ProductAttribute? GetAttribute(long attributeID)
    {
        ProductAttribute? data = null;
        using (var connection = OpenConnnection())
        {
            var sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
            var paraments = new
            {
                attributeID = attributeID
            };
            data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: paraments, commandType: System.Data.CommandType.Text);
            connection.Close();
        }
        return data;
    }
    public ProductPhoto? GetPhoto(long photoID)
    {
        ProductPhoto? data = null;
        using (var connection = OpenConnnection())
        {
            var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
            var paraments = new
            {
                photoID = photoID
            };
            data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: paraments, commandType: System.Data.CommandType.Text);
            connection.Close();
        }
        return data;
    }
    public bool InUsed(int productID)
    {
        bool results = false;
        using (var connection = OpenConnnection())
        {
            var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
                            select 1
                        else
                            select 0";
            var parameters = new
            {
                productID = productID
            };
            results = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
            connection.Close();
        }
        return results;
    }
    public List<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
    {
        List<Product> data = new List<Product>();
        searchValue = $"%{searchValue}%"; // Tìm kiếm tương đối với LIKE
        using (var connection =OpenConnnection())
        {
            var sql = @"SELECT *
                        FROM (
                                SELECT *,
                                ROW_NUMBER() OVER(ORDER BY ProductName) AS RowNumber
                                FROM Products
                                WHERE (@SearchValue = N'' OR ProductName LIKE @SearchValue)
                                AND (@CategoryID = 0 OR CategoryID = @CategoryID)
                                AND (@SupplierID = 0 OR SupplierId = @SupplierID)
                                AND (Price >= @MinPrice)
                                AND (@MaxPrice <= 0 OR Price <= @MaxPrice)
                                ) AS t
                        WHERE (@PageSize = 0)
                        OR (RowNumber BETWEEN (@Page - 1)*@PageSize + 1 AND @Page * @PageSize)";
            var parameters = new
            {
                page = page,
                pageSize = pageSize,
                searchValue = searchValue,
                categoryID = categoryID,
                supplierID = supplierID,
                minPrice = minPrice,
                maxPrice = maxPrice
            };
            data = connection.Query<Product>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            connection.Close();
        }
        return data;
    }
    public IList<ProductAttribute> ListAttributes(int productID)
    {
        List<ProductAttribute> data = new List<ProductAttribute>();
        using (var connection = OpenConnnection())
        {
            var sql = @"select * from ProductAttributes where ProductID = @ProductID";
            var parameters = new { ProductID = productID };
            data = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            connection.Close();
        }
        return data;
    }
    public IList<ProductPhoto> ListPhotos(int productID)
    {
        List<ProductPhoto> data = new List<ProductPhoto>();
        using (var connection = OpenConnnection())
        {
            var sql = @"select * from ProductPhotos where ProductID = @ProductID";
            var parameters = new { ProductID = productID };
            data = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
            connection.Close();
        }
        return data;
    }
    public bool Update(Product data)
    {
        bool result = false;
        using (var connection = OpenConnnection())
        {
            var sql = @"if not exists (select * from Products where ProductID <> @ProductID)
                    begin
                           update Products
                           set ProductName = @ProductName,
                           ProductDescription = @ProductDescription,
                           SupplierID = @SupplierID,
                           CategoryID = @CategoryID,
                           Unit = @Unit,
                           Price = @Price,
                           Photo = @Photo,
                           IsSelling = @IsSelling
                           WHERE ProductID = @ProductID
                    end";
            var parameters = new
            {
                CategoryID = data.CategoryID,
                SupplierID = data.SupplierID,
                ProductName = data.ProductName ?? "",
                ProductDescription = data.ProductDescription ?? "",
                Unit = data.Unit ?? "",
                Price = data.Price,
                Photo = data.Photo ?? "",
                IsSelling = data.IsSelling,
                ProductID = data.ProductID
            };
            result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            connection.Close();
        }
        return result;
    }
    public bool UpdateAttribute(ProductAttribute data)
    {
        bool result = false;
        using (var connection = OpenConnnection())
        {
            var sql = @"if exists(select * from ProductAttributes where AttributeID = @AttributeID and DisplayOrder = @DisplayOrder)
                            select -1
                        else
                            begin
                                update ProductAttributes
                                set AttributeName = @AttributeName,
                                    AttributeValue = @AttributeValue,
                                    DisplayOrder = @DisplayOrder
                                where AttributeID = @AttributeID
                            end";
            var parameters = new
            {
                ProductID = data.ProductID,
                AttributeName = data.AttributeName ?? "",
                AttributeValue = data.AttributeValue ?? "",
                DisplayOrder = data.DisplayOrder,
                AttributeID = data.AttributeID
            };
            result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            connection.Close();
        }
        return result;
    }

    public bool UpdatePhoto(ProductPhoto data)
    {
        bool result = false;
        using (var connection = OpenConnnection())
        {
            var sql = @"if exists(select * from ProductPhotos where PhotoID = @PhotoID and DisplayOrder = @DisplayOrder)
                                select -1
                            else
                                begin
                                    update ProductPhotos
                                    set Photo = @Photo,
                                        Description = @Description,
                                        DisplayOrder = @DisplayOrder,
           IsHidden = @IsHidden
                                    where PhotoID = @PhotoID
                                end";
            var parameters = new
            {
                ProductID = data.ProductID,
                Photo = data.Photo ?? "",
                Description = data.Description ?? "",
                DisplayOrder = data.DisplayOrder,
                IsHidden = data.IsHidden,
                PhotoID = data.PhotoID
            };
            result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
            connection.Close();
        }
        return result;
    }
}