using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Messages;
using MassTransit;

namespace FreeCourse.Services.Order.Application.Consumers;

public class CreateOrderMessageCommandConsumer : IConsumer<CreateOrderMessageCommand>
{
    private readonly OrderDbContext _orderDbContext;

    public CreateOrderMessageCommandConsumer(OrderDbContext orderDbContext)
    {
        _orderDbContext = orderDbContext;
    }

    public async Task Consume(ConsumeContext<CreateOrderMessageCommand> context)
    {
        var newAddress = new Domain.OrderAggregate.Address(
            context.Message.Province,
            context.Message.District,
            context.Message.Street,
            context.Message.ZipCode,
            context.Message.Line);
        
        var order = new Domain.OrderAggregate.Order(newAddress,context.Message.BuyerId);

        context.Message.OrderItems.ForEach(x =>
        {
            order.AddOrderItem(x.ProductId,x.ProductName,x.PictureUrl,x.Price);
        });

        await _orderDbContext.Orders.AddAsync(order);
        
        await _orderDbContext.SaveChangesAsync();
        
    }
}