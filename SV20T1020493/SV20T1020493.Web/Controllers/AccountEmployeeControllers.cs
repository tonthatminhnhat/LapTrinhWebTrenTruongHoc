using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using SV20T1020493.Web.AppCodes;
using SV20T1020493.Web.Models;

namespace SV20T1020493.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admistrator}")]
    public class AccountEmployeeControllers : Controller
    {
        const int PAGE_SIZE = 9;
        const string EMPLOYEE_SEARCH = "accountemployee_search";
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            Models.PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH);
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
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");

            var model = new EmployeeSearchResult()
            {
                RowCount = rowCount,
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                Data = data
            };
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH, input);
            return View(model);
        }
    }
}
