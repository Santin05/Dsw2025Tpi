using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Models;
using Dsw2025Tpi.Application.Exceptions;
using System.Linq.Expressions;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Dsw2025Tpi.Application.Services
{
    public class OrdersManagementService
    {
        private readonly IRepository _repository;
        public OrdersManagementService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<OrderModel.Response> addOrder(OrderModel.Request order) 
        {
            if( String.IsNullOrWhiteSpace(order.ShipppingAddress) || String.IsNullOrWhiteSpace(order.BillingAddress) || order.ItemsOrder == null ) 
            {
                throw new ArgumentException("Los datos ingresados de la orden no son válidos.");
            }
            else 
            {
                bool b = false, c = false;
                decimal TotalAmount = 0;
                OrderItem itemInOrder;
                List<OrderItem> allItems = new List<OrderItem>();
                List<Product> allProductsUpdate = new List<Product>();
                var allProducts = await _repository.GetAll<Product>();
                var orderAdd = new Order();

                foreach (var q in order.ItemsOrder) 
                {
                    b = false;
                    if(q.quantity <= 0) 
                    {
                        throw new ArgumentException("Los datos ingresados de los productos de la orden no son válidos.");
                    }
                    foreach (Product p in allProducts) 
                    {
                        if (p.Id == q.productId && p.name == q.name && q.currentUnitPrice > 0)
                        {
                            if(q.quantity > p.stockQuantity) 
                            {
                                throw new ArgumentException("No hay suficiente cantidad de productos para la orden.");
                            }
                            if(p.isActive == false)
                            {
                                throw new ArgumentException("Existen productos inhabilitados en la orden.");
                            }

                            c = false;

                            itemInOrder = new OrderItem();
                            itemInOrder.skuProduct = p.sku;
                            itemInOrder.quantity = q.quantity;
                            itemInOrder.unitPrice = q.currentUnitPrice;
                            itemInOrder.subTotal = (q.currentUnitPrice * q.quantity);
                            itemInOrder.orderId = orderAdd.Id;

                            TotalAmount += itemInOrder.subTotal;

                            b = true;

                            foreach (Product j in allProductsUpdate.ToList())
                            {
                                if (j.Id == q.productId && j.name == q.name)
                                {
                                    j.stockQuantity = (j.stockQuantity - q.quantity);
                                    if(j.stockQuantity < 0) 
                                    {
                                        throw new ArgumentException("No hay suficiente cantidad de productos para la orden.");
                                    }
                                    allProductsUpdate.Add(j);
                                    c = true;
                                }
                            }

                            if (c == false) 
                            {
                                p.stockQuantity = (p.stockQuantity - q.quantity);
                                allProductsUpdate.Add(p);
                            }
                            allItems.Add(itemInOrder);

                        }
                    }

                    if (b == false) 
                    {
                        throw new NoFoundEntityException("No existe un producto que está en la orden en la base de datos con el Sku indicado.");
                    }
                }

                orderAdd.date = DateTime.Now;
                orderAdd.shippingAddress = order.ShipppingAddress;
                orderAdd.billlingAddress = order.BillingAddress;
                orderAdd.notes = order.Notes;
                orderAdd.totalAmount = TotalAmount;
                orderAdd.customerId = order.customerId;
                orderAdd.orderItems = allItems;
                foreach (Product p in allProductsUpdate)
                {
                    await _repository.Update<Product>(p);
                }
                foreach (OrderItem o in allItems)
                {
                    await _repository.Add(o);
                }
                await _repository.Add(orderAdd);
                return new OrderModel.Response(orderAdd.Id);
            }
        }

        public async Task<IEnumerable<Order>?> getAllOrders()
        {
            var orders = await _repository.GetAll<Order>();
            if (orders.Equals(null) || !orders.Any())
            {
                throw new NoFoundEntityException("Ningun producto cargado/disponible.");
            }
            else
            {
                foreach (var order in orders) 
                {
                    var orderItems = await _repository.GetAll<OrderItem>();
                    List<OrderItem> allItems = new List<OrderItem>();
                    foreach (var orderItem in orderItems) 
                    {
                        if(order.Id == orderItem.orderId) 
                        {
                            allItems.Add(orderItem);
                        }
                    }
                    order.orderItems = allItems;
                }
                return orders;
            }
        }

        public async Task deleteOrder(Guid id)
        {
            var orderById = await _repository.GetById<Order>(id);
            if (orderById != null)
            {
                await _repository.Delete(orderById);
            }
            else
            {
                throw new NoFoundEntityException("Orden a inhabilitar no cargado/disponible.");
            }
        }
    }
}
