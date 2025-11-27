using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Dsw2025Tpi.Application.Models;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly ProductsManagementService _productsManagementService;

        public ProductController(ProductsManagementService productsManagementService) 
        {
            _productsManagementService = productsManagementService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllProducts()
        {
            try 
            {
                var products = await _productsManagementService.getAllProducts();
                return Ok(products);
            }
            catch (NoFoundEntityException) 
            {
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }

        }

        [HttpGet("filtered")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllFilteredProducts(
            string? searchName,
            string? status,
            int pageNumber = 1,
            int pageSize = 20)
        {
            try
            {
                var filteredProducts = await _productsManagementService.getAllFilteredProducts(searchName, status, pageNumber, pageSize);
                return Ok(filteredProducts);
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
        [Authorize(Roles = "Admin, User")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(Guid id) 
        {
            try
            {
                var products = await _productsManagementService.getProductById(id);
                return Ok(products);
            }
            catch (NoFoundEntityException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddProduct([FromBody]ProductModel.Request data) 
        {
            try 
            {
                var product = await _productsManagementService.addProduct(data);
                return Created($"/api/products/{product.Id}", product);
            }
            catch(DuplicateEntityException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.Request data) 
        {
            try 
            {
                await _productsManagementService.upgrateProduct(id, data);
                return Ok("Producto modificado con exito.");
            }
            catch(DuplicateEntityException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch(NoFoundEntityException ex) 
            {
                return NotFound(ex.Message);
            }
            catch(ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(Guid id) 
        {
            try
            {
                await _productsManagementService.deleteProduct(id);
                return Ok("Producto eliminado existosamente de la base de datos.");
            }
            catch(NoFoundEntityException) 
            {
                return NotFound($"No hay producto con ID {id}");
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DisableProduct(Guid id)
        {
            try
            {
                var product = await _productsManagementService.getProductById(id);
                if (product.isActive == true) 
                {
                    await _productsManagementService.disableProduct(id);
                    return NoContent();
                }
                else 
                {
                    await _productsManagementService.enableProduct(id);
                    return NoContent();
                }
            }
            catch (NoFoundEntityException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
