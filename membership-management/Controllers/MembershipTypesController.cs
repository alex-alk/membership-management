using AutoMapper;
using membership_management.Contracts;
using membership_management.Data;
using membership_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class MembershipTypesController : Controller
    {
        private readonly IMembershipTypeRepository _repo;
        private readonly IMapper _mapper;

        public MembershipTypesController(IMembershipTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET: MembershipTypes
        public ActionResult Index()
        {
            var membershipTypes = _repo.FindAll().ToList();
            var model = _mapper.Map<List<MembershipType>, List<MembershipTypeVM>>(membershipTypes); // converted to VM
            return View(model);
        }

        // GET: MembershipTypes/Details/5
        public ActionResult Details(int id)
        {
            if(!_repo.isExists(id))
            {
                return NotFound();
            }
            var membershipType = _repo.FindById(id);
            var model = _mapper.Map<MembershipTypeVM>(membershipType);
            return View(model);
        }

        // GET: MembershipTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MembershipTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MembershipTypeVM model)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return View(model);
                }
                var membershipType = _mapper.Map<MembershipType>(model);
                membershipType.DateCreated = DateTime.Now;
                var isSuccess = _repo.Create(membershipType);
                if(!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }
        }

        // GET: MembershipTypes/Edit/5
        public ActionResult Edit(int id)
        {
            if(!_repo.isExists(id))
            {
                return NotFound();
            }
            var membershipType = _repo.FindById(id);
            var model = _mapper.Map<MembershipTypeVM>(membershipType);
            return View(model);
        }

        // POST: MembershipTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MembershipTypeVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var membershipType = _mapper.Map<MembershipType>(model);
                var isSuccess = _repo.Update(membershipType);
                if(!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong...");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong...");
                return View();
            }
        }

        // GET: MembershipTypes/Delete/5
        public ActionResult Delete(int id)
        {
            var membershipType = _repo.FindById(id);
            if (membershipType == null)
            {
                return NotFound();
            }
            var isSuccess = _repo.Delete(membershipType);
            if (!isSuccess)
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: MembershipTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, MembershipTypeVM model)
        {
            try
            {
                var membershipType = _repo.FindById(id);
                if(membershipType == null)
                {
                    return NotFound();
                }
                var isSuccess = _repo.Delete(membershipType);
                if (!isSuccess)
                {
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}
