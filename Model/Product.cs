using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NWConsole.Model
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        public int? SupplierId { get; set; }
        public int? CategoryId { get; set; }        
        public string QuantityPerUnit { get; set; }
        [Range(0.00, 10000.00,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public decimal? UnitPrice { get; set; }
        [Range(0, 10000,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public short? UnitsInStock { get; set; }
        [Range(0, 10000,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public short? UnitsOnOrder { get; set; }
        [Range(0, 10000,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public virtual Category Category { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public override string ToString()
        {
            return $"Product ID: {ProductId}\n" +
               $"Product Name: {ProductName}\n" +
               $"Supplier ID: {SupplierId}\n" +
               $"Category ID: {CategoryId}\n" +
               $"Quantity Per Unit: {QuantityPerUnit}\n" +
               $"Unit Price: {UnitPrice}\n" +
               $"Units In Stock: {UnitsInStock}\n" +
               $"Units On Order: {UnitsOnOrder}\n" +
               $"Reorder Level: {ReorderLevel}\n" +
               $"Discontinued: {Discontinued}\n";
        }
    }
}
