using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        public Guid orderId { get; set; }
        public string skuProduct {  get; set; }
        public int quantity {  get; set; }
        public decimal unitPrice { get; set; }
        public decimal subTotal { get; set; }
        public OrderItem(int Quantity, decimal UnitPrice, Guid orderId, string skuProduct) : base()
        {
            this.quantity = Quantity;
            this.unitPrice = UnitPrice;
            subTotal = (Quantity * UnitPrice);
            this.orderId = orderId;
            this.skuProduct = skuProduct;
        }
        public OrderItem(int Quantity, decimal UnitPrice, string skuProduct) : base()
        {
            this.quantity = Quantity;
            this.unitPrice = UnitPrice;
            subTotal = (Quantity * UnitPrice);
            this.skuProduct = skuProduct;
        }

        public OrderItem() { }
    }
}
