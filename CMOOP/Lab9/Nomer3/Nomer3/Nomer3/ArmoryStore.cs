using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomer3
{
    public class ArmoryStore
    {
        private Item[] _items;
        private int _count;

        public ArmoryStore(int capacity)
        {
            _items = new Item[capacity];
            _count = 0;
        }
        public void AddItem(Item newItem)
        {
            if (_count >= _items.Length)
            {
                Console.WriteLine("Магазин заповнений! Неможливо додати товар.");
                return;
            }

            _items[_count] = newItem;
            _count++;
            Console.WriteLine($"Товар '{newItem.Name}' додано до асортименту.");
        }
        public void RemoveItem(string itemName)
        {
            int indexToRemove = -1;

            for (int i = 0; i < _count; i++)
            {
                if (_items[i].Name == itemName)
                {
                    indexToRemove = i;
                    break;
                }
            }

            if (indexToRemove != -1)
            {
                for (int i = indexToRemove; i < _count - 1; i++)
                {
                    _items[i] = _items[i + 1];
                }
                _count--;
                _items[_count] = default; 
                Console.WriteLine($"Товар '{itemName}' видалено.");
            }
        }

        public void UpdateStock(string itemName, int newQuantity, decimal newPrice)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_items[i].Name == itemName)
                {
                    _items[i].Quantity = newQuantity;
                    _items[i].Price = newPrice;
                    Console.WriteLine($"Товар '{itemName}' оновлено.");
                    return;
                }
            }
            Console.WriteLine("Товар не знайдено.");
        }

        public void ShowInventory()
        {
            Console.WriteLine("\n--- Поточний асортимент Armory ---");
            for (int i = 0; i < _count; i++)
            {
                Console.WriteLine(_items[i].ToString());
            }
            Console.WriteLine("----------------------------------\n");
        }
    }
}
