using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using SV20T1020493.DomainModels;
using SV20T1020493.Web.AppCodes;
using SV20T1020493.Web.Models;
using System.Diagnostics;

namespace SV20T1020493.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admistrator}")]
    public class EmployeeController : Controller
    {
        const int PAGE_SIZE = 9;
        const string EMPLOYEE_SEARCH = "employee_search";
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


        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            var model = new Employee()
            {
                EmployeeID = 0,
                BirthDate= new DateTime(2000,1,1),
                Photo= "nophoto.png",
                IsWorking=true
            };
            return View("Edit", model);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            if (model.Photo == "") model.Photo = "nophoto.png";
            // néu k có ảnh thì thay băng f nophoto
            return View(model);
        }
        //==================================================================================================================
        [HttpPost]//Attribute=> chỉ nhân jdữ liệu gửi lên dưới dạng post
        public IActionResult Save(Employee model, string birthDateInput = "",IFormFile? uploadPhoto = null)
        {
            //Yêu cầu tên khách, tên giao dịch, email, tỉnh ko dc để trống
            if (string.IsNullOrWhiteSpace(model.FullName))
                ModelState.AddModelError("FullName", "Họ và tên không đươc để trống");
            if (string.IsNullOrWhiteSpace(model.Address))
                ModelState.AddModelError("Address", "Địa chỉ không được để trống");
            if (string.IsNullOrWhiteSpace(model.Email))
                ModelState.AddModelError("Email", "Email không được để trống");
            if (string.IsNullOrWhiteSpace(model.Phone))
                ModelState.AddModelError("Phone", "Số điện thoại không được để trống");
         
            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";
                return View("Edit", model);
            }
            // xu ly ngay sinh
            DateTime? d = birthDateInput.ToDateTime();
            if (d.HasValue)
            {
                model.BirthDate = d.Value;
            }
            // xy ly anh: neu co hi luu anh len server, gan photo
   
            if (uploadPhoto != null)
            {//ten file luw tren server
                Debug.WriteLine("Upload: " + uploadPhoto.FileName);

              //  string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string fileName = $"{uploadPhoto.FileName}";
                // dg dan den file lu tren server(vd:D:\MyWeb\wwwwroot\imgames\employee\photo.pbg
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, path2: @"images\employees", path3: fileName);
                Debug.WriteLine("Upload: " + uploadPhoto.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Photo = fileName;
            }



            if (model.EmployeeID == 0) { 
                int id = CommonDataService.AddEmployee(model);
                if (id <= 0)
                {
                    ModelState.AddModelError("Phone", "Số điện thoại bị trùng!");
                    ViewBag.Title = "Bổ sung nhân viên";
                    return View("Edit", model);
                }
            }

            else
            {
                bool result = CommonDataService.UpdateEmployee(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được!, có thể số điện thoại bị trùng!");
                    ViewBag.Title = "Cập nhật thông tin nhân viên";
                    return View("Edit", model);
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                bool result = CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }

            var model = CommonDataService.GetEmployee(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
