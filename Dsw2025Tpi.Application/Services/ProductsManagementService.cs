using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Application.Dtos;
using static Dsw2025Tpi.Application.Dtos.ProductModel;
using System.Data;
using Dsw2025Tpi.Application.Exceptions;
using System.ComponentModel;

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
                string.IsNullOrWhiteSpace(product.Name) || string.IsNullOrWhiteSpace(product.Description) || 
                product.StockQuantity < 0 || product.CurrentUnitPrice <= 0 ) 
            {
                throw new ArgumentException("Valores no validos para un producto.");
            }

            if(product.Sku==null || product.Name == null || product.InternalCode == null || product.Description == null || 
               product.StockQuantity == null || product.CurrentUnitPrice == null) 
            {
                throw new ArgumentException("Valores no validos para un producto.");
            }

            var productFound = await _repository.First<Product>(p => p.sku == product.Sku);
            if (productFound != null) 
            {
                throw new DuplicateEntityException($"Producto con Sku {product.Sku} ya existente.");
            }

            var productAdd = new Product(product.Sku ,product.Name, product.Description, product.InternalCode, (int) product.CurrentUnitPrice, (int) product.StockQuantity);
            await _repository.Add(productAdd);
            return new ProductModel.Response(productAdd.Id);
        }

        public async Task<IEnumerable<Product>?> getAllProducts()
        {
            IEnumerable<Product?> products;
            try
            {
                products = await _repository.GetAll<Product>();
            }
            catch(NotImplementedException) 
            {
                throw new NoFoundEntityException($"{AppContext.BaseDirectory}");
            }
            if (products.Equals(null)) 
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
                throw new NoFoundEntityException("Ningun producto cargado/disponible.");
            }
        }

        public async Task upgrateProduct(Guid id, ProductModel.Request product) 
        {
            var productById = await _repository.GetById<Product>(id);

            if (productById != null)
            {
                if (await _repository.First<Product>(p => p.sku == product.Sku && p.Id != id) == null)
                {
                    //Verificacion de cada campo de la request
                    if (!String.IsNullOrEmpty(product.Sku))
                    {
                        if (string.IsNullOrWhiteSpace(product.Sku))
                        {
                            throw new ArgumentException("Valores no validos para un producto.");
                        }
                        productById.sku = product.Sku;
                    }

                    if (!String.IsNullOrEmpty(product.Name))
                    {
                        if (string.IsNullOrWhiteSpace(product.Name))
                        {
                            throw new ArgumentException("Valores no validos para un producto.");
                        }
                        productById.name = product.Name;
                    }

                    if (!String.IsNullOrEmpty(product.InternalCode))
                    {
                        if (string.IsNullOrWhiteSpace(product.InternalCode))
                        {
                            throw new ArgumentException("Valores no validos para un producto.");
                        }
                        productById.internalCode = product.InternalCode;
                    }

                    if (!String.IsNullOrEmpty(product.Description))
                    {
                        if (string.IsNullOrWhiteSpace(product.Description))
                        {
                            throw new ArgumentException("Valores no validos para un producto.");
                        }
                        productById.description = product.Description;
                    }

                    if (product.StockQuantity != null)
                    {
                        if (!(product.StockQuantity < 0))
                        {
                            if (product.StockQuantity != 0) 
                            {
                                productById.stockQuantity = (int)product.StockQuantity;
                            }
                        }
                        else
                        {
                            throw new ArgumentException("Valores no validos para un producto.");
                        }
                    }

                    if (product.CurrentUnitPrice != null)
                    {
                        if (!(product.CurrentUnitPrice < 0)) 
                        {
                            productById.currentUnitPrice = (int)product.CurrentUnitPrice;
                        }
                        else
                        {
                            throw new ArgumentException("Valores no validos para un producto.");
                        }
                    }

                    await _repository.Update<Product>(productById);
                }
                else
                {
                    throw new DuplicateEntityException($"Producto con el mismo Sku {product.Sku} encontrado en la base de datos.");
                }
            }
            else
            {
                throw new NoFoundEntityException("Producto a actualizar no cargado/disponible.");
            }





            /*
            var productById = await _repository.GetById<Product>(id);
            if (productById != null)
            {
                if(await _repository.First<Product>(p => p.sku == product.Sku && p.Id != id) == null) 
                {
                    if (string.IsNullOrWhiteSpace(product.Sku) || string.IsNullOrWhiteSpace(product.InternalCode) ||
                        string.IsNullOrWhiteSpace(product.Name) || string.IsNullOrWhiteSpace(product.Description) ||
                        product.StockQuantity < 0 || product.CurrentUnitPrice <= 0)
                    {
                        throw new ArgumentException("Valores no validos para un producto.");
                    }
                    else 
                    {
                        productById.sku = product.Sku;
                        productById.name = product.Name;
                        productById.description = product.Description;
                        productById.internalCode = product.InternalCode;
                        productById.currentUnitPrice = product.CurrentUnitPrice;
                        productById.stockQuantity = product.StockQuantity;
                        await _repository.Update(productById);
                     }
                }
                else 
                {
                    throw new DuplicateEntityException($"Producto con el mismo Sku {product.Sku} encontrado en la base de datos.");
                }
            }
            else
            {
                throw new NoFoundEntityException("Producto a actualizar no cargado/disponible.");
            }
            */
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
