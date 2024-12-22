using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020581.BusinessLayers;
using SV21T1020581.DomainModels;
using SV21T1020581.Web.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SV21T1020581.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINSTATOR}, {WebUserRoles.EMPLOYEE}")]
    public class ProductController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchCondition";
        public IActionResult Index()
        {
            ProductSearchInput? condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if (condition == null)
                condition = new ProductSearchInput()
                {
                    CategoryID = 0,
                    SupplierID = 0,
                    MinPrice = 0,
                    MaxPrice = 0,
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            return View(condition);
        }

        public IActionResult Search(ProductSearchInput condition)
        {
            int rowCount;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "", condition.CategoryID, condition.SupplierID, condition.MinPrice, condition.MaxPrice);
            ProductSearchResult model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = condition.CategoryID,
                SupplierID = condition.SupplierID,
                MinPrice = condition.MinPrice,
                MaxPrice = condition.MaxPrice,
                Data = data
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);

            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            var data = new Product
            {   CategoryID = 0,
                IsSelling = true
            };

            return View("Edit", data);
        }

        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin mặt hàng";
            var data = ProductDataService.GetProduct(id);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }

        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.GetProduct(id);

            if (data == null)
                return RedirectToAction("Index");

            return View(data);
        }

        [HttpPost]
        public IActionResult Save(Product data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.ProductID == 0 ? "Bổ sung mặt hàng" : "Cập nhật thông tin mặt hàng";

            if (string.IsNullOrWhiteSpace(data.ProductName))
            {
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để rỗng");
            }
            if (string.IsNullOrWhiteSpace(data.ProductDescription))
            {
                data.ProductDescription = "";
            }
            if (string.IsNullOrWhiteSpace(data.Unit))
            {
                ModelState.AddModelError(nameof(data.Unit), "Đơn vị không được để rỗng");
            }
            if(data.SupplierID == 0)
            {
                ModelState.AddModelError(nameof(data.Unit), "Vui lòng chọn nhà cung cấp");
            }
            if (data.CategoryID == 0)
            {
                ModelState.AddModelError(nameof(data.Unit), "Vui lòng chọn mặt hàng");
            }
            if (data.Price == 0)
            {
                ModelState.AddModelError(nameof(data.Price), "Vui lòng nhập giá");
            }

            if (string.IsNullOrWhiteSpace(data.CategoryID.ToString()))
            {
                ModelState.AddModelError(nameof(data.CategoryID), "Vui lòng lựa chọn loại hàng");
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = Path.Combine(ApplicationContext.WebRootPath, @"images\products\", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            if (data.ProductID == 0)
            {
                int id = ProductDataService.AddProduct(data);
                if (id <= 0)
                {
                    //ModelState.AddModelError(nameof(data.ProductName), "Tên sản phẩm trùng");
                    return View("Edit", data);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateProduct(data);

                if (result == false)
                {
                    //ModelState.AddModelError(nameof(data.ProductName), "Tên sản phẩm trùng");
                    return View("Edit", data);
                }

            }
            return RedirectToAction("Index");
        }

        public IActionResult Photo(int id = 0, string method = "", int photoId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                    var data = new ProductPhoto
                    {
                        ProductID = id,
                        IsHidden = false,
                        PhotoID = photoId
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Thay đổi ảnh cho mặt hàng";
                    data = ProductDataService.GetPhoto(photoId);
                    if (data == null)
                    {
                        return RedirectToAction("Edit", new { id = id });
                    }
                    return View(data);
                case "delete":
                    bool result = ProductDataService.DeletePhoto(photoId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult SavePhoto(ProductPhoto data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh cho mặt hàng" : "Thay đổi ảnh cho mặt hàng";
            if (uploadPhoto == null && data.Photo == null)
            {
                ModelState.AddModelError(nameof(data.Photo), "Vui lòng thêm ảnh");
            }
            if (string.IsNullOrWhiteSpace(data.Description))
            {
                ModelState.AddModelError(nameof(data.Description), "Vui lòng nhập mô tả cho ảnh");
            }
            if (data.DisplayOrder <= 0)
            {
                ModelState.AddModelError(nameof(data.DisplayOrder), $"Vui lòng nhập lại thứ tự hiển thị");
            }
            if (!ModelState.IsValid)
            {
                return View("Photo", data);
            }
            string filePath = "";
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                filePath = Path.Combine(ApplicationContext.WebRootPath, "images", "products", fileName);
                data.Photo = fileName;
            }
            if (data.PhotoID == 0)
            {
                long result = ProductDataService.AddPhoto(data);
                if (result > 0)
                {
                    if (uploadPhoto != null)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            uploadPhoto.CopyTo(stream);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã được sử dụng cho bức ảnh khác");
                    return View("Photo", data);
                }
            }
            else
            {
                bool result = ProductDataService.UpdatePhoto(data);
                if (result)
                {
                    if (uploadPhoto != null)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            uploadPhoto.CopyTo(stream);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã được sử dụng cho bức ảnh khác");
                    return View("Photo", data);
                }
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }
        public IActionResult Attribute(int id = 0, string method = "", int attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                    var data = new ProductAttribute
                    {
                        ProductID = id,
                        AttributeID = attributeId
                    };
                    return View(data);
                case "edit":
                    ViewBag.Title = "Thay đổi thuộc tính mặt hàng";
                    data = ProductDataService.GetAttribute(attributeId);
                    if (data == null)
                    {
                        return RedirectToAction("Edit", new { id = id });
                    }
                    return View(data);
                case "delete":
                    bool result = ProductDataService.DeleteAttribute(attributeId);
                    return RedirectToAction("Edit", new { id = id });
                default:
                    return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult SaveAttribute(ProductAttribute data)
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Vui lòng nhập tên thuộc tính");
            if (string.IsNullOrWhiteSpace(data.AttributeValue))
                ModelState.AddModelError(nameof(data.AttributeValue), "Vui lòng nhập giá trị cho thuộc tính");
            if (data.DisplayOrder <= 0)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Vui lòng lại dữ liệu tự hiển thị");
            if (!ModelState.IsValid)
            {
                return View("Attribute", data);
            }
            if (data.AttributeID == 0)
            {
                long result = ProductDataService.AddAttribute(data);
                if (result <= 0)
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã được sử dụng cho thuộc tính khác");
                    return View("Attribute", data);
                }
            }
            else
            {
                bool result = ProductDataService.UpdateAttribute(data);
                if (!result)
                {
                    ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị đã được sử dụng cho thuộc tính khác");
                    return View("Attribute", data);
                }
            }
            return RedirectToAction("Edit", new { id = data.ProductID });
        }
    
    }
}
