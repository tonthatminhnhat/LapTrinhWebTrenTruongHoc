using SV20T1020493.DataLayers;
using SV20T1020493.DataLayers.SQLServer;
using SV20T1020493.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.BusinessLayers
{
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;

        static ProductDataService() {
            productDB = new ProductDAL(Configuration.ConnectionString); 
        }

        public static List<Product> ListOfProducts(string searchValue = "")
        {
            return productDB.ListAll(searchValue).ToList();
        }

        public static List<Product> ListOfProducts(out int rowCount,int page=1,int pageSize=0,string searchValue="",int categoryID=0,int supplierID=0,decimal minPrice=0,decimal maxPrice=0)
        {
            rowCount = productDB.Count(searchValue, categoryID, supplierID, minPrice, maxPrice);
            return productDB.List(page,pageSize,searchValue,categoryID,supplierID,minPrice,maxPrice).ToList();              
        }

        public static Product? GetProduct(int id)
        {
            return productDB.Get(id);
        }
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }
        public static bool DeleteProduct(int id)
        {
            if (productDB.IsUsed(id))
                return false;
            return productDB.Delete(id);
        }
        public static bool IsUsedProduct(int id)
        {
            return productDB.IsUsed(id);
        }
        //================================================

        public static List<ProductPhoto> ListOfPhotos(int id)
        {
            return productDB.ListPhotos(id).ToList();
        }
        public static ProductPhoto? GetPhoto(int id)
        {
            return productDB.GetPhoto(id);
        }
        public static long AddPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }
        public static bool UpdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }
        public static bool DeletePhoto(int id)
        {
            return productDB.DeletePhoto(id);
        }
        //================================================

        public static List<ProductAttribute> ListOfAttributes(int id)
        {
            return productDB.ListAttributes(id).ToList();
        }
        public static ProductAttribute? GetAttribute(int id)
        {
            return productDB.GetAttribute(id);
        }
        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }
        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }
        public static bool DeleteAttribute(int id)
        {
            return productDB.DeleteAttribute(id);
        }










    }
}
