using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020581.BusinessLayers;
using SV21T1020581.DomainModels;
using SV21T1020581.Web.Models;
using System.Buffers;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Reflection;

namespace SV21T1020581.Web.Controllers
{
    [Authorize(Roles = $"{WebUserRoles.ADMINSTATOR}, {WebUserRoles.EMPLOYEE}")]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";

        private const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";

        private const string SHOPPING_CART = "ShoppingCart";

        public IActionResult Index()
        {
            OrderSearchInput? condition = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (condition == null)
            {
                var cultureInfo = new CultureInfo("en-US");
                condition = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    TimeRange = $"{DateTime.Today.AddDays(-7).ToString("dd-MM-yyyy", cultureInfo)} - {DateTime.Today.ToString("dd-MM-yyyy", cultureInfo)}"
                };
            }

            return View(condition);
        }
        public IActionResult Search(OrderSearchInput condition)
        {
            int rowCount;
            var data = OrderDataService.ListOrders(out rowCount, condition.Page, condition.PageSize, condition.Status, condition.FromTime, condition.ToTime, condition.SearchValue ?? "");
            OrderSearchResult model = new OrderSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                Status = condition.Status,
                TimeRange = condition.TimeRange,
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, condition);

            return View(model);
        }
        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var detail = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = detail
            };

            return View(model);
        }
        public IActionResult Create()
        {
            var condition = ApplicationContext.GetSessionData<ProductSearchInput>(PRODUCT_SEARCH_CONDITION);
            if(condition == null)
            {
                condition = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PRODUCT_PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(condition);
        }
        public IActionResult SearchProduct(ProductSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(out rowCount, condition.Page, condition.PageSize, condition.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };

            ApplicationContext.SetSessionData(PRODUCT_SEARCH_CONDITION, condition);
            return View(model);
        }
        public IActionResult Init(int customerID = 0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingCart = GetShoppingCart();
            if(shoppingCart.Count == 0)
            {
                return Json("Giỏ hàng trống. Vui lòng chọn mặt hàng cần bán");
            }

            if(customerID == 0 || string.IsNullOrWhiteSpace(deliveryProvince) || string.IsNullOrWhiteSpace(deliveryAddress))
            {
                return Json("Vui lòng nhập đầy đủ thông tin khách hàng và nơi giao hàng");
            }

            int employeeID = Convert.ToInt32(User.GetUserData()?.UserId);

            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach(var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                });
            }

            int orderID = OrderDataService.InitOrder(employeeID, customerID, deliveryProvince, deliveryAddress, orderDetails);
            ClearCart();
            return Json(orderID);
        }
        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if(shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }
        public IActionResult AddToCart(CartItem item)
        {
            if (item.SalePrice < 0 || item.Quantity <= 0)
                return Json("Giá bán và số lượng không hợp lệ");

            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID  == item.ProductID);
            if(existsProduct == null)
            {
                shoppingCart.Add(item);
            }
            else
            {
                existsProduct.Quantity += item.Quantity;
                existsProduct.SalePrice += item.SalePrice;
            }

            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingCart = GetShoppingCart();
            int index = shoppingCart.FindIndex(m => m.ProductID == id);
            if (index > 0)
                shoppingCart.RemoveAt(index);
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ClearCart()
        {
            var shoppingCart = GetShoppingCart();
            shoppingCart.Clear();
            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            return Json("");
        }
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        public IActionResult Accept(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order != null)
            {
                bool result = OrderDataService.AcceptOrder(id);
                if (result)
                {
                    order = OrderDataService.GetOrder(id);
                    var listOrderDetail = OrderDataService.ListOrderDetails(id);

                    var data = new OrderDetailModel() { Order = order, Details = listOrderDetail };
                    return View("Details", data);
                }
            }
            return View("Index");
        }
        public IActionResult Finish(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if(order != null)
            {
                bool result = OrderDataService.FinishOrder(id);
                if (result)
                {
                    order = OrderDataService.GetOrder(id);
                    var listOrderDetail = OrderDataService.ListOrderDetails(id);

                    var data = new OrderDetailModel() { Order = order, Details = listOrderDetail };
                    return View("Details", data);
                }
            }

            return View("Index");
        }
        public IActionResult Cancel(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order != null)
            {
                bool result = OrderDataService.CancelOrder(id);
                if (result)
                {
                    order = OrderDataService.GetOrder(id);
                    var listOrderDetail = OrderDataService.ListOrderDetails(id);

                    var data = new OrderDetailModel() { Order = order, Details = listOrderDetail };
                    return View("Details", data);
                }
            }

            return View("Index");
        }
        public IActionResult Reject(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order != null)
            {
                bool result = OrderDataService.RejectOrder(id);
                if (result)
                {
                    order = OrderDataService.GetOrder(id);
                    var listOrderDetail = OrderDataService.ListOrderDetails(id);

                    var data = new OrderDetailModel() { Order = order, Details = listOrderDetail };
                    return View("Details", data);
                }
            }

            return View("Index");
        }
        public IActionResult Shipping(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if(order == null) { return View("Index"); }
            else
            {
                return View(order);
            }
        }
        [HttpPost]
        public IActionResult Shipping(int id = 0, int shipperID = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if(order == null) { return RedirectToAction("Index"); }
            else
            {
                if(shipperID > 0)
                {
                    bool result = OrderDataService.ShipOrder(id, shipperID);
                    if(result)
                    {
                        var listOrderDetail = OrderDataService.ListOrderDetails(id);
                        var orderDetailModel = new OrderDetailModel()
                        {
                            Order = order,
                            Details = listOrderDetail
                        };

                        return View("Details", orderDetailModel);
                    }
                }
            }

            return View("Index");
        }
        public IActionResult Delete(int id)
        {
            var order = OrderDataService.GetOrder(id);
            if(order != null)
            {
                bool result = OrderDataService.DeleteOrder(id);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var listOrderDetail = OrderDataService.ListOrderDetails(id);
                    var data = new OrderDetailModel() { Order = order, Details = listOrderDetail };
                    return View("Details", data);
                }
            }
            return View("Index");
        }
        public IActionResult DeleteDetail(int id, int productId)
        {
            var order = OrderDataService.GetOrder(id);
            var listOrderDetails = OrderDataService.ListOrderDetails(id);

            if (order?.Status == Constants.ORDER_INIT || order?.Status == Constants.ORDER_ACCEPTED)
            {
                OrderDataService.DeleteOrderDetail(id, productId);

                var data = new OrderDetailModel() { Order = order, Details = listOrderDetails };
                if (OrderDataService.ListOrderDetails(id).Count == 0)
                {
                    OrderDataService.DeleteOrder(id);
                    return RedirectToAction("Index");
                }
                return View("Details", data);

            }
            return View("Index");
        }
        [HttpPost]
        public IActionResult UpdateDetail(int id, OrderDetail data)
        {
            var order = OrderDataService.GetOrder(id);

            if (id > 0)
            {
                bool result = OrderDataService.SaveOrderDetail(id, data.ProductID, data.Quantity, data.SalePrice);
                if (result)
                {
                    var details = OrderDataService.ListOrderDetails(id);
                    var orderDetail = new OrderDetailModel() { Order = order, Details = details };
                    return View("Details", orderDetail);
                }
            }

            return View("Index");
        }
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var data = OrderDataService.GetOrderDetail(id, productId);
            if (data == null)
            {
                return RedirectToAction("Index");
            }
            return View(data);
        }
    }
}
