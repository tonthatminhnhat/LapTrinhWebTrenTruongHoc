using SV20T1020493.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.DataLayers
{
    public interface IProductDAL
    {
        //Tìm kiếm và lấy ds mặt hàng dưới dạng phân trang và không phân trang
        IList<Product> ListAll(string searchValue = "");

        IList<Product> List(int page = 1, int pageSize = 0, string searchValue = ""
            , int categoryID = 0, int supplierID = 0, 
            decimal minPrice = 0, decimal maxPrice = 0);
        //Đếm số lượng mặt hàng tìm được
        int Count(string searchValue = "", int categoryID = 0, int supplierID = 0,
            decimal minPrice = 0, decimal maxPrice = 0);
        //Lấy thông tin mặt hàng theo mã hàng
        Product? Get(int productID);
        //bổ sung mặt hàng mới
        int Add(Product data);
        //cập nhật thông tin mặt hàng
        bool Update(Product data);
        // xóa mặt hàng
        bool Delete(int productID);
        // kiểm tra mặt hàng hiện có đơn hàng liên quan hay không
        bool IsUsed(int productID);
        // lấy ds ảnh của mặt hàng 
        IList<ProductPhoto> ListPhotos(int productID);
        // lấy thông tin 1 ảnh dựa vào ID
        ProductPhoto? GetPhoto(long photoID);
        // bổ sung 1 ảnh cho mặt hàng
        long AddPhoto(ProductPhoto data);
        // cập nhật ảnh của mặt hàng
        bool UpdatePhoto(ProductPhoto data);
        // xóa ảnh của mặt hàng
        bool DeletePhoto(long photoID);
        //lấy ds các thuộc tính của mặt hàng sắp xếp theo thứ tự displayOder
        IList<ProductAttribute> ListAttributes(int productID);
        //lấy thông tin của thuộc tính theo mã thuộc tính
        ProductAttribute? GetAttribute(long attributeID);
        //bổ sung thuộc tính cho mặt hàng
        long AddAttribute(ProductAttribute data);
        //cập nhật thuộc tính của mặt hàng
        bool UpdateAttribute(ProductAttribute data);
        //xóa thuộc tính
        bool DeleteAttribute(long attributeID);

    }
}
