using AutoMapper;
using membership_management.Data;
using membership_management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Mappings
{
    public class Maps : Profile
    {
        public Maps()
        {
            CreateMap<MembershipType, MembershipTypeVM>().ReverseMap(); // either direction
            CreateMap<MembershipRequest, MembershipRequestVM>().ReverseMap();
            CreateMap<MembershipAllocation, MembershipAllocationVM>().ReverseMap();
            CreateMap<MembershipAllocation, EditMembershipAllocationVM>().ReverseMap();
            CreateMap<Employee, EmployeeVM>().ReverseMap();
        }
    }
}
