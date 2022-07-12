using membership_management.Contracts;
using membership_management.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Repository
{
    public class MembershipAllocationRepository : IMembershipAllocationRepository
    {
        private readonly ApplicationDbContext _db;
        public MembershipAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CheckAllocation(int membershiptypeid, string employeeid)
        {
            var period = DateTime.Now.Year;
            return FindAll().Where(q => q.EmployeeId == employeeid && q.MembershipTypeId == membershiptypeid && q.Period == period).Any();
        }

        public bool Create(MembershipAllocation entity)
        {
            _db.MembershipAllocations.Add(entity);
            return Save();
        }

        public bool Delete(MembershipAllocation entity)
        {
            _db.MembershipAllocations.Remove(entity);
            return Save();
        }

        public ICollection<MembershipAllocation> FindAll()
        {
            return _db.MembershipAllocations
                .Include(q => q.MembershipType)
                .Include(q => q.Employee)
                .ToList();
        }

        public MembershipAllocation FindById(int id)
        {
            return _db.MembershipAllocations
                .Include(q => q.MembershipType)
                .Include(q => q.Employee)
                .FirstOrDefault(q => q.Id == id);
        }

        public ICollection<MembershipAllocation> GetEmployeesByMembershipType()
        {
            throw new NotImplementedException();
        }

        public ICollection<MembershipAllocation> GetMembershipAllocationsByEmployee(string employeeid)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                .Where(q => q.EmployeeId == employeeid && q.Period == period)
                .ToList();
        }

        public MembershipAllocation GetMembershipAllocationsByEmployeeAndType(string employeeid, int membershiptypeid)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                .FirstOrDefault(q => q.EmployeeId == employeeid && q.Period == period && q.MembershipTypeId == membershiptypeid);
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

        public bool Update(MembershipAllocation entity)
        {
            _db.MembershipAllocations.Update(entity);
            return Save();
        }
    }
}
