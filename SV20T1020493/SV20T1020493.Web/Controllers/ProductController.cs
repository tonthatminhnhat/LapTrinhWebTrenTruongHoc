using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV20T1020493.BusinessLayers;
using SV20T1020493.DomainModels;
using SV20T1020493.Web.AppCodes;
using SV20T1020493.Web.Models;
using System.Diagnostics;
using System.Reflection;

namespace SV20T1020493.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.Admistrator},{WebUserRoles.Employee}")]
    public class ProductController : Controller
    {
        const int PAGE_SIZE = 20;
        const string PRODUCT_SEARCH = "product_search";

        public IActionResult Index()
        {
            Models.ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH);
            if (input == null)
            {
                input = new ProductSearchInput()
                {
                    Page = 1,PageSize = PAGE_SIZE,SearchValue = "",
                    CategoryID=0,SupplierID=0
                };
            }
            return View(input);
        }

        public IActionResult Search(ProductSearchInput input, int page = 1, string searchValue = "", int categoryID = 0,
            int supplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            // nho check null thi quay ve mac dinh
            int rowCount = 0;
            var data = ProductDataService.ListOfProducts(out rowCount, page, PAGE_SIZE, searchValue ?? "",
                categoryID, supplierID, minPrice, maxPrice);

            var model = new ProductSearchResult()
            {
                Page = page,PageSize = PAGE_SIZE,SearchValue = searchValue ?? "",CategoryID = categoryID,
                SupplierID = supplierID, RowCount = rowCount, Data = data
            };
            // luu
            ApplicationContext.SetSessionData(PRODUCT_SEARCH, input);
            return View(model);
        }




        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var model = new ProductEditResult()
            {
                Product = new Product()
                {
                    ProductID = 0,
                    IsSelling = true,
                    Photo = "nophoto.png",
                    Unit = "Cái"
                }
            };

            ViewBag.IsEdit = false;
            return View("Edit", model);
        }
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            var model = new ProductEditResult()
            {
                Product = ProductDataService.GetProduct(id),
                DataPhotos = ProductDataService.ListOfPhotos(id),
                DataAttributes = ProductDataService.ListOfAttributes(id)
            };

            if (model == null)
            {
                return null;
            }

            ViewBag.IsEdit = true;
            return View(model);
        }
        [HttpPost]//Attribute=> chỉ nhân jdữ liệu gửi lên dưới dạng post
        public IActionResult Save(ProductEditResult model, IFormFile? uploadPhoto = null)
        {
            int ProductID = model.Product.ProductID;
            model.DataAttributes = ProductDataService.ListOfAttributes(ProductID);
            model.DataPhotos = ProductDataService.ListOfPhotos(ProductID);

            bool isNewProduct = ProductID == 0;

            if (ProductID == 0) ViewBag.IsEdit = false;
            else ViewBag.IsEdit = true;

            if (string.IsNullOrWhiteSpace(model.Product.ProductName))
                ModelState.AddModelError("ProductName", "Tên mặt hàng không đươc để trống");
            if (model.Product.CategoryID == 0)
                ModelState.AddModelError("CategoryID", "Vui lòng chọn loại hàng");
            if (model.Product.SupplierID == 0)
                ModelState.AddModelError("SupplierID", "Vui lòng chọn nhà cung cấp");
            if (string.IsNullOrWhiteSpace(model.Product.Unit))
                ModelState.AddModelError("Unit", "Đơn vị tính không được để trống");
            if (model.Product.Price < 1000)
                ModelState.AddModelError("Price", "Vui lòng nhập giá hàng ≥ 10000");
            if (uploadPhoto != null)
            {
                string fileName = $"{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, path2: @"images\products", path3: fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Product.Photo = fileName + "";

            }
            if (String.IsNullOrEmpty(model.Product.Photo) || model.Product.Photo == "nophoto.png")
                ModelState.AddModelError("uploadPhoto", "Vui lòng chọn ảnh");

            if (!ModelState.IsValid)
            {
                ViewBag.IsEdit = isNewProduct ? false : true;
                ViewBag.Title = isNewProduct ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";
                return View("Edit", model);

            }
            if (isNewProduct)
            {
                int id = ProductDataService.AddProduct(model.Product);
                if (id <= 0)
                {
                    ModelState.AddModelError("ProductName", "Tên mặt hàng bị trùng!");
                    ViewBag.Title = "Bổ sung mặt hàng";
                    return View("Edit", model);
                }
                else return RedirectToAction("Index");
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(model.Product);

                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được!, có thể tên mặt hàng bị trùng!");
                    ViewBag.Title = "Cập nhật thông tin mặt hàng";
                    return View("Edit", model);
                }
            }
           
            ModelState.AddModelError("Success", "Dữ liệu đã được cập nhật thành công!");
            return View("Edit", model);
        }
        public IActionResult Delete(int id)
        {
            ViewBag.Title = "Xóa mặt hàng";
            if (Request.Method == "POST")
            {
                bool result = ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }

            var model = ProductDataService.GetProduct(id);

            if (model == null)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public IActionResult Photo(int id, string method, int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    return View(new ProductPhoto() { ProductID = id, DisplayOrder = 0, Photo = "nophoto.png" });

                case "edit":
                    ViewBag.Title = "Cập nhật ảnh cho mặt hàng";
                    return View(ProductDataService.GetPhoto(photoId));

                case "delete":
                    //TODO: xóa ảnh có mã là photoid trực tiếp
                    ProductDataService.DeletePhoto(photoId);            
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto model, IFormFile? uploadPhoto = null)
        {
            if (uploadPhoto != null)
            {
                string fileName = $"{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.HostEnviroment.WebRootPath, path2: @"images\products", path3: fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                model.Photo = fileName;

            }
            if (String.IsNullOrEmpty(model.Photo) || model.Photo == "nophoto.png")
            {
                ModelState.AddModelError("uploadPhoto", "Vui lòng chọn ảnh");
                ViewBag.Title = model.PhotoID == 0 ? "Bổ sung ảnh cho mặt hàng" : "Cập nhật ảnh cho mặt hàng";
                return View("Photo", model);
            }
         

            if (model.PhotoID == 0)
            {
                long id = ProductDataService.AddPhoto(model);
                if (id <= 0)
                {
                    ModelState.AddModelError("uploadPhoto", "Ảnh bị trùng!");
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    return View("Photo", model);
                }
                return RedirectToAction("Edit", new { id = model.ProductID });
            }
            else
            {
                bool result = ProductDataService.UpdatePhoto(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được!, có thể ảnh đã bị trùng!");
                    ViewBag.Title = "Cập nhật ảnh cho mặt hàng";
                    return View("Photo", model);
                }
            }
            return RedirectToAction("Edit", new { id = model.ProductID });
        }

        public IActionResult Attribute(int id, string method, int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    return View(new ProductAttribute() { ProductID = id,DisplayOrder=0}) ;
                case "edit":
                    ViewBag.Title = "Cập nhật thuộc tính cho mặt hàng";
                    return View(ProductDataService.GetAttribute(attributeId));
                case "delete":
                    ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");

            }

        }

        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute model)
        {

            if (String.IsNullOrEmpty(model.AttributeValue))
                ModelState.AddModelError("AttributeValue", "Giá trị thuộc tính không thể trống!");

            if (String.IsNullOrEmpty(model.AttributeName))
                ModelState.AddModelError("AttributeName", "Tên thuộc tính không thể trống!");


            if (!ModelState.IsValid)
            {
                ViewBag.Title = model.AttributeID == 0 ? "Bổ sung thuộc tính cho mặt hàng" : "Cập nhật thuộc tính cho mặt hàng";
                return View("Attribute", model);
            }

            if (model.AttributeID == 0)
            {
                long id = ProductDataService.AddAttribute(model);
                if (id <= 0)
                {
                    ModelState.AddModelError("AttributeName", "Tên thuộc tính bị trùng!");
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    return View("Attribute", model);
                }
                return RedirectToAction("Edit", new { id = model.ProductID });
            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(model);
                if (!result)
                {
                    ModelState.AddModelError("Error", "Không cập nhật được!, có thể tên thuộc tính đã bị trùng!");
                    ViewBag.Title = "Cập nhật thuộc tính cho mặt hàng";
                    return View("Attribute", model);
                }
            }
            return RedirectToAction("Edit", new { id = model.ProductID });
        }



    }
}
