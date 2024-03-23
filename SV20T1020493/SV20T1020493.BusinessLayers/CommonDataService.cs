using Dapper;
using Microsoft.Data.SqlClient;
using SV20T1020493.DataLayers;
using SV20T1020493.DataLayers.SQLServer;
using SV20T1020493.DomainModels;

namespace SV20T1020493.BusinessLayers
{
    /// <summary>
    /// cung cấp các chức năng nghiệp vụ liên quan đến các dữ liệu "chung"
    /// (tinh/thành, khách hàng, nhà cung cấp, loại hàng, người giao hàng,nhân viên)
    /// </summary>
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Category> categoryDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<AccountEmployee> accountDB;
        /// <summary>
        /// ctor( static constructor hoạt động như thế nào ? cách viết?)
        /// </summary>
        static CommonDataService()
        {
            string connectionString = Configuration.ConnectionString;
            provinceDB = new ProvinceDAL(connectionString);
            customerDB = new CustomerDAL(connectionString);
            categoryDB = new CategoryDAL(connectionString);
            supplierDB = new SupplierDAL(connectionString);
            shipperDB  = new ShipperDAL(connectionString);
            employeeDB = new EmployeeDAL(connectionString);
            accountDB = new AccountEmployeeDAL(connectionString);
          //  productDB = new ProductDAL(connectionString);
        }

        //============================ Province =========================================

        public static List<Province> ListOfProvinces(){
            return provinceDB.List().ToList();
        }

        //============================ Customer =========================================

        /// <summary>
        ///Tìm kiếm và lấy ds khách hàng
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng dữ liệu tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang(0 nếu không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (Rỗng nếu lấy toàn bộ khách hàng)</param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers(out int rowCount,int page=1,int pageSize=0,string searchValue="")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page,pageSize,searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin cửa 1 khách hàng theo mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// Bổ súng 1 khách hàng mới. hàm trả về mã của khách hàng mới được bổ sung
        /// (hàm trả về -1 nếu email bị trùng, trả về giá trị 0 nếu lỗi)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        /// <summary>
        /// xóa 1 khách hàng ( nếu khách hàng đó không có dữ liệu liên quan)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if (customerDB.IsUsed(id))
                return false;
            return customerDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra xem 1 khách hàng hiện có dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCustomer(int id)
        {
            return customerDB.IsUsed(id);
        }

        //============================ Categories =========================================

        /// <summary>
        /// Tìm kiếm và lấy danh sách loại hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
       /// Lấy thông tin 1 loại hàng theo mã hàng
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        /// <summary>
        /// Bổ sung 1 loại hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        /// <summary>
        /// Cập nhật loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        /// <summary>
        /// Xóa loại hàng có mã id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            if (categoryDB.IsUsed(id))
                return false;
            return categoryDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra xem loại hàng hiện có dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCategory(int id)
        {
            return categoryDB.IsUsed(id);
        }

        //============================ Supplier =========================================

        /// <summary>
        ///Tìm kiếm và lấy ds khách hàng
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng dữ liệu tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang(0 nếu không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (Rỗng nếu lấy toàn bộ khách hàng)</param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin cửa 1 khách hàng theo mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        /// <summary>
        /// Bổ súng 1 khách hàng mới. hàm trả về mã của khách hàng mới được bổ sung
        /// (hàm trả về -1 nếu email bị trùng, trả về giá trị 0 nếu lỗi)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        /// <summary>
        /// xóa 1 khách hàng ( nếu khách hàng đó không có dữ liệu liên quan)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if (supplierDB.IsUsed(id))
                return false;
            return supplierDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra xem 1 khách hàng hiện có dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedSupplier(int id)
        {
            return supplierDB.IsUsed(id);
        }

        //============================ Shipper =========================================

        /// <summary>
        ///Tìm kiếm và lấy ds khách hàng
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng dữ liệu tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang(0 nếu không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (Rỗng nếu lấy toàn bộ khách hàng)</param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin cửa 1 khách hàng theo mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        /// <summary>
        /// Bổ súng 1 khách hàng mới. hàm trả về mã của khách hàng mới được bổ sung
        /// (hàm trả về -1 nếu email bị trùng, trả về giá trị 0 nếu lỗi)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        /// <summary>
        /// xóa 1 khách hàng ( nếu khách hàng đó không có dữ liệu liên quan)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            if (shipperDB.IsUsed(id))
                return false;
            return shipperDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra xem 1 khách hàng hiện có dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedShipper(int id)
        {
            return shipperDB.IsUsed(id);
        }

        //============================ Employee =========================================

        /// <summary>
        ///Tìm kiếm và lấy ds khách hàng
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng dữ liệu tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang(0 nếu không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (Rỗng nếu lấy toàn bộ khách hàng)</param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin cửa 1 khách hàng theo mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        /// <summary>
        /// Bổ súng 1 khách hàng mới. hàm trả về mã của khách hàng mới được bổ sung
        /// (hàm trả về -1 nếu email bị trùng, trả về giá trị 0 nếu lỗi)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        /// <summary>
        /// xóa 1 khách hàng ( nếu khách hàng đó không có dữ liệu liên quan)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.IsUsed(id))
                return false;
            return employeeDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra xem 1 khách hàng hiện có dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedEmloyee(int id)
        {
            return employeeDB.IsUsed(id);
        }

        //============================ Employee =========================================

        /// <summary>
        ///Tìm kiếm và lấy ds khách hàng
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng dữ liệu tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang(0 nếu không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (Rỗng nếu lấy toàn bộ khách hàng)</param>
        /// <returns></returns>
        public static List<AccountEmployee> ListOfAccoutEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = accountDB.Count(searchValue);
            return accountDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin cửa 1 khách hàng theo mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AccountEmployee? GetAccountEmployee(int id)
        {
            return accountDB.Get(id);
        }
        /// <summary>
        /// Bổ súng 1 khách hàng mới. hàm trả về mã của khách hàng mới được bổ sung
        /// (hàm trả về -1 nếu email bị trùng, trả về giá trị 0 nếu lỗi)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddAccountEmployee(AccountEmployee data)
        {
            return accountDB.Add(data);
        }
        public static bool UpdateAccountEmployee(AccountEmployee data)
        {
            return accountDB.Update(data);
        }
        /// <summary>
        /// xóa 1 khách hàng ( nếu khách hàng đó không có dữ liệu liên quan)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteAccountEmployee(int id)
        {
            if (accountDB.IsUsed(id))
                return false;
            return accountDB.Delete(id);
        }
        /// <summary>
        /// Kiểm tra xem 1 khách hàng hiện có dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedAccountEmloyee(int id)
        {
            return accountDB.IsUsed(id);
        }
    }
}

/// ctrl + M + O thu gon ngươc lại M + L