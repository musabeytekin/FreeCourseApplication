using AutoMapper;
using FreeCourse.Services.Order.Application.DTOs;

namespace FreeCourse.Services.Order.Application.Mapping;

public class CustomMapping : Profile
{
    public CustomMapping()
    {
        CreateMap<Order.Domain.OrderAggregate.Order, OrderDto>().ReverseMap();
        CreateMap<Order.Domain.OrderAggregate.OrderItem, OrderItemDto>().ReverseMap();
        CreateMap<Order.Domain.OrderAggregate.Address, AddressDto>().ReverseMap();
    }
}