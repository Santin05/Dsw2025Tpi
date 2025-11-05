using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly ProductsManagementService _productsManagementService;

        public ProductController(ProductsManagementService productsManagementService) 
        {
            _productsManagementService = productsManagementService;
        }

        [HttpGet]
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

        [HttpGet("{id}")]
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

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DisableProduct(Guid id)
        {
            try
            {
                await _productsManagementService.disableProduct(id);
                return NoContent();
            }
            catch (NoFoundEntityException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
