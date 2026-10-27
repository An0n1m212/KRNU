using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomer2
{
    public class Suitcase
    {
        public string Color { get; set; }
        public string Producer { get; set; }
        public double Weight { get; set; }
        public double MaxVolume { get; set; }

        private List<Item> _content = new List<Item>();
        private double _currentVolume = 0;

        public event EventHandler<ItemAddedEventArgs> ItemAdded;
        public event EventHandler<ItemRemovedEventArgs> ItemRemoved;

        public void AddItem(Item item)
        {
            if (_currentVolume + item.Volume > MaxVolume)
            {
                throw new InvalidOperationException($"Перевищено об'єм валізи! Залишилось: {MaxVolume - _currentVolume}, треба: {item.Volume}");
            }

            _content.Add(item);
            _currentVolume += item.Volume;

            ItemAdded?.Invoke(this, new ItemAddedEventArgs(item));
        }
        public void RemoveItem(Item item)
        {
            if (_content.Contains(item))
            {
                _content.Remove(item);
                _currentVolume -= item.Volume;
            }
            ItemRemoved?.Invoke(this, new ItemRemovedEventArgs(item));
        }
    }
}
