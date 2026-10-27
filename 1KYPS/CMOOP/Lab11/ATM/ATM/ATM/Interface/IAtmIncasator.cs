using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM.ATM.Interface
{
    public interface IAtmIncasator
    {
        IAtm[] InitializeNetwork(int count, AtmCassette[] initialCash);
    }
}
