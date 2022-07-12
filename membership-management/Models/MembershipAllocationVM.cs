using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Models
{
    public class MembershipAllocationVM
    {

        public int Id { get; set; }
        public int NumberOfDays { get; set; }
        public DateTime DateCreated { get; set; }
        public int Period { get; set; }
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        public MembershipTypeVM MembershipType { get; set; }
        public int MembershipTypeId { get; set; }
    }

    public class CreateMembershipAllocationVM
    {
        public int NumberUpdated { get; set; }
        public List<MembershipTypeVM> MembershipTypes { get; set; }
    }

    public class EditMembershipAllocationVM
    {
        public int Id { get; set; }
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        public int NumberOfDays { get; set; }
        public MembershipTypeVM MembershipType { get; set; }
    }

    public class ViewAllocationsVM
    {
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; }
        public List<MembershipAllocationVM> MembershipAllocations { get; set; }
    }
}
