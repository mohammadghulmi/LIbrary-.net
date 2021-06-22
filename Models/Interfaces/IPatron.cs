using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Interfaces
{
    public interface IPatron
    {
        Patron Get(int Id);
        IEnumerable<Patron> GetAll();
        void Add(Patron patron);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int Id);
        IEnumerable<Hold> GetHolds(int Id);
        IEnumerable<Checkout> GetCheckouts(int Id);

    }
}
