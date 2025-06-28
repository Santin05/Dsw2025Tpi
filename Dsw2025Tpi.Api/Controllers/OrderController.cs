using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Models;
using Dsw2025Tpi.Application.Exceptions;

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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _ordersManagementService.getAllOrders();
                return Ok(orders);
            }
            catch (NoFoundEntityException)
            {
                return NoContent();
            }
        }

        [HttpPost]
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
                return BadRequest("Orden duplicada.");
            }
            catch (NoFoundEntityException) 
            {
                return BadRequest("No existe un producto que está en la orden en la base de datos con el Sku indicado.");
            }
        }

        [HttpPatch]
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
    }
}
