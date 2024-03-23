using System.Globalization;
using System.Text.RegularExpressions;

namespace SV20T1020493.Web.Models
{
    public class PaginationSearchInput
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public string SearchValue { get; set; } = "";
    }
    // dung cho mat hang
    public class ProductSearchInput : PaginationSearchInput
    {
        public int CategoryID { get; set; } = 0;
        public int SupplierID { get; set; } = 0;
        public string deliveryProvince { get; set; } = "";
        public int customerID { get; set; } = 0;
        public string deliveryAddress { get; set; } = "";
    }
    public class HistorySearchInput : PaginationSearchInput
    {
        public string Work { get; set; } = "";
        public string TableName { get; set; } = "";
        public string DateRange { get; set; } = "";
        /// <summary>
        /// Lấy thời điểm bắt đầu dựa vào DateRange
        /// </summary>
        public DateTime? FromTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DateRange))
                    return null;
                string[] times = DateRange.Split('-');
                
                if (times.Length == 2)
                {
                    CultureInfo culture = new CultureInfo("vi-VN");
                    if (DateTime.TryParseExact(times[0].Trim(), "dd/MM/yyyy", culture, DateTimeStyles.None, out DateTime result))
                    {
                        // Đặt giờ và phút thành 00:00:00
                        return result.Date.AddHours(0).AddMinutes(0).AddSeconds(0);
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Lấy thời điểm kết thúc dựa vào DateRange
        /// (thời điểm kết thúc phải là cuối ngày)
        /// </summary>
        public DateTime? ToTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DateRange))
                    return null;
                string[] times = DateRange.Split('-');
                if (times.Length == 2)
                {
                    DateTime? value = Converter.ToDateTime(times[1].Trim());
                    if (value.HasValue)

                        value = value.Value.AddMilliseconds(86399998); //86399999
                    return value;
                }
                return null;
            }
        }
    }
}
