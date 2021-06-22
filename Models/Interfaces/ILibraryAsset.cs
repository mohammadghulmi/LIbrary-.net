using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Interfaces
{
    public interface ILibraryAsset
    {
        IEnumerable<LibraryAsset> GetAll();
        LibraryAsset GetById(int Id);
        void Add(LibraryAsset newAsset);
        string GetAuthorOrDirector(int Id);
        string GetDeweyIndex(int id);
        string GetType(int Id);
        string GetTitle(int Id);
        string GetISBN(int Id);
        LibraryBranch GetLocation(int Id);
    }
}
