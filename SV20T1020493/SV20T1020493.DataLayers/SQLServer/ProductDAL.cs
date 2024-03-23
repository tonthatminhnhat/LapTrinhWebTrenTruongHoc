using Azure;
using Dapper;
using SV20T1020493.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.DataLayers.SQLServer
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where ProductName = @ProductName)
                                select -1
                            else
                                begin
                                    insert into Products(ProductName,ProductDescription,SupplierID,CategoryID,Unit,Price,Photo,IsSelling)
                                    values(@ProductName,@ProductDescription,@SupplierID,@CategoryID,@Unit,@Price,@Photo,@IsSelling);

                                    select @@identity;
                                end";
                var parameters = new
                {
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling=data.IsSelling,
                };
                id = connection.ExecuteScalar<int>(sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddAttribute(ProductAttribute data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductAttributes where AttributeName = @AttributeName
                                             and ProductID = @ProductID)
                                select -1
                            else
                                begin
                                    insert into ProductAttributes(ProductID,AttributeName,AttributeValue,DisplayOrder)
                                    values(@ProductID,@AttributeName,@AttributeValue,@DisplayOrder);

                                    select @@identity;
                                end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                };
                id = connection.ExecuteScalar<int>(sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public long AddPhoto(ProductPhoto data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from ProductPhotos where Photo = @Photo and ProductID=@ProductID)
                                select -1
                            else
                                begin
                                    insert into ProductPhotos(ProductID,Photo,Description,DisplayOrder,IsHidden)
                                    values(@ProductID,@Photo,@Description,@DisplayOrder,@IsHidden);

                                    select @@identity;
                                end";
                var parameters = new
                {
                    ProductID = data.ProductID ,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden,
                };
                id = connection.ExecuteScalar<int>(sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Products 
                            where (@searchValue = N'' or ProductName like @searchValue)
                                 and (@CategoryID = 0 or CategoryID = @CategoryID)
                                 and (@SupplierID = 0 or SupplierID = @SupplierID)
                                 and (Price >= @MinPrice)
                                 and (@MaxPrice <= 0 or Price <= @MaxPrice)";
                var parameters = new { 
                    searchValue = searchValue ?? "" ,
                    CategoryID= categoryID,
                    SupplierID= supplierID,
                    MinPrice= minPrice,
                    MaxPrice= maxPrice
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Products where ProductID = @ProductID";
                var parameters = new{ ProductID = id};
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    
        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new { AttributeID = attributeID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new { PhotoID = photoID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Product? Get(int id)
        {
            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Products where ProductID = @ProductID";
                var parameters = new
                {
                    ProductID = id
                };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductAttribute? GetAttribute(long attributeID)
        {
            ProductAttribute? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductAttributes where AttributeID = @AttributeID";
                var parameters = new
                {
                    AttributeID = attributeID
                };
                data = connection.QueryFirstOrDefault<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public ProductPhoto? GetPhoto(long photoID)
        {
            ProductPhoto? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from ProductPhotos where PhotoID = @PhotoID";
                var parameters = new
                {
                    PhotoID = photoID
                };
                data = connection.QueryFirstOrDefault<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from OrderDetails where ProductID = @ProductID)
                            OR EXISTS(SELECT * FROM ProductAttributes WHERE ProductID = @ProductID)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    ProductID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return result;
        }

        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> list = new List<Product>();
            if (!string.IsNullOrEmpty(searchValue))

                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                            (
	                            select	*, row_number() over (order by ProductName) as RowNumber
	                            from	Products WITH(INDEX(IX_ProductName))
	                            where	(@searchValue = N'' or ProductName like @searchValue)
                                   and (@CategoryID = 0 or CategoryID = @CategoryID)
                                   and (@SupplierID = 0 or SupplierID = @SupplierID)
                                   and (Price >= @MinPrice)
                                   and (@MaxPrice <= 0 or Price <= @MaxPrice)
                            )
                            select * from cte
                           where  (@pageSize = 0) 
                           or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                          order by RowNumber";
                var parameters = new
                {
                    page = page, pageSize = pageSize,
                    searchValue = searchValue ?? "",
                    CategoryID=categoryID, SupplierID=supplierID,
                    MinPrice=minPrice, MaxPrice=maxPrice
                };
                list = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();

            }
            return list;
        }

        public IList<Product> ListAll(string searchValue = "")
        {
            List<Product> list = new List<Product>();
            if (!string.IsNullOrEmpty(searchValue))

                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"
                             SELECT * FROM Products WITH(INDEX(IX_ProductName))
                              where	(@searchValue = N'') or (ProductName like @searchValue) 
                              ORDER BY ProductName;
	                           
                            ";
                var parameters = new
                {
                    searchValue = searchValue ?? ""
                };
                list = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();

            }
            return list;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> list = new List<ProductAttribute>();
          
            using (var connection = OpenConnection())
            {
                var sql = @"
	                            select	*
	                            from	ProductAttributes
	                            where	ProductID = @ProductID  
                                order by DisplayOrder
                            ";
                var parameters = new{ ProductID = productID  };
                list = connection.Query<ProductAttribute>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> list = new List<ProductPhoto>();

            using (var connection = OpenConnection())
            {
                var sql = @"
	                            select	*
	                            from	ProductPhotos
	                            where	ProductID = @ProductID  
                                order by DisplayOrder
                            ";
                var parameters = new { ProductID = productID };
                list = connection.Query<ProductPhoto>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public bool Update(Product data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Products where ProductID <> @ProductID and ProductName = @ProductName)
                                begin
                                    update Products 
                                    set ProductName = @ProductName,
                                        ProductDescription = @ProductDescription,
                                        SupplierID = @SupplierID,
                                        CategoryID = @CategoryID,
                                        Unit = @Unit,
                                        Price = @Price,
                                        Photo = @Photo,
                                        IsSelling=@IsSelling
                                    where ProductID = @ProductID end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    ProductDescription = data.ProductDescription ?? "",
                    SupplierID = data.SupplierID,
                    CategoryID = data.CategoryID,
                    Unit = data.Unit ?? "",
                    Price = data.Price,
                    Photo = data.Photo ?? "",
                    IsSelling = data.IsSelling
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from ProductAttributes where ProductID = @ProductID 
                              and AttributeID <> @AttributeID and AttributeName = @AttributeName)
                                begin
                                    update ProductAttributes 
                                    set AttributeName = @AttributeName,
                                        AttributeValue = @AttributeValue,
                                        DisplayOrder = @DisplayOrder
                                    where AttributeID = @AttributeID end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    AttributeID = data.AttributeID,
                    AttributeName = data.AttributeName ?? "",
                    AttributeValue = data.AttributeValue ?? "",
                    DisplayOrder = data.DisplayOrder
                 
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from ProductPhotos where ProductID = @ProductID 
                              and PhotoID <> @PhotoID and Photo = @Photo)
                                begin
                                    update ProductPhotos 
                                    set Photo = @Photo,
                                        Description = @Description,
                                        DisplayOrder = @DisplayOrder,
                                        IsHidden = @IsHidden
                                    where PhotoID = @PhotoID end";
                var parameters = new
                {
                    ProductID = data.ProductID,
                    PhotoID = data.PhotoID,
                    Photo = data.Photo ?? "",
                    Description = data.Description ?? "",
                    DisplayOrder = data.DisplayOrder,
                    IsHidden = data.IsHidden
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }

            return result;
        }
    }
}
