using AutoMapper;
using membership_management.Contracts;
using membership_management.Data;
using membership_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class MembershipAllocationController : Controller
    {
        private readonly IMembershipTypeRepository _membershiprepo;
        private readonly IMembershipAllocationRepository _membershipallocationrepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public MembershipAllocationController(
            IMembershipTypeRepository membershiprepo,
            IMembershipAllocationRepository membershipallocationrepo,
            IMapper mapper,
            UserManager<Employee> userManager
            )
        {
            _membershiprepo = membershiprepo;
            _membershipallocationrepo = membershipallocationrepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: MembershipAllocation
        public ActionResult Index()
        {
            var membershipTypes = _membershiprepo.FindAll().ToList();
            var mappedMembershipTypes = _mapper.Map<List<MembershipType>, List<MembershipTypeVM>>(membershipTypes); // converted to VM
            var model = new CreateMembershipAllocationVM
            {
                MembershipTypes = mappedMembershipTypes,
                NumberUpdated = 0
            };
            return View(model);
        }

        public ActionResult SetMembership(int id)
        {
            var membershipType = _membershiprepo.FindById(id);
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            foreach (var emp in employees)
            {
                if(_membershipallocationrepo.CheckAllocation(id, emp.Id))
                {
                    continue;
                }
                var allocation = new MembershipAllocationVM
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    MembershipTypeId = id,
                    NumberOfDays = membershipType.DefaultDays,
                    Period = DateTime.Now.Year
                };
                var membershipallocation = _mapper.Map<MembershipAllocation>(allocation);
                _membershipallocationrepo.Create(membershipallocation);
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult ListEmployees()
        {
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var model = _mapper.Map<List<EmployeeVM>>(employees);
            return View(model);
        }

        // GET: MembershipAllocation/Details/5
        public ActionResult Details(string id)
        {
            var employee = _mapper.Map<EmployeeVM>(_userManager.FindByIdAsync(id).Result);
            var allocations = _mapper.Map <List<MembershipAllocationVM>>(_membershipallocationrepo.GetMembershipAllocationsByEmployee(id));
            var model = new ViewAllocationsVM
            {
                Employee = employee,
                MembershipAllocations = allocations
            };
            return View(model);
        }

        // GET: MembershipAllocation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MembershipAllocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MembershipAllocation/Edit/5
        public ActionResult Edit(int id)
        {
            var membershipAllocation = _membershipallocationrepo.FindById(id);
            var model = _mapper.Map<EditMembershipAllocationVM>(membershipAllocation);
            return View(model);
        }

        // POST: MembershipAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditMembershipAllocationVM model)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return View(model);
                }

                var record = _membershipallocationrepo.FindById(model.Id);
                record.NumberOfDays = model.NumberOfDays;

                var isSuccess = _membershipallocationrepo.Update(record);
                if(!isSuccess)
                {
                    ModelState.AddModelError("", "Error while saving");
                    return View(model);
                }


                return RedirectToAction(nameof(Details), new {id = model.EmployeeId});
            }
            catch
            {
                return View(model);
            }
        }

        // GET: MembershipAllocation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MembershipAllocation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
