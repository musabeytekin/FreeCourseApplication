using FreeCourse.Services.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.FakePayment.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : CustomControllerBase
{
    [HttpPost]
    public IActionResult ReceivePayment(PaymentDto paymentDto)
    {
        return CreateActionResultInstance(Response<NoContent>.Success(204));
    }
}