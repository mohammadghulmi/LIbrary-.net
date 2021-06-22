using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Interfaces
{
    public interface ICheckOut
    {
        IEnumerable<Checkout> GetAll();
        Checkout GetById(int CheckOutId);
        Checkout GetLatestCheckout(int Id);
        void Add(Checkout Checkout);
        void CheckOutItem(int AssetID, int LibraryCardId);
        void CheckInItem(int AssetID, int LibraryCardId);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int Id);
        void PlaceHold(int AssetID, int LibraryCardId);
        string GetCurrentHoldPatronName(int Id);
        DateTime GetCurrentHoldPlaced(int Id);
        IEnumerable<Hold> GetCurrentHolds(int Id);
        bool IsCheckedOut(int Id);
        void MarkLost(int AssetID);
        void MarkFound(int AssetID);

    }
}
