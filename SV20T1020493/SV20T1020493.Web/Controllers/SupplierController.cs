using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using SV20T1020493.DomainModels;
using SV20T1020493.Web.AppCodes;
using SV20T1020493.Web.Models;

namespace SV20T1020493.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admistrator},{WebUserRoles.Employee}")]
    public class SupplierController : Controller
    {
        
        const int PAGE_SIZE = 10;
        const string SUPPLIER_SEARCH = "supplier_search";

        public IActionResult Index(int page = 1, string searchValue = "")
        {
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SUPPLIER_SEARCH);
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
            var data = CommonDataService.ListOfSuppliers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            
            var model = new SupplierSearchResult()
            {
                RowCount = rowCount,
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                Data = data
            };
            Console.WriteLine("ccc: " + model.RowCount + "=" + model.Page + "-" + model.PageSize + "-" + model.SearchValue);
            // luu
            ApplicationContext.SetSessionData(SUPPLIER_SEARCH, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            var model = new Supplier()
            {
                SupplierID = 0
            };
            return View("Edit",model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
            var model = CommonDataService.GetSupplier(id);
            return View(model);
        }

        public IActionResult Save(Supplier model)
        {
            if (string.IsNullOrWhiteSpace(model.SupplierName))
                ModelState.AddModelError("SupplierName", "Tên nhà cung cấp không đươc để trống");
            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError("Phone", "Số điện thoại không thể trống");
            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError("Email", "Email không được để trống");
            if (string.IsNullOrWhiteSpace(model.Address))
                ModelState.AddModelError("Address", "Địa chỉ không được để trống");
            if (string.IsNullOrWhiteSpace(model.Province))
                ModelState.AddModelError("Province", "Vui lòng chọn địa chỉ");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật thông tin nhà cung cấp";
                return View("Edit", model);
            }
            if (model.SupplierID == 0) { 
                int id = CommonDataService.AddSupplier(model);
                if (id <= 0)
                {
                    ModelState.AddModelError("Email", "Email bị trùng!");
                    ViewBag.Title = "Bổ sung nhà cung cấp";
                    return View("Edit", model);
                }
            }

            else
            {
                bool result = CommonDataService.UpdateSupplier(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được, có thể email bị trùng!");
                    ViewBag.Title = "Cập nhật thông tin nhà cung cấp";
                    return View("Edit", model);
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa nhà cung cấp";
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetSupplier(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
