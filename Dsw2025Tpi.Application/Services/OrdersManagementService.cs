using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Models;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Dtos;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;

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

            if ( String.IsNullOrWhiteSpace(order.shippingAddress) || String.IsNullOrWhiteSpace(order.billingAddress) || order.orderItems == null ) 
            {
                throw new ArgumentException("Los datos ingresados de la orden no son válidos.");
            }

            if( await _repository.First<Customer>(p => p.id == order.customerId) == null) 
            {
                throw new ArgumentException($"Cliente con el ID {order.customerId} no encontrado en la base de datos.");
            }
            else 
            {
                OrderModel.Item itemDuplicate, itemNew;
                var itemsOrderFinish = new List<OrderModel.Item>();
                foreach (var q in order.orderItems.ToList()) 
                {
                    if(itemsOrderFinish.Any()==false)
                    {
                        itemsOrderFinish.Add(q);
                    }
                    else 
                    {
                        if(itemsOrderFinish.Exists(p => p.productId == q.productId))
                        {
                            itemDuplicate = itemsOrderFinish.Find(p => p.productId == q.productId);
                            itemsOrderFinish.Remove(itemDuplicate);
                            itemNew = new OrderModel.Item(itemDuplicate.productId, (itemDuplicate.quantity+q.quantity), itemDuplicate.name, itemDuplicate.description, itemDuplicate.currentUnitPrice);
                            itemsOrderFinish.Add(itemNew);
                        }
                        else 
                        {
                            itemsOrderFinish.Add(q);
                        }
                    }
                }
                decimal TotalAmount = 0;
                OrderItem itemInOrder;
                List<OrderItem> allItems = new List<OrderItem>();
                List<Product> allProductsUpdate = new List<Product>();
                var allProducts = await _repository.GetAll<Product>();
                var orderAdd = new Order();

                foreach (var q in itemsOrderFinish) 
                {
                    if(q.quantity <= 0) 
                    {
                        throw new ArgumentException("Cantidad de uno de los productos en la orden menor/igual a cero. Por favor, revise los productos de la orden e intentelo nuevamente.");
                    }
                    if (!(allProducts.ToList().Exists(p => ( p.id == q.productId && p.name == q.name && p.currentUnitPrice == q.currentUnitPrice ))))
                    {
                        throw new NoFoundEntityException($"No existe un producto que está en la orden con el ID, Nombre o precio indicado.");
                    }
                    foreach (Product p in allProducts) 
                    {
                        if (p.id == q.productId && p.name == q.name && p.currentUnitPrice == q.currentUnitPrice)
                        {
                            if(q.quantity > p.stockQuantity) 
                            {
                                throw new ArgumentException("No hay suficiente cantidad de productos para la orden.");
                            }
                            if(p.isActive == false)
                            {
                                throw new ArgumentException("Existen productos inhabilitados en la orden.");
                            }

                            itemInOrder = new OrderItem();
                            itemInOrder.skuProduct = p.sku;
                            itemInOrder.quantity = q.quantity;
                            itemInOrder.unitPrice = q.currentUnitPrice;
                            itemInOrder.subTotal = (q.currentUnitPrice * q.quantity);
                            itemInOrder.orderId = orderAdd.id;

                            TotalAmount += itemInOrder.subTotal;

                            allItems.Add(itemInOrder);

                            p.stockQuantity -= q.quantity;
                            allProductsUpdate.Add(p);

                        }
                    }
                }

                orderAdd.date = DateTime.Now;
                orderAdd.shippingAddress = order.shippingAddress;
                orderAdd.billlingAddress = order.billingAddress;
                orderAdd.notes = order.notes;
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
                return new OrderModel.Response(orderAdd.id);
            }
        }

        public async Task<IEnumerable<Order>?> getAllOrders()
        {
            var orders = await _repository.GetAll<Order>();
            if (orders.IsNullOrEmpty())
            {
                throw new NoFoundEntityException("Ninguna orden cargada/disponible.");
            }
            else
            {
                foreach (var order in orders) 
                {
                    var orderItems = await _repository.GetAll<OrderItem>();
                    List<OrderItem> allItems = new List<OrderItem>();
                    foreach (var orderItem in orderItems) 
                    {
                        if(order.id == orderItem.orderId) 
                        {
                            allItems.Add(orderItem);
                        }
                    }
                    order.orderItems = allItems;
                }
                return orders;
            }
        }

        public async Task<PageModel<OrderClient>?> getAllFilteredOrders(string? searchName, string? status, int pageNumber = 1, int pageSize = 20)
        {
            var orders = await _repository.GetAll<Order>();
            if (orders.IsNullOrEmpty()) { throw new NoFoundEntityException("Ninguna orden cargada/disponible."); }
            else
            {
                var filteredOrders = new List<Order>();
                if(pageNumber <= 0 || pageSize <= 0) { throw new ArgumentException("Ingrese un tamaño de página o número de página correcto (Mayor a cero y entero)."); }
                if (!string.IsNullOrWhiteSpace(status)) 
                {
                    status = status.ToUpper();
                    switch (status)
                    {
                        case "PENDING":
                            foreach (var order in orders) 
                            {
                                if (order.status.ToString() == "PENDING") { filteredOrders.Add(order); }
                            }
                            if(filteredOrders.IsNullOrEmpty()) { throw new NoFoundEntityException("Ninguna orden cargada/disponible con el status PENDING."); }
                            break;
                        case "PROCESSING":
                            foreach (var order in orders)
                            {
                                if (order.status.ToString() == "PROCESSING") { filteredOrders.Add(order); }
                            }
                            if (filteredOrders.IsNullOrEmpty()) { throw new NoFoundEntityException("Ninguna orden cargada/disponible con el status PROCESSING."); }
                            break;
                        case "SHIPPED":
                            foreach (var order in orders)
                            {
                                if (order.status.ToString() == "SHIPPED") { filteredOrders.Add(order); }
                            }
                            if (filteredOrders.IsNullOrEmpty()) { throw new NoFoundEntityException("Ninguna orden cargada/disponible con el status SHIPPED."); }
                            break;
                        case "DELIVERED":
                            foreach (var order in orders)
                            {
                                if (order.status.ToString() == "DELIVERED") { filteredOrders.Add(order); }
                            }
                            if (filteredOrders.IsNullOrEmpty()) { throw new NoFoundEntityException("Ninguna orden cargada/disponible con el status DELIVERED."); }
                            break;
                        case "CANCELLED":
                            foreach (var order in orders)
                            {
                                if (order.status.ToString() == "CANCELLED") { filteredOrders.Add(order); }
                            }
                            if (filteredOrders.IsNullOrEmpty()) { throw new NoFoundEntityException("Ninguna orden cargada/disponible con el status CANCELLED."); }
                            break;
                        default:
                            throw new ArgumentException("Ingrese un status de orden válido (PENDING, PROCESSING, SHIPPED, DELIVERED, CANCELLED).");
                    }
                }
                if (!string.IsNullOrWhiteSpace(searchName))
                {
                    if (filteredOrders.IsNullOrEmpty())
                    {
                        foreach (var order in orders)
                        {
                            if (order.id.ToString().Contains(searchName) || order.customerId.ToString().Contains(searchName)) { filteredOrders.Add(order); }
                        }
                        if (filteredOrders.IsNullOrEmpty()) { throw new NoFoundEntityException($"Ninguno producto cargada/disponible que contenga en su nombre {searchName}."); }
                    }
                    else
                    {
                        var filteredOrdersByName = new List<Order>();
                        foreach (var order in filteredOrders)
                        {
                            if (order.id.ToString().Contains(searchName) || order.customerId.ToString().Contains(searchName)) { filteredOrdersByName.Add(order); }
                        }
                        if (filteredOrdersByName.IsNullOrEmpty()) { throw new NoFoundEntityException($"Ninguno producto cargada/disponible que contenga en su nombre {searchName}."); }
                        else { filteredOrders = filteredOrdersByName; filteredOrdersByName = null; }
                    }
                }
                if (filteredOrders.IsNullOrEmpty()) { filteredOrders = orders.ToList(); };
                filteredOrders.OrderByDescending(order => order.date);
                int totalPages = (int)Math.Ceiling(filteredOrders.Count() / (double)pageSize);
                if (filteredOrders.Count() > pageSize)
                {
                    if (filteredOrders.Count() > pageSize)
                    {
                        for (int i = 0; i < totalPages; i++)
                        {
                            if (i == (pageNumber - 1))
                            {
                                filteredOrders = filteredOrders.GetRange((i * pageSize), (pageSize));
                            }
                        }
                    }
                }

                var ordersClients = new List<OrderClient>();
                foreach (var order in filteredOrders)
                {
                    var orderItems = await _repository.GetAll<OrderItem>();
                    List<OrderItem> allItems = new List<OrderItem>();
                    foreach (var orderItem in orderItems)
                    {
                        if (order.id == orderItem.orderId) { allItems.Add(orderItem); }
                    }
                    order.orderItems = allItems;

                    var orderClient = new OrderClient(order.date, order.shippingAddress, order.billlingAddress, order.notes, order.totalAmount, order.orderItems, order.status,order.customerId, "No Client Name Found.", order.id);
                    var customers = await _repository.GetById<Customer>(order.customerId);
                    if (customers != null) { orderClient.customerName = customers.name; }
                    ordersClients.Add(orderClient);
                }
                return new PageModel<OrderClient>
                {
                    elementsPage = ordersClients,
                    pageNumber = pageNumber.ToString(),
                    pageSize = pageSize.ToString(),
                    totalPages = totalPages.ToString()
                };
            }
        }

        public async Task<Order?> getOrderById(Guid id)
        {
            var orderById = await _repository.GetById<Order>(id);
            if (orderById != null)
            {
                var orderItems = await _repository.GetAll<OrderItem>();
                List<OrderItem> allItems = new List<OrderItem>();
                foreach (var orderItem in orderItems)
                {
                    if (orderById.id == orderItem.orderId)
                    {
                        allItems.Add(orderItem);
                    }
                }
                orderById.orderItems = allItems; 
                return orderById;
            }
            else
            {
                throw new NoFoundEntityException($"Ninguna orden con ID {id} cargada/disponible.");
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
                throw new NoFoundEntityException("Orden a inhabilitar no cargada/disponible.");
            }
        }

        public async Task upgrateOrderStatus(Guid id, string? status)
        {
            var orderById = await _repository.GetById<Order>(id);

            if (orderById != null)
            {
                string newStatus = status.ToUpper();
                if(newStatus == OrderStatus.CANCELLED.ToString()) 
                {
                    orderById.status = OrderStatus.CANCELLED;
                }
                    else if(newStatus == OrderStatus.DELIVERED.ToString())
                    {
                        orderById.status = OrderStatus.DELIVERED;
                    }
                    else if (newStatus == OrderStatus.PENDING.ToString())
                    {
                        orderById.status = OrderStatus.PENDING;
                    }
                    else if (newStatus == OrderStatus.PROCESSING.ToString())
                    {
                        orderById.status = OrderStatus.PROCESSING;
                    }
                    else if (newStatus == OrderStatus.SHIPPED.ToString())
                    {
                        orderById.status = OrderStatus.SHIPPED;
                    } else { throw new ArgumentException("Status ingresado no valido para una orden."); }

                await _repository.Update<Order>(orderById);
            }
            else
            {
                throw new NoFoundEntityException("Orden a actualizar no cargada/disponible.");
            }
        }

        public async Task<IEnumerable<Order>?> getOrdersByCustomersId(Guid id)
        {
            var orders = await _repository.GetAll<Order>();

            if (orders != null || !orders.Any())
            {
                foreach (var order in orders)
                {
                    var orderItems = await _repository.GetAll<OrderItem>();
                    List<OrderItem> allItems = new List<OrderItem>();
                    foreach (var orderItem in orderItems)
                    {
                        if (order.id == orderItem.orderId)
                        {
                            allItems.Add(orderItem);
                        }
                    }
                    order.orderItems = allItems;
                }

                List<Order> customersOrders = new List<Order>();
                foreach (var order in orders)
                {
                    if(order.customerId == id) 
                    {
                        customersOrders.Add(order);
                    }
                }

                if(!customersOrders.IsNullOrEmpty()) { return customersOrders; }
                else { throw new NoFoundEntityException("Ninguna orden con el id del cliente cargada/disponible."); }
            }
            else
            {
                throw new NoFoundEntityException("Ninguna orden cargada/disponible.");
            }
        }
    }
}
