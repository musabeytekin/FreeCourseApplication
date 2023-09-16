using FreeCourse.Web.Models.Order;

namespace FreeCourse.Web.Services.Interfaces;

public interface IOrderService
{
    Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput);
    Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput);
    Task<List<OrderViewModel>> GetOrder();
}