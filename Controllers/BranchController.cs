using Library.Models.Branch;
using Library.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class BranchController : Controller
    {
        private readonly ILibraryBranch _branchService;

        public BranchController(ILibraryBranch branchService)
        {
            _branchService = branchService;
        }

        public IActionResult Index()
        {
            var branchModels = _branchService.GetAll()
                .Select(br => new BranchDetailModel
                {
                    Id = br.Id,
                    BranchName = br.Name,
                    NumberOfAssets = _branchService.GetAssets(br.Id).Count(),
                    NumberOfPatrons = _branchService.GetPatrons(br.Id).Count(),
                    IsOpen = _branchService.IsBranchOpen(br.Id)
                }).ToList();

            var model = new BranchIndexModel
            {
                Branches = branchModels
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var branch = _branchService.Get(id);
            var model = new BranchDetailModel
            {
                BranchName = branch.Name,
                Description = branch.Description,
                Address = branch.Address,
                Telephone = branch.Telephone,
                BranchOpenedDate = branch.OpenDate.ToString("yyyy-MM-dd"),
                NumberOfPatrons = _branchService.GetPatrons(id).Count()                            ,
                NumberOfAssets = _branchService.GetAssets(id).Count(),
                
                ImageUrl = branch.ImageUrl,
                HoursOpen = _branchService.GetBranchHours(id)
            };

            return View(model);
        }
    }
}
