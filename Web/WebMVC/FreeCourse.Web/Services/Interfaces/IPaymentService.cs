using FreeCourse.Web.Models.Payment;

namespace FreeCourse.Web.Services.Interfaces;

public interface IPaymentService
{
    Task<bool> ReceivePayment(PaymentInfoInput paymentInfoInput);
}