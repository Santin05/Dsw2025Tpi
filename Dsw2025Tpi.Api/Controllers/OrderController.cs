using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Models;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        
        private readonly OrdersManagementService _ordersManagementService;

        public OrderController(OrdersManagementService ordersManagementService)
        {
            _ordersManagementService = ordersManagementService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllOrders([FromQuery] Guid? customerId)
        {
            try
            {
                if (customerId.HasValue)
                {
                    var orders = await _ordersManagementService.getOrdersByCustomersId(customerId.Value);
                    return Ok(orders);
                }
                else 
                {
                    var orders = await _ordersManagementService.getAllOrders();
                    return Ok(orders);
                }
            }
            catch (NoFoundEntityException)
            {
                return NoContent();
            }
        }

        [HttpGet("filtered")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllFilteredOrders(
            string? status,
            int pageNumber = 1,
            int pageSize = 20)
        {
            try
            {
                var filteredOrders = await _ordersManagementService.getAllFilteredOrders(status, pageNumber, pageSize);
                return Ok(filteredOrders);
            }
            catch (NoFoundEntityException)
            {
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var order = await _ordersManagementService.getOrderById(id);
                return Ok(order);
            }
            catch (NoFoundEntityException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddOrder([FromBody] OrderModel.Request data) 
        {
            try
            {
                var order = await _ordersManagementService.addOrder(data);
                return Ok(data);
            }
            catch (DuplicateEntityException)
            {
                return Conflict("Orden duplicada.");
            }
            catch (NoFoundEntityException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch(Exception e) 
            {
                return Problem(e.Message);
            }
        }

        [HttpPatch]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            try
            {
                await _ordersManagementService.deleteOrder(id);
                return Ok("Orden eliminada existosamente de la base de datos.");
            }
            catch (NoFoundEntityException)
            {
                return NotFound($"No hay orden con ID {id}");
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderUpdateModel status)
        {
            try
            {
                await _ordersManagementService.upgrateOrderStatus(id, status);
                return Ok($"Status de orden modificado a {status.newStatus.ToUpper()} con exito.");
            }
            catch (DuplicateEntityException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NoFoundEntityException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /*
        [HttpGet("Customers {id}")]
        [Authorize(Roles = "Customer,Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderByCustomersId(Guid id)
        {
            try
            {
                var order = await _ordersManagementService.getOrdersByCustomersId(id);
                return Ok(order);
            }
            catch (NoFoundEntityException ex)
            {
                return NotFound(ex.Message);
            }
        }
        */
    }
}
