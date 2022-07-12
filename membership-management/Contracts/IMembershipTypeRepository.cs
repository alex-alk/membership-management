using membership_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Contracts
{
    public interface IMembershipTypeRepository : IRepositoryBase<MembershipType>
    {
        ICollection<MembershipType> GetEmployeesByMembershipType();
    }
}
