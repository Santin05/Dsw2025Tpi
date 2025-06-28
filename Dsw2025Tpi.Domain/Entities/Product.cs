using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Product : EntityBase
    {
        public string sku {  get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string internalCode { get; set; }
        public decimal currentUnitPrice { get; set; } 
        public int stockQuantity { get; set; }
        public bool isActive { get; set; }

        public Product(string sku, string name, string description, string internalCode, decimal currentUnitPrice, int stockQuantity) : base()
        {
            this.sku = sku;
            this.name = name;
            this.description = description;
            this.internalCode = internalCode;
            this.currentUnitPrice = currentUnitPrice;
            this.stockQuantity = stockQuantity;
            this.isActive = true;
        }

        public Product(string sku, string name, string description, string internalCode, decimal currentUnitPrice, int strockQuantity, Guid id) : base(id)
        {
            this.sku = sku;
            this.name = name;
            this.description = description;
            this.internalCode = internalCode;
            this.currentUnitPrice = currentUnitPrice;
            this.stockQuantity = stockQuantity;
            this.isActive = true;
        }
        public Product() { }
    }
}
