﻿using AutoMapper;
using OrderService.Models;
using Shared.DTOs;

namespace OrderService.Mappings
{
    // Профил за AutoMapper – указваме какво и към какво да се мапва
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<CreateOrderDto, Order>();
        }
    }
}
