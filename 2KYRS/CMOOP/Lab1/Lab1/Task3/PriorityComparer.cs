using System;
using System.Collections.Generic;
using System.Text;

namespace Lab1
{
    public class PriorityComparer : IComparer<(Priority Priority, DateTime CreatedAt)>
    {
        public int Compare((Priority Priority, DateTime CreatedAt) x, (Priority Priority, DateTime CreatedAt) y)
        {
            int priorityComparison = y.Priority.CompareTo(x.Priority);
            if (priorityComparison != 0)
            {
                return priorityComparison;
            }
            return x.CreatedAt.CompareTo(y.CreatedAt);
        }
    }
}
