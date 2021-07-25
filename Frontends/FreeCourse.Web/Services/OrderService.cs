using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Baskets;
using FreeCourse.Web.Models.FakePayments;
using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class OrderService
        : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IPaymentService _paymentService;
        private readonly IBasketService _basketService;
        private readonly ISharedIdentityService _sharedIdentityService;

        public OrderService(HttpClient httpClient, IPaymentService paymentService, IBasketService basketService, ISharedIdentityService sharedIdentityService)
        {
            _httpClient = httpClient;
            _paymentService = paymentService;
            _basketService = basketService;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput)
        {
            BasketViewModel basket = await _basketService.Get();
            PaymentInfoInput paymentInfoInput = new()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                CVV = checkoutInfoInput.CVV,
                Expiration = checkoutInfoInput.Expiration,
                TotalPrice = basket.TotalPrice
            };

            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);
            if (!responsePayment)
            {
                return new OrderCreatedViewModel
                {
                    Error = "Ödeme alınamadı",
                    IsSuccessful = false
                };
            }

            var orderCreateInput = new OrderCreateInput
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput
                {
                    District = checkoutInfoInput.District,
                    Line = checkoutInfoInput.Line,
                    Province = checkoutInfoInput.Province,
                    Street = checkoutInfoInput.Street,
                    ZipCode = checkoutInfoInput.ZipCode
                }
            };
            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput
                {
                    ProductId = x.CourseId,
                    Price = x.GetCurrentPrice,
                    //Course servisi üzerinden picture alınabilir
                    PictureUrl = "",
                    ProductName = x.CourseName
                };
                orderCreateInput.OrderItems.Add(orderItem);
            });

            var response = await _httpClient.PostAsJsonAsync("orders", orderCreateInput);
            if (!response.IsSuccessStatusCode)
            {
                return new OrderCreatedViewModel { Error = "Sipariş oluşturulamadı", IsSuccessful = false };
            }

            var orderCreatedViewModel = await response.Content.ReadFromJsonAsync<Response<OrderCreatedViewModel>>();
            orderCreatedViewModel.Data.IsSuccessful = true;
            await _basketService.Delete();
            return orderCreatedViewModel.Data;
        }

        public async Task<List<OrderViewModel>> GetOrder()
        {
            var response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("orders");
            return response.Data;
        }

        public async Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput)
        {
            BasketViewModel basket = await _basketService.Get();
            var orderCreateInput = new OrderCreateInput
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput
                {
                    District = checkoutInfoInput.District,
                    Line = checkoutInfoInput.Line,
                    Province = checkoutInfoInput.Province,
                    Street = checkoutInfoInput.Street,
                    ZipCode = checkoutInfoInput.ZipCode
                }
            };

            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput
                {
                    ProductId = x.CourseId,
                    Price = x.GetCurrentPrice,
                    //Course servisi üzerinden picture alınabilir
                    PictureUrl = "",
                    ProductName = x.CourseName
                };
                orderCreateInput.OrderItems.Add(orderItem);
            });

            PaymentInfoInput paymentInfoInput = new()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                CVV = checkoutInfoInput.CVV,
                Expiration = checkoutInfoInput.Expiration,
                TotalPrice = basket.TotalPrice,
                Order = orderCreateInput
            };

            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);
            if (!responsePayment)
            {
                return new OrderSuspendViewModel
                {
                    Error = "Ödeme alınamadı",
                    IsSuccessful = false
                };
            }

            await _basketService.Delete();
            return new OrderSuspendViewModel { IsSuccessful = true };
        }
    }
}
