using Library.Models.Interfaces;
using Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public class CheckoutService : ICheckOut
    {
        private LibraryContext _Context;
        public CheckoutService(LibraryContext Context)
        {
            _Context = Context;
        }
       
        public void CheckInItem(int AssetID, int LibraryCardId)
        {
            var now = DateTime.Now;
            var item = _Context.LibraryAssets.FirstOrDefault(l => l.ID == AssetID);
            RemoveCheckouts(item.ID);
            RemoveHistory(item.ID);
            var Holds = _Context.Holds.Include(h => h.LibraryAsset).Include(h => h.LibraryCard).Where(h => h.LibraryAsset.ID == AssetID);
            if (Holds.Any())
            {
                CheckoutToEarliestHold(AssetID,Holds);
            }
            var Item = _Context.LibraryAssets.FirstOrDefault(LibraryAsset => LibraryAsset.ID == AssetID);
            _Context.Update(Item);
            Item.Status = _Context.Statuses.FirstOrDefault(s => s.Name == "Available");
            _Context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int assetID, IQueryable<Hold> holds)
        {
            var MostRecent = holds.OrderBy(holds => holds.HoldPlaced).FirstOrDefault();
            var LibraryCard = MostRecent.LibraryCard;
            _Context.Remove(MostRecent);
            _Context.SaveChanges();
            CheckOutItem(assetID, LibraryCard.Id);
        }

        public void CheckOutItem(int AssetID, int LibraryCardId)
        {
            if (IsCheckedOut(AssetID))
            {
                return;
            }
            var Item = _Context.LibraryAssets.FirstOrDefault(LibraryAsset => LibraryAsset.ID == AssetID);
            _Context.Update(Item);
            Item.Status = _Context.Statuses.FirstOrDefault(s => s.Name == "Checked Out");
            var LibraryCard = _Context.LibraryCards.Include(Card => Card.Checkouts).FirstOrDefault(Card => Card.Id == LibraryCardId);
            var Checkout = new Checkout
            {
                LibraryAsset = Item,
                LibraryCard = LibraryCard,
                Since = DateTime.Now,
                Until = GetDefaultCheckOutTime(DateTime.Now)
            };
            _Context.Add(Checkout);
            var CheckoutHistory = new CheckoutHistory
            {
                CheckedOut = DateTime.Now,
                LibraryAsset=Item,
                LibraryCard = LibraryCard,

            };
            _Context.Add(CheckoutHistory);
            _Context.SaveChanges();
        }

        private DateTime GetDefaultCheckOutTime(DateTime now)
        {
            return now.AddDays(14);
        }

        public bool IsCheckedOut(int assetID)
        {
            var IsCheckedOut = _Context.Checkouts.Where(co => co.LibraryAsset.ID == assetID).Any();
            return IsCheckedOut;    
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _Context.Checkouts;
        }

        public Checkout GetById(int CheckOutId)
        {
            return GetAll().FirstOrDefault(checkout => checkout.Id == CheckOutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int Id)
        {
            return _Context.CheckoutHistories.Include(h => h.LibraryAsset).Include(h => h.LibraryCard).Where(h=>h.LibraryAsset.ID==Id);
        }

        public string GetCurrentHoldPatronName(int Id)
        {
            var hold = _Context.Holds.Include(h => h.LibraryAsset).Include(h=> h.LibraryCard).FirstOrDefault(h => h.Id == Id);
            var CardId = hold.LibraryCard.Id;
            var Patron = _Context.Patrons.Include(p => p.LibraryCard).FirstOrDefault(p => p.LibraryCard.Id == CardId);
            return Patron.FirstName + " " + Patron.LastName;
        }
        public string GetCurrentCheckoutPatronName(int Id)
        {
            var Checkout = _Context.Checkouts.Include(co => co.LibraryAsset).Include(co => co.LibraryCard).FirstOrDefault(co => co.LibraryAsset.ID == Id);
            var Patron = _Context.Patrons.Include(p => p.LibraryCard).FirstOrDefault(p => p.LibraryCard.Id == Checkout.LibraryCard.Id);
            return Patron.FirstName +" " +Patron.LastName;
        }
        public DateTime GetCurrentHoldPlaced(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Hold> GetCurrentHolds(int Id)
        {
            return _Context.Holds.Include(h => h.LibraryAsset).Where(h => h.LibraryAsset.ID == Id);
        }

        public Checkout GetLatestCheckout(int Id)
        {
            return _Context.Checkouts.Where(c => c.LibraryAsset.ID == Id).OrderByDescending(c => c.Since).FirstOrDefault();
        }
        public void RemoveCheckouts(int AssetID)
        {
            var Checkout = _Context.Checkouts.FirstOrDefault(co => co.LibraryAsset.ID == AssetID);
            if (Checkout != null)
            {
                _Context.Remove(Checkout);
            }
        }
        public void RemoveHistory(int Id)
        {
            var History = _Context.CheckoutHistories.FirstOrDefault(h => h.LibraryAsset.ID == Id && h.CheckedIn == null);
            if (History != null)
            {
                _Context.Update(History);
                History.CheckedIn = DateTime.Now;
            }
        }
        public void MarkFound(int AssetID)
        {
            var Item = _Context.LibraryAssets.FirstOrDefault(LibraryAsset => LibraryAsset.ID == AssetID);
            _Context.Update(Item);
            Item.Status = _Context.Statuses.FirstOrDefault(s => s.Name == "Available");
            RemoveCheckouts(Item.ID);
            var History = _Context.CheckoutHistories.FirstOrDefault(h => h.LibraryAsset.ID == Item.ID && h.CheckedIn == null);
            if (History != null)
            {
                _Context.Update(History);
                History.CheckedIn = DateTime.Now;
            }
            _Context.SaveChanges();
        }

        public void MarkLost(int AssetID)
        {
            var Item = _Context.LibraryAssets.FirstOrDefault(LibraryAsset => LibraryAsset.ID == AssetID);
            _Context.Update(Item);
            Item.Status = _Context.Statuses.FirstOrDefault(s => s.Name == "Lost");
            _Context.SaveChanges();
        }

        public void PlaceHold(int AssetID, int LibraryCardId)
        {
            var now = DateTime.Now;
            var Asset = _Context.LibraryAssets.FirstOrDefault(Asset => Asset.ID == AssetID);
            var Card = _Context.LibraryCards.FirstOrDefault(Card => Card.Id == LibraryCardId);
            if (Asset.Status.Name == "Available")
            {
                Asset.Status = _Context.Statuses.FirstOrDefault(s => s.Name == "On Hold");
            }
            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = Asset,
                LibraryCard = Card,
            };
            _Context.Add(hold);
            _Context.SaveChanges();
        }

       
        public void Add(Checkout Checkout)
        {
            _Context.Add(Checkout);
            _Context.SaveChanges();
        }
    }
}
