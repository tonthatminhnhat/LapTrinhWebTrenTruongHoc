using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using SV20T1020493.DomainModels;
using SV20T1020493.Web.AppCodes;
using SV20T1020493.Web.Models;

namespace SV20T1020493.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admistrator},{WebUserRoles.Employee}")]
    public class ShipperController : Controller
    {
        const int PAGE_SIZE = 20;
        const string SHIPPER_SEARCH = "shipper_search";
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH);
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
            int rowCount = 0;
            var data = CommonDataService.ListOfShippers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new ShipperSearchResult()
            {
                RowCount = rowCount,
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                Data = data
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH, input);
            return View(model);
        }


        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung shipper";
            var model = new Shipper()
            {
                ShipperID = 0
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin shipper";
            var model = CommonDataService.GetShipper(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]//Attribute=> chỉ nhân jdữ liệu gửi lên dưới dạng post
        public IActionResult Save(Shipper model)
        {
            if (string.IsNullOrWhiteSpace(model.ShipperName))
                ModelState.AddModelError("ShipperName", "Tên shipper không đươc để trống");
            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError("Phone", "Số điện thoại không được để trống");

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.ShipperID == 0 ? "Bổ sung shipper" : "Cập nhật thông tin shipper";
                return View("Edit", model);
            }

            if (model.ShipperID == 0) { 
                int id = CommonDataService.AddShipper(model);
                if (id <= 0)
                {
                    ModelState.AddModelError("Phone", "Số điện thoại bị trùng!");
                    ViewBag.Title = "Bổ sung shipper";
                    return View("Edit", model);
                }
            }

            else
            {
                bool result = CommonDataService.UpdateShipper(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được khách hàng, có thể số điện thoại bị trùng!");
                    ViewBag.Title = "Cập nhật thông tin shipper"; 
                    return View("Edit", model);
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa shipper";
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetShipper(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
