using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;

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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductsById(Guid id) 
        {
            try
            {
                var products = await _productsManagementService.getProductById(id);
                return Ok(products);
            }
            catch (NoFoundEntityException)
            {
                return NoContent();
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
                return Ok(product);
            }
            catch(DuplicateEntityException) 
            {
                return BadRequest("Producto duplicado.");
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] ProductModel.Request data) 
        {
            try 
            {
                await _productsManagementService.upgrateProduct(id, data);
                return Ok("Producto modificado con exito.");
            }
            catch(DuplicateEntityException) 
            {
                return BadRequest("Producto duplicado.");
            }
            catch(NoFoundEntityException) 
            {
                return NotFound($"No hay producto con ID {id}");
            }
            catch(ArgumentException) 
            {
                return BadRequest("Los nuevos valores del producto no son validos.");
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
            catch (NoFoundEntityException)
            {
                return NotFound($"No hay producto con ID {id}");
            }
        }
    }
}
