using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomer3
{
    public struct Item
    {
        public string Name { get; }
        public ItemCategory Category { get; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Item(string name, ItemCategory category, int quantity, decimal price)
        {
            Name = name;
            Category = category;
            Quantity = quantity;
            Price = price;
        }

        public override string ToString()
        {
            return $"[{Category}] {Name} | К-сть: {Quantity} | Ціна: {Price} gold";
        }
    }
}
