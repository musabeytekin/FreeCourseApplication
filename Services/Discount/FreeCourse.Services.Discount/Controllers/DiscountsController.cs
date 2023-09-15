using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.Services.Discount.Services;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.Discount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountsController : CustomControllerBase
    {
        private readonly IDiscountService _discountService;
        private readonly ISharedIdentityService _sharedIdentityService;
        
        public DiscountsController(IDiscountService discountService, ISharedIdentityService sharedIdentityService)
        {
            _discountService = discountService;
            _sharedIdentityService = sharedIdentityService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var discounts = await _discountService.GetAll();
            return CreateActionResultInstance(discounts);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var discount = await _discountService.GetById(id);
            return CreateActionResultInstance(discount);
        }
        
        [HttpPost]
        public async Task<IActionResult> Save(Models.Discount discount)
        {
            discount.UserId = _sharedIdentityService.GetUserId;
            var response = await _discountService.Save(discount);
            return CreateActionResultInstance(response);
        }
    
        [HttpPut]
        public async Task<IActionResult> Update(Models.Discount discount)
        {
            discount.UserId = _sharedIdentityService.GetUserId;
            var response = await _discountService.Update(discount);
            return CreateActionResultInstance(response);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _discountService.Delete(id);
            return CreateActionResultInstance(response);
        }
        
        [HttpGet]
        [Route("/api/[controller]/code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var userId = _sharedIdentityService.GetUserId;
            var response = await _discountService.GetByCodeAndUserId(code, userId);
            return CreateActionResultInstance(response);
        }
        
        
    }
}
