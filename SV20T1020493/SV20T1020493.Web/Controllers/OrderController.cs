using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using SV20T1020493.DataLayers.SQLServer;
using SV20T1020493.DomainModels;
using SV20T1020493.Web.AppCodes;
using SV20T1020493.Web.Models;

namespace SV20T1020493.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admistrator},{WebUserRoles.Employee}")]
    public class OrderController : Controller
    {
        // số dòng trên 1 trang khi hiển thị danh sách đơn hàng
        private const int ORDER_PAGE_SIZE = 20;
        private const string ORDER_SEARCH = "order_search";
        public IActionResult Index()
        {           
            OrderSearchInput? input = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH);
            if (input == null)
            {
                input = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = ORDER_PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    DateRange = string.Format("{0:dd/MM/yyyy}-{1:dd/MM/yyyy}",
                    DateTime.Today.AddMonths(-1), DateTime.Today)
                };
            }
            return View(input);
        }
        public IActionResult Search(OrderSearchInput input)
        {
            int rowCount = 0;
            var data = OrderDataService.ListOrders(out rowCount, input.Page, input.PageSize,
            input.Status, input.FromTime, input.ToTime, input.SearchValue ?? "");
           
            var model = new OrderSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                Status = input.Status,
                TimeRange = input.DateRange ?? "",
                RowCount = rowCount,
                Data = data
            }; 
            ApplicationContext.SetSessionData(ORDER_SEARCH, input);
            return View(model);
        }
        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var details = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = details
            };
            return View(model);
        }
        // chuyển đơn hàng sang tạng thái đã được duyệt
        public IActionResult Accept(int id = 0)
        {
            bool result = OrderDataService.AcceptOrder(id);
            if (!result)
                TempData["Message"] = "Không thể duyệt đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        //Chuyển đơn hàng sang trạng thái đã kết thúc
        public IActionResult Finish(int id = 0)
        {
            bool result = OrderDataService.FinishOrder(id);
            if (!result) TempData["Message"] = "Không thể ghi nhận trạng thái kết thúc cho đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        //Chuyển đơn hàng sang trạng thái bị hủy
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
                TempData["Message"] = "Không thể thực hiện thao tác hủy đối với đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        //Chuyển đơn hàng sang trạng thái bị từ chối
        public IActionResult Reject(int id = 0)
        {
            bool result = OrderDataService.RejectOrder(id);
            if (!result)
                TempData["Message"] = "Không thể thực hiện thao tác từ chối đối với đơn hàng này";
            return RedirectToAction("Details", new { id });
        }
        // xóa đơn hàng
        public IActionResult Delete(int id)
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
            {
                TempData["Message"] = "Không thể xóa đơn hàng này";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // Giao diện để chọn người giao hàng cho đơn hàng
        [HttpGet]
        public IActionResult Shipping(int id = 0)
        {        
            ViewBag.OrderID = id;
            return View();
        }
        //Ghi nhận người giao hàng cho đơn hàng và chuyển đơn hàng sang trạng thái đang giao hàng.
        //Hàm trả về chuỗi khác rỗng thông báo lỗi nếu đầu vào ko hợp lệ hc lỗi
        //trả về chuỗi rỗng nếu thành công
        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            if (shipperID <= 0)
                return Json("Vui lòng chọn người giao hàng");
            bool result = OrderDataService.ShipOrder(id, shipperID);
            if (!result)
                return Json("Đơn hàng không cho phép chuyển cho người giao hàng");

            return Json("");

        } 
        public IActionResult UpdateAddress(int id = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            if (HttpContext.Request.Method == "POST")
            {
                // Xử lý logic cho phương thức POST
                if (string.IsNullOrEmpty(deliveryProvince))
                    return Json("Vui lòng chọn tỉnh/thành");
                if (string.IsNullOrEmpty(deliveryAddress))
                    return Json("Địa chỉ không được để trống");
                bool result = OrderDataService.UpdateOrder(id, deliveryProvince, deliveryAddress);
                if (!result)
                    return Json("Đơn hàng không cho phép thay đổi địa chỉ giao hàng");
                return Json("");
            }
            else 
            {
                // Xử lý logic cho phương thức GET
                ViewBag.OrderID = id;
                ViewBag.DeliveryProvince = deliveryProvince;
                ViewBag.DeliveryAddress = deliveryAddress;
                return View();
            }
         
        }

        //xóa mặt hàng khỏi đơn hàng
        public IActionResult DeleteDetail(int id = 0, int productId = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productId);
            if (!result)
                TempData["Message"] = "Không thể xóa mặt hàng ra khỏi đơn hàng";
            return RedirectToAction("Details", new { id });
        }
        // Giao diện để sửa đổi thông tin mặt hàng dc bán tron đơn hàng
        [HttpGet]
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productId);
            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            if (quantity <= 0)
                return Json("Số lượng bán không hợp lệ");
            if (salePrice < 0)
                return Json("Giá bán không hơp lệ");
            bool result = OrderDataService.SaveOrderDetail(orderID, productID, quantity, salePrice);
            if (!result)
                return Json("Không được phép thay đổi thông tin của đơn hàng này");
            return Json("");
        }
        //============================================================================
        private const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH = "product_search_for_sale";
        private const string SHOPPING_CART = "shopping_cart";
        // giao dien trang lap don hang moi
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = "",
                    deliveryProvince="",
                    customerID= 0,
                    deliveryAddress=""
                };
            }
            return View(input);
        }
        [HttpPost]
        public IActionResult UpdateProductSearchInput(string name, string value)
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);

            switch (name)
            {
                case "deliveryProvince":
                    input.deliveryProvince = value;
                    break;
                case "customerID":
                    input.customerID = int.Parse(value);
                    break;
                case "deliveryAddress":
                    input.deliveryAddress = value;
                    break;
                default:
                    break;
            }

            // Lưu dữ liệu vào session
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);

            return Ok(); // Trả về mã thành công 200
        }

        // tim kiem mat hang de dua vao gio hang
        public IActionResult SearchProduct(ProductSearchInput input)
           
        {
            int rowCount = 0;
            var data = ProductDataService.ListOfProducts(out rowCount, input.Page, input.PageSize,
                            input.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }
        // lay gio hang hien dang luu trong sesssion
        private List<OrderDetail> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<OrderDetail>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<OrderDetail>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
        // trang hien thi danh sach cac mat hang dang co trong gio hang
        public IActionResult ShowShoppingCart()
        {
            var model = GetShoppingCart();
            return View(model);
        }
        // bo sung them mat hang vao gio hang
        public IActionResult AddToCart(OrderDetail data)
        {
            if (data.SalePrice <= 0 || data.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ!");
            var shoppingCart = GetShoppingCart();
            var exitsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == data.ProductID);
            if (exitsProduct == null)
            {
                shoppingCart.Add(data);
            }
            else
            {
                exitsProduct.Quantity += data.Quantity;
                exitsProduct.SalePrice = data.SalePrice;
            }
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        // xoa mat hang khoi gio hang
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index >= 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        //Xoa tat ca cac mat hang trong gio hang
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            input.deliveryProvince = "";
            input.customerID = 0;
            input.deliveryAddress = "";
            return Json("");
        }
        // khoi tao don hang (lap 1 don hang moi)
        public IActionResult Init(int customerID = 0, string deliveryProvince = "",
                                 string deliveryAddress = "")
        {

            var shoppingCart = GetShoppingCart();
  
            if (shoppingCart.Count == 0)
                return Json("Giỏ hàng trống, không thể lập đơn hàng");
            if (customerID <= 0 || string.IsNullOrWhiteSpace(deliveryAddress)
                    || string.IsNullOrWhiteSpace(deliveryProvince))
                return Json("Vui lòng nhập đầy đủ thông tin!");

            int employeeID = Convert.ToInt32(User.GetUserData()?.UserId);
            int orderID = OrderDataService.InitOrder(employeeID, customerID,
                deliveryProvince, deliveryAddress, shoppingCart);
            ClearCart();
            return Json(orderID);
        }
    }
}
