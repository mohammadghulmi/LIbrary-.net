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
    public class PatronService : IPatron
    {
        private LibraryContext _Context;
        public PatronService(LibraryContext Context) 
        {
            _Context = Context;
        }

        public void Add(Patron patron)
        {
            _Context.Add(patron);
            _Context.SaveChanges();
        }

        public Patron Get(int Id)
        {
            return GetAll().FirstOrDefault(Patron => Patron.Id == Id);

        }

        public IEnumerable<Patron> GetAll()
        {
            return _Context.Patrons.Include(Patron => Patron.LibraryCard).Include(Patron => Patron.HomeLibraryBranch);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int Id)
        {
            var CardId = Get(Id).LibraryCard.Id;
            return _Context.CheckoutHistories.
                Include(CheckoutHistory => CheckoutHistory.LibraryCard).
                Include(CheckoutHistory => CheckoutHistory.LibraryAsset).
                Where(CheckoutHistory => CheckoutHistory.LibraryCard.Id == CardId).
                OrderByDescending(CheckoutHistory => CheckoutHistory.CheckedOut);
        }

        public IEnumerable<Checkout> GetCheckouts(int Id)
        {
            var CardId = Get(Id).LibraryCard.Id;
            return _Context.Checkouts.
                Include(Checkout => Checkout.LibraryCard).
                Include(Checkout => Checkout.LibraryAsset).
                Where(Checkout => Checkout.LibraryCard.Id == CardId);
        }

        public IEnumerable<Hold> GetHolds(int Id)
        {
            var CardId = Get(Id).LibraryCard.Id;
            return _Context.Holds.
                Include(Hold => Hold.LibraryCard).
                Include(Hold => Hold.LibraryAsset).
                Where(Hold => Hold.LibraryCard.Id == Id).
                OrderByDescending(Hold => Hold.HoldPlaced);
        }
    }
}
