using membership_management.Contracts;
using membership_management.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace membership_management.Repository
{
    public class MembershipTypeRepository : IMembershipTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public MembershipTypeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool Create(MembershipType entity)
        {
            _db.MembershipTypes.Add(entity);
            return Save();
        }

        public bool Delete(MembershipType entity)
        {
            _db.MembershipTypes.Remove(entity);
            return Save ();
        }

        public ICollection<MembershipType> FindAll()
        {
            return _db.MembershipTypes.ToList();
        }

        public MembershipType FindById(int id)
        {
            return _db.MembershipTypes.Find(id);
        }

        public ICollection<MembershipType> GetEmployeesByMembershipType()
        {
            throw new NotImplementedException();
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

        public bool Update(MembershipType entity)
        {
            _db.MembershipTypes.Update(entity);
            return Save();
        }
    }
}
