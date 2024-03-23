using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using SV20T1020493.DomainModels;
using SV20T1020493.Web.AppCodes;
using SV20T1020493.Web.Models;

namespace SV20T1020493.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admistrator},{WebUserRoles.Employee}")]
    public class CategoryController : Controller
    {
        const int PAGE_SIZE = 10;
        const string CATEGORY_SEARCH = "category_search";// ten bien session dung der lu lai dieu kien tim kiem

        public IActionResult Index(int page = 1, string searchValue = "")
        {

            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(CATEGORY_SEARCH);
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
            var data = CommonDataService.ListOfCategories(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new CategorySearchResult()
            {
                RowCount = rowCount,
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                Data = data
            };
            ApplicationContext.SetSessionData(CATEGORY_SEARCH, input);
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung danh mục";
            var model = new Category()
            {
                CategoryID = 0
            };

            return View("Edit",model);
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin danh mục";
            var model = CommonDataService.GetCategory(id);
  
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa danh mục";

            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetCategory(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);

        }

        public IActionResult Save(Category model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryName))
                ModelState.AddModelError("CategoryName", "Tên danh mục không đươc để trống");         

            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.CategoryID == 0 ? "Bổ sung danh mục" : "Cập nhật thông tin danh mục";
                return View("Edit", model);
            }

            if (model.CategoryID == 0) { 
                int id = CommonDataService.AddCategory(model);
                if (id <= 0)
                {
                    ModelState.AddModelError("CategoryName", "Tên danh mục bị trùng!");
                    ViewBag.Title = "Bổ sung danh mục";
                    return View("Edit", model);
                }
            }

            else
            {
                bool result = CommonDataService.UpdateCategory(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được!, có thể tên danh mục bị trùng!");
                    ViewBag.Title = "Cập nhật thông tin danh mục";
                    return View("Edit", model);
                }
            }

            return RedirectToAction("Index");
        }
    }
}
