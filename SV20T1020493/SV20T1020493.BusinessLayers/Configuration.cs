using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020493.BusinessLayers
{
    /// <summary>
    /// Khởi tạo và lưu trữ các thông tin cấu hình của BusinessLayer
    /// </summary>
    public static class Configuration
    {
        public static string ConnectionString { get; private set; } = "";
        /// <summary>
        /// Hàm khởi tạo cấu hình cho BusinesLayer
        /// (Hàm này phải được gọi trước khi chạy ứng dụng)
        /// </summary>
        /// <param name="connectionString"></param>
        public static void Initialize(string connectionString)
        {
            Configuration.ConnectionString = connectionString;
        }
    }
}
