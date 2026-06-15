using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nomer2
{

    public class ItemAddedEventArgs : EventArgs
    {
        public Item AddedItem { get; }
        public ItemAddedEventArgs(Item item) => AddedItem = item;
    }    
    public class ItemRemovedEventArgs : EventArgs
    {
        public Item RemovedItem { get; set; }
        public ItemRemovedEventArgs(Item item) => RemovedItem = item;
    }

}
