using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace Dsw2025Tpi.Application.Services
{
    public class ProductsManagementService
    {
        private readonly IRepository _repository;

        public ProductsManagementService(IRepository repository) 
        {
            _repository = repository;
        }

        public async Task<ProductModel.Response> addProduct(ProductModel.Request product) 
        {
            if(string.IsNullOrWhiteSpace(product.Sku) || string.IsNullOrWhiteSpace(product.InternalCode) || 
                string.IsNullOrWhiteSpace(product.Name) || string.IsNullOrWhiteSpace(product.Description)) 
            {
                throw new ArgumentException("Faltan datos del producto a llenar.");
            }

            if(product.StockQuantity == null || product.CurrentUnitPrice == null ||
                product.StockQuantity < 0 || product.CurrentUnitPrice <= 0)
            {
                throw new ArgumentException("Cantidades de Stock y/o Precio no validos para un producto.");
            }

            var productFound = await _repository.First<Product>(p => p.sku == product.Sku);
            if (productFound != null) 
            {
                throw new DuplicateEntityException($"Producto con Sku {product.Sku} ya existente.");
            }

            var productAdd = new Product(product.Sku ,product.Name, product.Description, product.InternalCode, (int) product.CurrentUnitPrice, (int) product.StockQuantity);
            await _repository.Add(productAdd);
            return new ProductModel.Response(productAdd.id);
        }

        public async Task<IEnumerable<Product>?> getAllProducts()
        {
            IEnumerable<Product> products = await _repository.GetAll<Product>();

            if (products.IsNullOrEmpty()) 
            {
                throw new NoFoundEntityException("Ningun producto cargado/disponible.");
            }
            else 
            {
                return products;
            }
        }

        public async Task<Product?> getProductById(Guid id) 
        {
            var productById = await _repository.GetById<Product>(id);
            if (productById != null)
            {
                return productById;
            }
            else 
            {
                throw new NoFoundEntityException($"Ningun producto con ID {id} cargado/disponible.");
            }
        }

        public async Task upgrateProduct(Guid id, ProductModel.Request product) 
        {
            var productById = await _repository.GetById<Product>(id);

            if (productById != null)
            {
                if (await _repository.First<Product>(p => p.sku == product.Sku && p.id != id) == null)
                {
                    //Verificacion de cada campo de la request
                    if (product.Sku != null) 
                    {
                        if (!(string.IsNullOrWhiteSpace(product.Sku)))
                        {
                            productById.sku = product.Sku;
                        }
                        else 
                        {
                            throw new ArgumentException("Sku del producto inexistente.");
                        }
                    }

                    if (product.Name != null)
                    {
                        if (!(string.IsNullOrWhiteSpace(product.Name)))
                        {
                            productById.name = product.Name;
                        }
                        else
                        {
                            throw new ArgumentException("Sku del producto inexistente.");
                        }
                    }

                    if (product.InternalCode != null)
                    {
                        if (!(string.IsNullOrWhiteSpace(product.InternalCode)))
                        {
                            productById.internalCode = product.InternalCode;
                        }
                        else
                        {
                            throw new ArgumentException("Sku del producto inexistente.");
                        }
                    }

                    if (product.Description != null)
                    {
                        if (!(string.IsNullOrWhiteSpace(product.Description)))
                        {
                            productById.description = product.Description;
                        }
                        else
                        {
                            throw new ArgumentException("Sku del producto inexistente.");
                        }
                    }

                    if (product.StockQuantity != null)
                    {
                        if (!(product.StockQuantity < 0))
                        {
                            productById.stockQuantity = (int)product.StockQuantity;
                        }
                        else
                        {
                            throw new ArgumentException("Cantidad de stock menor a cero.");
                        }
                    }

                    if (product.CurrentUnitPrice != null)
                    {
                        if (!(product.CurrentUnitPrice <= 0)) 
                        {
                            productById.currentUnitPrice = (decimal)product.CurrentUnitPrice;
                        }
                        else
                        {
                            throw new ArgumentException("Valor del precio unitario menor/igual a cero.");
                        }
                    }

                    await _repository.Update<Product>(productById);
                }
                else
                {
                    throw new DuplicateEntityException($"Producto con el Sku a modificar ( {product.Sku} ) encontrado en otro producto existente.");
                }
            }
            else
            {
                throw new NoFoundEntityException("Producto a actualizar no cargado/disponible.");
            }
        }

        public async Task deleteProduct(Guid id) 
        {
            var productById = await _repository.GetById<Product>(id);
            if (productById != null)
            {
                await _repository.Delete(productById);
            }
            else
            {
                throw new NoFoundEntityException("Producto a inhabilitar no cargado/disponible.");
            }
        }

        public async Task disableProduct(Guid id)
        {
            var productById = await _repository.GetById<Product>(id);
            if (productById != null)
            {
                productById.isActive = false;
                await _repository.Update(productById);
            }
            else
            {
                throw new NoFoundEntityException("Producto a inhabilitar no cargado/disponible.");
            }
        }
    }
}
