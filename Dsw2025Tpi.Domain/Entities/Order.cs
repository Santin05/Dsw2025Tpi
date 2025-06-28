using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        public DateTime date { get; set; }
        public string? shippingAddress { get; set; }
        public string? billlingAddress { get; set; }
        public string? notes { get; set; }
        public decimal totalAmount { get; set; }
        public IEnumerable<OrderItem> orderItems { get; set; }
        public OrderStatus status { get; set; }
        public Guid customerId { get; set; }
        public Order (DateTime Date, string AddressShip, string AddressBill, string Notes, decimal Total, IEnumerable<OrderItem> Items, Guid customerId) 
        {
            date = Date;
            shippingAddress = AddressShip;
            billlingAddress = AddressBill;
            notes = Notes;
            totalAmount = Total;
            orderItems = Items;
            status = 0;
            this.customerId = customerId;
        }

        public Order() { }
    }
}