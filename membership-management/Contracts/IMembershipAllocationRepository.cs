using membership_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Contracts
{
    public interface IMembershipAllocationRepository : IRepositoryBase<MembershipAllocation>
    {
        bool CheckAllocation(int membershiptypeid, string employeeid);
        ICollection<MembershipAllocation> GetMembershipAllocationsByEmployee(string employeeid );
        MembershipAllocation GetMembershipAllocationsByEmployeeAndType(string employeeid, int membershiptypeid);
    }
}
