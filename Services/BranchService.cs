using Library.Data;
using Library.Models;
using Library.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Services
{
    public class BranchService : ILibraryBranch
    {
        private LibraryContext _Context;
        public BranchService(LibraryContext context)
        {
            _Context = context;
        }
        

        public LibraryBranch Get(int id)
        {
            return GetAll().FirstOrDefault(LibraryBranch => LibraryBranch.Id == id);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _Context.LibraryBranches.
                Include(LibraryBranch => LibraryBranch.LibraryAssets).
                Include(LibraryBranch => LibraryBranch.Patrons);
        }

        public IEnumerable<LibraryAsset> GetAssets(int Id)
        {
            return Get(Id).LibraryAssets;
        }

        public IEnumerable<string> GetBranchHours(int id)
        {
            var Hours = _Context.BranchHours.Where(BranchHours => BranchHours.Branch.Id == id);
            return DataHelpers.HumanizeHours(Hours);
        }

        public IEnumerable<Patron> GetPatrons(int Id)
        {
            return Get(Id).Patrons;
        }

        public bool IsBranchOpen(int Id)
        {
            var now = DateTime.Now.Hour;
            var Day = (int)DateTime.Now.DayOfWeek +1;
            var Hours = _Context.BranchHours.Where(BranchHours => BranchHours.Branch.Id == Id);
            var DaysHours = Hours.FirstOrDefault(h => h.DayOfWeek == Day);
            bool IsOpen = now < DaysHours.CloseTime && now > DaysHours.OpenTime;
            return IsOpen;


        }

        void ILibraryBranch.Add(LibraryBranch branch)
        {
            _Context.Add(branch);
            _Context.SaveChanges();
        }
    }
}
