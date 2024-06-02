using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky.Infrastructure;
using Rocky.Services;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models.Models;
using Rocky_Models.ViewModels;
using Rocky_Utility;

namespace Rocky.Controllers
{
    [OwnAuthorize(WC.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IUserInteractionService _userInteractionService;
        private readonly IUserService _userService;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IOrderHeaderRepository orderHeaderRepository, 
            IOrderDetailRepository orderDetailRepository, 
            IUserInteractionService userInteractionService, IUserService userService)
        {
            _orderHeaderRepository = orderHeaderRepository;
            _orderDetailRepository = orderDetailRepository;
            _userInteractionService = userInteractionService;
            _userService = userService;
        }

        public IActionResult Index(string searchName = null, string searchEmail = null, string searchPhone = null, string Status = null)
        {
            OrderListVM orderListVM = new OrderListVM
            {
                OrderHeaderList = _orderHeaderRepository.GetAll(),
                StatusList = WC.listStatus.ToList().Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };

            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }
            if (!string.IsNullOrEmpty(Status) && Status != "--Order Status--")
            {
                orderListVM.OrderHeaderList = orderListVM.OrderHeaderList.Where(u => u.OrderStatus.ToLower().Contains(Status.ToLower()));
            }

            return View(orderListVM);
        }

        public IActionResult Details(int id)
        {
            OrderVM = new OrderVM
            {
                OrderHeader = _orderHeaderRepository.FirstOrDefault(u => u.Id == id),
                OrderDetail = _orderDetailRepository.GetAll(o => o.OrderHeaderId == id, includeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WC.StatusInProcess;
            _orderHeaderRepository.Save();
            TempData[WC.Success] = "Order is In Process";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WC.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;

            // Log the purchase interactions
            LogPurchaseInteractions(orderHeader.Id);

            _orderHeaderRepository.Save();
            TempData[WC.Success] = "Order Shipped Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);

            orderHeader.OrderStatus = WC.StatusRefunded;
            _orderHeaderRepository.Save();
            TempData[WC.Success] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _orderHeaderRepository.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.FullName = OrderVM.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFromDb.Email = OrderVM.OrderHeader.Email;

            _orderHeaderRepository.Save();
            TempData[WC.Success] = "Order Details Updated Successfully";

            return RedirectToAction("Details", "Order", new { id = orderHeaderFromDb.Id });
        }

        // Log purchase interactions
        private void LogPurchaseInteractions(int orderHeaderId)
        {
            var orderDetails = _orderDetailRepository.GetAll(od => od.OrderHeaderId == orderHeaderId, includeProperties: "Product");
            var userId = _userService.GetUserId();

            foreach (var detail in orderDetails)
            {
                var interaction = new UserInteraction
                {
                    UserId = userId,
                    ProductId = detail.ProductId,
                    InteractionType = "purchase",
                    InteractionTime = DateTime.Now
                };
                _userInteractionService.LogInteraction(interaction);
            }
        }
    }
}
