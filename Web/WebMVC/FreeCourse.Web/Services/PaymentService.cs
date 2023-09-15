using FreeCourse.Web.Models.Payment;
using FreeCourse.Web.Services.Interfaces;

namespace FreeCourse.Web.Services;

public class PaymentService : IPaymentService
{

    private readonly HttpClient _httpClient;

    public PaymentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ReceivePayment(PaymentInfoInput paymentInfoInput)
    {
        var response = await _httpClient.PostAsJsonAsync("payment", paymentInfoInput);

        return response.IsSuccessStatusCode;
    }
}