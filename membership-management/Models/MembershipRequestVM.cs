using membership_management.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Models
{
    public class MembershipRequestVM
    {
        public int Id { get; set; }
        public Employee ReqestingEmployee { get; set; }
        [Display(Name = "Employee name")]
        public string ReqestingEmployeeId { get; set; }
        [Display(Name = "Start date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Display(Name = "End date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public MembershipTypeVM MembershipType { get; set; }
        public int MembershipTypeId { get; set; }

        [Display(Name = "Date requested")]
        public DateTime DateRequested { get; set; }

        [Display(Name = "Date actioned")]
        public DateTime DateActioned { get; set; }
        [Display(Name = "Approval state")]
        public bool? Approved { get; set; }
        public Employee ApprovedBy { get; set; }
        [Display(Name = "Approver name")]
        public string ApprovedById { get; set; }
        [Display(Name = "Employee comments")]
        [MaxLength(300)]
        public string RequestComments { get; set; }
        public bool Cancelled { get; set; }
    }

    public class AdminMembershipRequestViewVM
    {
        [Display(Name = "Total number of requests")]
        public int TotalRequests { get; set; }
        [Display(Name = "Approved requests")]
        public int ApprovedRequests { get; set; }
        [Display(Name = "Pending requests")]
        public int PendingRequests { get; set; }
        [Display(Name = "Rejected requests")]
        public int RejectedRequests { get; set; }
        public List<MembershipRequestVM> MembershipRequests { get; set; }
    }

    public class CreateMembershipRequestVM
    {
        [Display(Name = "Start date")]
        [Required]
        public string StartDate { get; set; }
        [Display(Name = "End date")]
        [Required]
        public string EndDate { get; set; }
        public IEnumerable<SelectListItem> MembershipTypes { get; set; }
        [Display(Name = "Membership type")]
        public int MembershipTypeId { get; set; }
        public bool Cancelled { get; set; }
        [Display(Name = "Employee comments")]
        [MaxLength(300)]
        public string RequestComments { get; set; }
    }

    public class EmployeeMembershipRequestViewVM
    {
        public List<MembershipAllocationVM> MembershipAllocations { get; set; }
        public List<MembershipRequestVM> MembershipRequests { get; set; }
    }
}
