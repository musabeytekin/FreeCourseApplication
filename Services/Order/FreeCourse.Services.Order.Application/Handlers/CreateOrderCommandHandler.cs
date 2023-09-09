using FreeCourse.Services.Order.Application.Commands;
using FreeCourse.Services.Order.Application.DTOs;
using FreeCourse.Services.Order.Domain.OrderAggregate;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.DTOs;
using MediatR;

namespace FreeCourse.Services.Order.Application.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Response<CreatedOrderDto>>
{
    private readonly OrderDbContext _context;

    public CreateOrderCommandHandler(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Response<CreatedOrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var newAddress = new Address(request.AddressDto.Province, request.AddressDto.District,
            request.AddressDto.Street, request.AddressDto.ZipCode, request.AddressDto.Line);
        var newOrder = new Domain.OrderAggregate.Order(newAddress, request.BuyerId);

        request.OrderItems.ForEach(x => { newOrder.AddOrderItem(x.ProductId, x.ProductName, x.PictureUrl, x.Price); });

        await _context.Orders.AddAsync(newOrder);
        await _context.SaveChangesAsync();
        return Shared.DTOs.Response<CreatedOrderDto>.Success(new CreatedOrderDto { OrderId = newOrder.Id }, 200);
    }
}