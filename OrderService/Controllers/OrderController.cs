﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;
using Shared.DTOs;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrdersService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrdersService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orderDtos = await _orderService.GetAllAsync();
            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            return Ok(_mapper.Map<OrderDto>(order));
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            var order = _mapper.Map<Order>(createOrderDto);
            var createdOrder = await _orderService.CreateAsync(order);
            var orderDto = _mapper.Map<OrderDto>(createdOrder);

            return CreatedAtAction(nameof(GetOrderById), new { id = orderDto.Id }, orderDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto dto)
        {
            var updated = await _orderService.UpdateAsync(id, dto.TotalAmount);
            if (updated == null)
                return NotFound();

            return Ok(_mapper.Map<OrderDto>(updated));
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateOrder(int id, decimal totalAmount)
        //{
        //    var updated = await _orderService.UpdateAsync(id, totalAmount);
        //    if (updated == null)
        //        return NotFound();

        //    return Ok(_mapper.Map<OrderDto>(updated));
        //}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var deleted = await _orderService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
