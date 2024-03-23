using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using SV20T1020493.DomainModels;
using SV20T1020493.Web.AppCodes;
using SV20T1020493.Web.Models;
using System;

namespace SV20T1020493.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admistrator},{WebUserRoles.Employee}")]
    public class HistoryController : Controller
    {
        const int PAGE_SIZE = 20;
        const string HiSTORY_SEARCH = "history_search";

        public IActionResult Index()
        {
            Models.HistorySearchInput? input = ApplicationContext.GetSessionData<HistorySearchInput>(HiSTORY_SEARCH);
            if (input == null)
            {
                input = new HistorySearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Work="",
                    TableName="",
                    DateRange = string.Format("{0:dd/MM/yyyy}-{1:dd/MM/yyyy}",
                    DateTime.Today.AddMonths(-1), DateTime.Today)
                };
            }
            return View(input);
        }
        public IActionResult Search(HistorySearchInput input)
        {
            int rowCount = 0;
            var data = HistoryDataService.ListHistory(out rowCount, input.Page, input.PageSize,
            input.Work, input.FromTime, input.ToTime, input.SearchValue ?? "",input.TableName);         
            var model = new HistorySearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                Work = input.Work,
                TimeRange = input.DateRange ?? "",
                RowCount = rowCount,
                TableName=input.TableName,
                Data = data
            };
            ApplicationContext.SetSessionData(HiSTORY_SEARCH, input);
            return View(model);
        }
        public IActionResult Deleterestore(int id)
        {
            History data = HistoryDataService.GetHistory(id);
            bool restone = HistoryDataService.Deleterestore(data.OldData);
                if (restone)
                {
                    return Json("Khôi phục thành công!");
                }
           return Json("Khôi phục không thành công! Có thể đã khôi phục rồi hoặc cần phải khôi phục bảng tham chiếu của nó trước");
         }
        public IActionResult Updaterestore(int id)
        {
            History data = HistoryDataService.GetHistory(id);
            bool restone = HistoryDataService.Updaterestore(data.OldData);
            if (restone)
            {
                return Json("Hoàn tác thành công!");
            }
            return Json("Hoàn tác không thành công! Có thể dữ liệu đã bị xóa rồi");
        }
    }
}
