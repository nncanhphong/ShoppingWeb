using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21T1020581.BusinessLayers;
using SV21T1020581.DomainModels;
using SV21T1020581.Shop.Models;
using System.Net.WebSockets;

namespace SV21T1020581.Shop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";

        private const int PRODUCT_PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH_CONDITION = "ProductSearchForSale";

        private const string SHOPPING_CART = "ShoppingCart";

        private const int BOT_EMPLOYEE = 1037;

        private List<CartItem> GetShoppingCart()
        {
            var shoppingCart = ApplicationContext.GetSessionData<List<CartItem>>(SHOPPING_CART);
            if (shoppingCart == null)
            {
                shoppingCart = new List<CartItem>();
                ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);
            }
            return shoppingCart;
        }

        /// <summary>
        /// Giỏ hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult AddToCart(int id = 0)
        {
            var data = ProductDataService.GetProduct(id);
            if(data == null)
            {
                return RedirectToAction("Index", "Product");
            }
            var newCartItem = new CartItem
            {
                ProductID = id,
                ProductName = data.ProductName,
                Photo = data.Photo,
                Unit = data.Unit,
                Quantity = 1,
                SalePrice = data.Price,
            };

            var shoppingCart = GetShoppingCart();
            var existsProduct = shoppingCart.FirstOrDefault(m => m.ProductID == newCartItem.ProductID);
            if (existsProduct == null)
            {
                shoppingCart.Add(newCartItem);
            }
            else
            {
                existsProduct.Quantity += newCartItem.Quantity;
                existsProduct.SalePrice += newCartItem.SalePrice;
            }

            ApplicationContext.SetSessionData(SHOPPING_CART, shoppingCart);

            return RedirectToAction("Index", "Product");
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
        /// <summary>
        /// View Shopping Cart
        /// </summary>
        /// <returns></returns>
        public IActionResult ShoppingCart()
        {
            return View(GetShoppingCart());
        }
        /// <summary>
        /// Tạo hoá đơn
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Payment(string city, string address) 
        {
            var shoppingCart = GetShoppingCart();
            int customerID = Convert.ToInt32(User.GetUserData()?.UserId);
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var item in shoppingCart)
            {
                orderDetails.Add(new OrderDetail()
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    SalePrice = item.SalePrice,
                });
            }
            int orderID = OrderDataService.InitOrder(BOT_EMPLOYEE, customerID, city, address, orderDetails);
            var data = OrderDataService.GetOrder(orderID);
            return View("OrderStatus");
        }

        //TODO: Chưa xong
        /// <summary>
        /// Tình trạng vận chuyển của đơn hàng
        /// </summary>
        /// <returns></returns>
        public IActionResult OrderStatus()
        {
            int customerID = Convert.ToInt32(User.GetUserData()?.UserId);
            var orderHistory = OrderDataService.OrderHistory(customerID);

            if(orderHistory == null)
            {
                return View();
            }

            var listDetail = new List<OrderDetailModel>();

            foreach(var item in orderHistory)
            {
                var order = OrderDataService.GetOrder(item.OrderID);
                var orderDetails = OrderDataService.ListOrderDetails(item.OrderID);

                var orderDetailModel = new OrderDetailModel() { Order = order, Details = orderDetails };
                listDetail.Add(orderDetailModel);
            }

            var data = new ListOrderDetailModel() { CustomerID = customerID, ListOrder = listDetail };

            return View(data);
        }
        /// <summary>
        /// Thông tin thanh toán: bao gồm hoá đơn và thông tin vận chuyển
        /// </summary>
        /// <returns></returns>
        public IActionResult CheckOut()
        {
            var shoppingCart = GetShoppingCart();
            return View(shoppingCart);
        }

        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index", "Home");
            var detail = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = detail
            };

            return View(model);
        }
    }
}
