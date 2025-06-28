using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Models
{
    public record OrderModel
    {
        public record Item(Guid productId, int quantity, string name, string description, decimal currentUnitPrice);
        public record Request(Guid customerId, string ShipppingAddress, string BillingAddress, string Notes, IEnumerable<Item> ItemsOrder);
        public record Response(Guid Id);
    }
}