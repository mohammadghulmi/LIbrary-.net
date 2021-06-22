using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Interfaces
{
    public interface ILibraryBranch
    {
        IEnumerable<LibraryBranch> GetAll();
        IEnumerable<Patron> GetPatrons(int Id);
        IEnumerable<LibraryAsset> GetAssets(int Id);
        IEnumerable<string> GetBranchHours(int id);
        LibraryBranch Get(int id);
        void Add(LibraryBranch branch);
        bool IsBranchOpen(int Id);
    }
}
