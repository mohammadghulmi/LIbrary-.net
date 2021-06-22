using Library.Models;
using Library.Models.Catalog;
using Library.Models.CheckoutModels;
using Library.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAsset _Asset;
        private ICheckOut _Checkout;
        public CatalogController(ILibraryAsset asset,ICheckOut checkOut)
        {
            _Asset = asset;
            _Checkout = checkOut;
        }
        public IActionResult Index()
        {
            var AssetModels = _Asset.GetAll();
            var ListingResults = AssetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id = result.ID,
                    ImageUrl = result.ImageUrl,
                    AuthorOrDIrector=_Asset.GetAuthorOrDirector(result.ID),
                    DeweyCallNumber=_Asset.GetDeweyIndex(result.ID),
                    Title=result.Title,
                    Type=_Asset.GetType(result.ID)

                }) ;
            var Model = new AssetIndexModel()
            {
                Assets = ListingResults
            };
            return View(Model);
        }
        public IActionResult Detail(int id)
        {
            var Asset = _Asset.GetById(id);
            var cuurentHolds = _Checkout.GetCurrentHolds(Asset.ID).Select(a => new AssetHoldModel
            {
                HoldPlaced = _Checkout.GetCurrentHoldPlaced(Asset.ID).ToString("d"),
                PatronName = _Checkout.GetCurrentHoldPatronName(Asset.ID)
            }
            );

            var Model = new AssetDetailModel()
            {
                AssetId = id,
                Title = Asset.Title,
                Year = Asset.Year,
                Cost = Asset.Cost,
                Status = Asset.Status.Name,
                ImageUrl = Asset.ImageUrl,
                AuthorOrDirector = _Asset.GetAuthorOrDirector(id),
                CurrentLocation = _Asset.GetLocation(id).Name,
                DeweyCallNumber = _Asset.GetDeweyIndex(id),
                ISBN = _Asset.GetISBN(id),
                CurrentHolds = cuurentHolds,
                
            };
            return View(Model);
        }
        public IActionResult Checkout(int id)
        {
            var asset = _Asset.GetById(id);
            var model = new CheckoutModels
            {   
                AssetId=id,
                ImageUrl=asset.ImageUrl,
                Title=asset.Title,
                LibraryCardId="",
                IsCheckedOut=_Checkout.IsCheckedOut(id)
            };
            return View(model); 
        }
        public IActionResult CheckIn(int Id)
        {
            var asset = _Asset.GetById(Id);
            var model = new CheckoutModels
            {
                AssetId = Id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _Checkout.IsCheckedOut(Id)
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult PlaceCheckout(int AssetId,int LibraryCardId)
        {
            _Checkout.CheckOutItem(AssetId, LibraryCardId);
            return RedirectToAction("Detail",new {id = AssetId});
        }
        [HttpPost]
        public IActionResult PlaceHold(int AssetId, int LibraryCardId)
        {
            _Checkout.PlaceHold(AssetId, LibraryCardId);
            return RedirectToAction("Detail", new { id = AssetId });
        }
        [HttpPost]
        public IActionResult PlaceCheckIn(int AssetId, int LibraryCardId)
        {
            _Checkout.CheckInItem(AssetId, LibraryCardId);
            return RedirectToAction("Detail", new { id = AssetId });
        }
        
        public IActionResult MarkLost(int id)
        {
            _Checkout.MarkLost(id);
            return RedirectToAction("Detail", new { id });
        }

        public IActionResult MarkFound(int id)
        {
            _Checkout.MarkFound(id);
            return RedirectToAction("Detail", new { id });
        }
        public IActionResult Hold(int id)
        {
            var asset = _Asset.GetById(id);

            var model = new CheckoutModels
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                HoldCount = _Checkout.GetCurrentHolds(id).Count()
            };
            return View(model);
        }

    }
}
