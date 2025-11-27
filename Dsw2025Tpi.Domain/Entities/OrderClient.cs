using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class OrderClient : EntityBase
    {
        public DateTime date { get; set; }
        public string? shippingAddress { get; set; }
        public string? billlingAddress { get; set; }
        public string? notes { get; set; }
        public decimal totalAmount { get; set; }
        public IEnumerable<OrderItem> orderItems { get; set; }
        public OrderStatus status { get; set; }
        public Guid customerId { get; set; }

        public string? customerName { get; set; }
        public OrderClient(DateTime Date, string AddressShip, string AddressBill, string Notes, decimal Total, IEnumerable<OrderItem> Items, OrderStatus status, Guid customerId, string? customerName, Guid id)
        {
            date = Date;
            shippingAddress = AddressShip;
            billlingAddress = AddressBill;
            notes = Notes;
            totalAmount = Total;
            orderItems = Items;
            this.status = status;
            this.customerId = customerId;
            this.customerName = customerName;
            this.id = id;
        }

        public OrderClient() { }
    }
}
