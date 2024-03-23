using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using SV20T1020493.DomainModels;
using SV20T1020493.Web.AppCodes;
using SV20T1020493.Web.Models;

namespace SV20T1020493.Web.Controllers
{
    [Authorize(Roles =$"{WebUserRoles.Admistrator},{WebUserRoles.Employee}")]
    public class CustomerController : Controller
    {
        const int PAGE_SIZE = 20;
        const string CREATE_TITLE = "Bổ sung khách hàng";
        const string CUSTOMER_SEARCH = "customer_search";// ten bien session dung der lu lai dieu kien tim kiem
      
        public IActionResult Index()
        {           
            // Kiem tra xem trong session cos lu dieu kien tim kiem khong
            // neu co thi su dung dieu kien tim kiem nguoc laij thi tim kiem theo dieu kien mac dinh
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CUSTOMER_SEARCH);
            if (input == null) 
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        
        }

        public IActionResult Search(PaginationSearchInput input)
        {
            // nho check null thi quay ve mac dinh
            int rowCount = 0;
            var data = CommonDataService.ListOfCustomers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new CustomerSearchResult() { 
                RowCount=rowCount,
                Page=input.Page,
                PageSize=input.PageSize,
                SearchValue=input.SearchValue??"",
                Data=data
            };
            // luu
            ApplicationContext.SetSessionData(CUSTOMER_SEARCH, input);
            return View(model);
        }


        public IActionResult Create()
        {
            ViewBag.Title = CREATE_TITLE;
            var model = new Customer()
            {
                CustomerID = 0
            };
            return View("Edit",model);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin khách hàng";
            var model = CommonDataService.GetCustomer(id);
             if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetCustomer(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]//Attribute=> chỉ nhân jdữ liệu gửi lên dưới dạng post
        public IActionResult Save(Customer model)
        {
            if (string.IsNullOrWhiteSpace(model.CustomerName))
                ModelState.AddModelError("CustomerName", "Tên không đươc để trống");
            if(string.IsNullOrWhiteSpace(model.ContactName))
                ModelState.AddModelError("ContactName", "Tên giao dịch không được để trống");
            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError("Email", "Email không được để trống");
            if (string.IsNullOrWhiteSpace(model.Province))
                ModelState.AddModelError("Province", "Vui lòng chọn tỉnh/thành");
            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.CustomerID == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";
                return View("Edit", model);
            }
            if (model.CustomerID == 0) { 
                int id = CommonDataService.AddCustomer(model);
                if (id <= 0) {
                    ModelState.AddModelError("Email", "Email bị trùng!");
                    ViewBag.Title = CREATE_TITLE;
                    return View("Edit", model);
                }
            }
            else {          
                bool result = CommonDataService.UpdateCustomer(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được khách hàng, có thể email bị trùng!");
                    ViewBag.Title = "Cập nhật thông tin khách hàng";
                    return View("Edit", model);
                }
            }
            
            return RedirectToAction("Index");
        }

    }

}
/* tương tự thiết kế giao diện bổ sung cập nhật đối với:
 nhà cung câp, người giao hàng, loại hàng*/
/*ctrl r r laf chonj het chu giong nhau*/