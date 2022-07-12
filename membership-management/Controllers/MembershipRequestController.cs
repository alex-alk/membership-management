using AutoMapper;
using membership_management.Contracts;
using membership_management.Data;
using membership_management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Controllers
{
    [Authorize]
    public class MembershipRequestController : Controller
    {

        private readonly IMembershipRequestRepository _membershipRequestRepo;
        private readonly IMembershipTypeRepository _membershipTypeRepo;
        private readonly IMembershipAllocationRepository _membershipAllocRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public MembershipRequestController(
            IMembershipRequestRepository membershipRequestRepo,
            IMembershipTypeRepository membershipTypeRepo,
            IMembershipAllocationRepository membershipAllocRepo,
            IMapper mapper,
            UserManager<Employee> userManager
        )
        {
            _membershipRequestRepo = membershipRequestRepo;
            _membershipTypeRepo = membershipTypeRepo;
            _membershipAllocRepo = membershipAllocRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrator")]

        // GET: MembershipRequest
        public ActionResult Index()
        {
            var membershipRequests = _membershipRequestRepo.FindAll();
            var membershipRequestsModel = _mapper.Map<List<MembershipRequestVM>>(membershipRequests);
            var model = new AdminMembershipRequestViewVM
            {
                TotalRequests = membershipRequestsModel.Count,
                ApprovedRequests = membershipRequestsModel.Count(q => q.Approved == true),
                PendingRequests = membershipRequestsModel.Count(q => q.Approved == null),
                RejectedRequests = membershipRequestsModel.Count(q => q.Approved == false),
                MembershipRequests = membershipRequestsModel
            };
            return View(model);
        }

        public ActionResult MyMembership()
        {
            var employee = _userManager.GetUserAsync(User).Result;
            var employeeid = employee.Id;
            var employeeAllocations = _membershipAllocRepo.GetMembershipAllocationsByEmployee(employeeid);
            var employeeRequests = _membershipRequestRepo.GetMembershipRequestsByEmployee(employeeid);

            var employeeAllocationModel = _mapper.Map<List<MembershipAllocationVM>>(employeeAllocations);
            var employeeRequestsModel = _mapper.Map<List<MembershipRequestVM>>(employeeRequests);

            var model = new EmployeeMembershipRequestViewVM
            {
                MembershipAllocations = employeeAllocationModel,
                MembershipRequests = employeeRequestsModel
            };
            return View(model);
        }

        // GET: MembershipRequest/Details/5
        public ActionResult Details(int id)
        {
            var membershipRequest = _membershipRequestRepo.FindById(id);
            var model = _mapper.Map<MembershipRequestVM>(membershipRequest);
            return View(model);
        }

        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var membershipRequest = _membershipRequestRepo.FindById(id);
                var employeeid = membershipRequest.ReqestingEmployee.Id;
                var membershiptypeId = membershipRequest.MembershipTypeId;
                var allocation = _membershipAllocRepo.GetMembershipAllocationsByEmployeeAndType(employeeid, membershiptypeId);

                int daysRequested = (int)(membershipRequest.EndDate - membershipRequest.StartDate).TotalDays;

                allocation.NumberOfDays -= daysRequested;

                membershipRequest.Approved = true;
                membershipRequest.ApprovedById = user.Id;
                membershipRequest.DateActioned = DateTime.Now;
                _membershipRequestRepo.Update(membershipRequest);
                _membershipAllocRepo.Update(allocation);

                return RedirectToAction(nameof(Index));
                
            } catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public ActionResult RejectRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var membershipRequest = _membershipRequestRepo.FindById(id);
                membershipRequest.Approved = false;
                membershipRequest.ApprovedById = user.Id;
                membershipRequest.DateActioned = DateTime.Now;
                _membershipRequestRepo.Update(membershipRequest);
                return RedirectToAction(nameof(Index));

            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: MembershipRequest/Create
        public ActionResult Create()
        {
            var membershipTypes = _membershipTypeRepo.FindAll();
            var membershipTypeItems = membershipTypes.Select(q => new SelectListItem
            {
                Text = q.Name,
                Value = q.Id.ToString()
            });
            var model = new CreateMembershipRequestVM
            {
                MembershipTypes = membershipTypeItems
            };
            return View(model);
        }

        // POST: MembershipRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateMembershipRequestVM model)
        {
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate);
                var membershipTypes = _membershipTypeRepo.FindAll();
                var membershipTypeItems = membershipTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString()
                }); 
                model.MembershipTypes = membershipTypeItems;

                if (!ModelState.IsValid)
                {
                    return View();
                }

                if (DateTime.Compare(startDate, endDate) > 0)
                {
                    ModelState.AddModelError("", "Start date cannot be further in the future than the end date");
                    return View(model);
                }

                var employee = _userManager.GetUserAsync(User).Result;

                var allocation = _membershipAllocRepo.GetMembershipAllocationsByEmployeeAndType(employee.Id, model.MembershipTypeId);

                int daysRequested = (int)(endDate.Date - startDate.Date).TotalDays;
                if(daysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You do not have sufficient days");
                    return View(model);
                }

                var membershipRequestModel = new MembershipRequestVM
                {
                    ReqestingEmployeeId = employee.Id,
                    ReqestingEmployee = employee,
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    MembershipTypeId = model.MembershipTypeId,
                    RequestComments = model.RequestComments
                };

                var membershipRequest = _mapper.Map<MembershipRequest>(membershipRequestModel);
                var isSuccess = _membershipRequestRepo.Create(membershipRequest);

                if(!isSuccess)
                {
                    ModelState.AddModelError("", "Something went wrong with submitting");
                    return View(model);
                }

                return RedirectToAction("MyMembership");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Membership type not allocated yet");
                return View(model);
            }
        }

        public ActionResult CancelRequest(int id)
        {
            var membershipRequest = _membershipRequestRepo.FindById(id);
            membershipRequest.Cancelled = true;
            _membershipRequestRepo.Update(membershipRequest);
            return RedirectToAction("MyMembership");
        }

        // GET: MembershipRequest/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MembershipRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: MembershipRequest/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MembershipRequest/Delete/5
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
