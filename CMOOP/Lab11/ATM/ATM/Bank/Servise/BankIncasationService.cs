using ATM.ATM.Interface;
using ATM.ATM;
using ATM.ATM.Servise;

namespace ATM.Servise
{
    public class BankIncasationService : IAtmIncasator
    {
        public IAtm[] InitializeNetwork(int count, AtmCassette[] initialCash)
        {
            if (count <= 0) throw new ArgumentException("Кількість банкоматів має бути > 0.");

            IAtm[] network = new IAtm[count];
            for (int i = 0; i < count; i++)
            {
                AtmDevice atm = new AtmDevice(i + 1);
                foreach (AtmCassette cashItem in initialCash)
                {
                    atm.AddBanknotes(cashItem.Denomination, cashItem.Count);
                }
                network[i] = atm;
            }
            return network;
        }
    }
}
