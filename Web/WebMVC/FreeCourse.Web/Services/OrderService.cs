using FreeCourse.Shared.DTOs;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.Order;
using FreeCourse.Web.Models.Payment;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class OrderService : IOrderService
{
    private readonly IPaymentService _paymentService;
    private readonly HttpClient _httpClient;
    private IBasketService _basketService;
    private readonly ISharedIdentityService _sharedIdentityService;

    public OrderService(IPaymentService paymentService, HttpClient httpClient, IBasketService basketService,
        ISharedIdentityService sharedIdentityService)
    {
        _paymentService = paymentService;
        _httpClient = httpClient;
        _basketService = basketService;
        _sharedIdentityService = sharedIdentityService;
    }

    public async Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput)
    {
        var basket = await _basketService.Get();
        var payment = await _paymentService.ReceivePayment(new PaymentInfoInput()
        {
            CardName = checkoutInfoInput.CardName,
            CardNumber = checkoutInfoInput.CardNumber,
            Expiration = checkoutInfoInput.Expiration,
            CVV = checkoutInfoInput.CVV,
            TotalPrice = basket!.TotalPrice
        });

        if (!payment)
            return new OrderCreatedViewModel() { Error = "An Error Occured", IsSuccessful = false };

        var createOrderInput = new CreateOrderInput()
        {
            BuyerId = _sharedIdentityService.GetUserId,
            Address = new AddressCreateInput
            {
                Province = checkoutInfoInput.Province,
                District = checkoutInfoInput.District,
                Street = checkoutInfoInput.Street,
                ZipCode = checkoutInfoInput.ZipCode,
                Line = checkoutInfoInput.Line
            }
        };

        basket.BasketItems.ForEach(item =>
        {
            var orderItem = new OrderItemCreateInput
            {
                ProductId = item.CourseId,
                Price = item.CurrentPrice,
                PictureUrl = "",
                ProductName = item.CourseName
            };
            createOrderInput.OrderItems.Add(orderItem);
        });

        var response = await _httpClient.PostAsJsonAsync<CreateOrderInput>("orders", createOrderInput);

        if (!response.IsSuccessStatusCode)
            return new OrderCreatedViewModel() { Error = "An Error Occured", IsSuccessful = false };

        var orderCreatedViewModel = await response.Content.ReadFromJsonAsync<Response<OrderCreatedViewModel>>();
        orderCreatedViewModel!.Data.IsSuccessful = true;
        await _basketService.Delete();
        return orderCreatedViewModel.Data;
    }

    public async Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput)
    {
        var basket = await _basketService.Get();
        var createOrderInput = new CreateOrderInput()
        {
            BuyerId = _sharedIdentityService.GetUserId,
            Address = new AddressCreateInput()
            {
                Province = checkoutInfoInput.Province,
                District = checkoutInfoInput.District,
                Street = checkoutInfoInput.Street,
                ZipCode = checkoutInfoInput.ZipCode,
                Line = checkoutInfoInput.Line
            }
        };

        basket.BasketItems.ForEach(item =>
        {
            var orderItem = new OrderItemCreateInput()
            {
                ProductId = item.CourseId,
                Price = item.CurrentPrice,
                PictureUrl = "",
                ProductName = item.CourseName
            };
            createOrderInput.OrderItems.Add(orderItem);
        });

        var paymentInfo = new PaymentInfoInput()
        {
            CardName = checkoutInfoInput.CardName,
            CardNumber = checkoutInfoInput.CardNumber,
            Expiration = checkoutInfoInput.Expiration,
            CVV = checkoutInfoInput.CVV,
            TotalPrice = basket!.TotalPrice,
            Order = createOrderInput
        };

        var paymentResponse = await _paymentService.ReceivePayment(paymentInfo);

        if (!paymentResponse)
            return new OrderSuspendViewModel() { Error = "An Error Occured", IsSuccessful = false };

        return new OrderSuspendViewModel() { IsSuccessful = true };
    }

    public async Task<List<OrderViewModel>> GetOrder()
    {
        var response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("orders");

        return response.Data;
    }
}