using membership_management.Contracts;
using membership_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Repository
{
    public class MembershipRequestRepository : IMembershipRequestRepository
    {
        private readonly ApplicationDbContext _db;
        public MembershipRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool Create(MembershipRequest entity)
        {
            _db.MembershipRequests.Add(entity);
            return Save();
        }

        public bool Delete(MembershipRequest entity)
        {
            _db.MembershipRequests.Remove(entity);
            return Save();
        }

        public ICollection<MembershipRequest> FindAll()
        {
            var MembershipHistorys = _db.MembershipRequests
                .Include(q => q.ReqestingEmployee)
                .Include(q => q.ApprovedBy)
                .Include(q => q.MembershipType)
                .ToList();
            return MembershipHistorys;
        }

        public MembershipRequest FindById(int id)
        {
            var MembershipHistorys = _db.MembershipRequests
               .Include(q => q.ReqestingEmployee)
               .Include(q => q.ApprovedBy)
               .Include(q => q.MembershipType)
               .FirstOrDefault(q => q.Id == id);
            return MembershipHistorys;
        }

        public ICollection<MembershipRequest> GetEmployeesByMembershipType()
        {
            throw new NotImplementedException();
        }

        public ICollection<MembershipRequest> GetMembershipRequestsByEmployee(string employeeid)
        {
            var membershipRequests = FindAll()
                .Where(q => q.ReqestingEmployeeId == employeeid)
                .ToList();
            return membershipRequests;
        }

        public bool isExists(int id)
        {
            var exists = _db.MembershipTypes.Any(q => q.Id == id);
            return exists;
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0;
        }

        public bool Update(MembershipRequest entity)
        {
            _db.MembershipRequests.Update(entity);
            return Save();
        }
    }
}
