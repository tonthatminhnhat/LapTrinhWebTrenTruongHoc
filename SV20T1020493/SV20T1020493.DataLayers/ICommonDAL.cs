using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.DataLayers
{

    //Mô tả các phép xử lý dữ liệu chung chung (generic)
    public interface ICommonDAL<T> where T : class
    {
        /// <summary>
        ///Tìm kiếm và lấy danh sách dữ liệu dưới dạng phân trang
        /// </summary>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">số dòng trên mỗi trang (bằng 0 nếu ko phân trang) ,</param>
        /// <param name="searchValue">Giá trị tìm kiếm(Chuõi rống nếu lấy toàn bộ dữ liệu</param>
        /// <returns></returns>
        IList<T> List ( int page=1, int pageSize=0, string searchValue="" );

        /// <summary>
        /// Đếm số lượng dòng dữ liệu tìm đươc
        /// </summary>
        /// <param name="searchValue">Giá trị tìm kiếm(chuỗi rỗng nếu lấy toàn bộ dữ liệu)</param>
        /// <returns></returns>
        int Count(string searchValue = "");

        /// <summary>
        /// Lấy 1 bản ghi/ dòng dữ liệu dựa trên mã (id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);

        /// <summary>
        /// bổ sung dữ liệu vào trong csdl. Hàm trả về ID của dữ liệu được bổ sung
        /// (Trả về giá trị nhỏ hơn hc =0 nếu lỗi
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);

        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);

        /// <summary>
        /// Xóa 1 bản ghi dữ liệu dựa vào id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(int id);

        /// <summary>
        /// Kiểm trả xem 1 bản ghi dữ liẹu có mã id hiện đang có được sử dụng bởi các bảng khác hay không
        /// (Có dữ liệu liên quan hay không?)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsUsed(int id);

    }
}
//ctrl + m + o là thu m+ l là ra lại