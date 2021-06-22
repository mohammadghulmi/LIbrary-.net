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
    public class LibraryAssetService : ILibraryAsset
    {
        private LibraryContext _context;
        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);

            _context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location);
        }

        public string GetAuthorOrDirector(int Id)
        {
            var IsBook = _context.LibraryAssets.OfType<Book>().Where(asset => asset.ID == Id).Any();
            var IsVideo = _context.LibraryAssets.OfType<Video>().Where(asset => asset.ID == Id).Any();
            if (IsBook)
            {
                return _context.Books.FirstOrDefault(Book => Book.ID == Id).Author;
            }
            else if (IsVideo)
            {
                return _context.Videos.FirstOrDefault(Video => Video.ID == Id).Director;
            }
            else return "";
        }

        public LibraryAsset GetById(int Id)
        {
            return _context.LibraryAssets
            .Include(asset => asset.Status)
            .Include(asset => asset.Location)
            .FirstOrDefault(asset => asset.ID == Id);
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(Book => Book.ID == id))
            {
                return _context.Books.FirstOrDefault(book => book.ID == id).DeweyIndex;
            }
            else return "";
        }

        public string GetISBN(int Id)
        {
            if (_context.Books.Any(Book => Book.ID == Id))
            {
                return _context.Books.FirstOrDefault(book => book.ID == Id).ISBN;
            }
            else return "";
        }

        public LibraryBranch GetLocation(int Id)
        {
            return GetById(Id).Location;
        }

        public string GetTitle(int Id)
        {
            return GetById(Id).Title;   
        }

        public string GetType(int Id)
        {
            var IsBook = _context.LibraryAssets.OfType<Book>().Where(asset => asset.ID == Id).Any();
            var IsVideo = _context.LibraryAssets.OfType<Video>().Where(asset => asset.ID == Id).Any();
            if (IsBook)
            {
                return "Book";
            }
            else if (IsVideo)
            {
                return "Video";
            }
            else return "";
        }
    }
}
