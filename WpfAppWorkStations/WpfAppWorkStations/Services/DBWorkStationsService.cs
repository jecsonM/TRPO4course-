
using Microsoft.EntityFrameworkCore;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.EntityFramework.Context;
using WpfAppWorkStations.Interfaces.Services;

namespace WpfAppWorkStations.Services
{
    public class DBWorkStationsService : IDBWorkStationsService
    {
        public void AddNewUser(Staff userToAdd)
        {
            using (MachineServicesDbContext dbContext
                 = new MachineServicesDbContext())
            {
                dbContext.Add<Staff>(userToAdd);
                dbContext.SaveChanges();
            }
        }

        public void AddOrEditRequest(Request request)
        {
            throw new NotImplementedException();
        }

        public List<Request> GetRequests(bool includeClients = true, bool showUnassigned = true, int masterId = 0)
        {
            using (MachineServicesDbContext dbContext
                 = new MachineServicesDbContext())
            {
                IQueryable<Request> query = dbContext.Requests;
                
                if (includeClients)
                    query = query.Include(r => r.Client);
                
                if (masterId != 0 && showUnassigned)
                    query = query.Where(r => r.MasterId == masterId || r.MasterId == null);
                if (masterId != 0 && !showUnassigned)
                    query = query.Where(r => r.MasterId == masterId);
                if (masterId == 0 && !showUnassigned)
                    query = query.Where(r => r.MasterId != null);
                 
                return query.ToList();
            }
            
        }

        public Role GetRoleByStaffID(int id)
        {
            using (MachineServicesDbContext dbContext
                 = new MachineServicesDbContext())
            {
                return dbContext.Staff.Find(id).Role;
            }
        }

        public List<Role> GetRoles()
        {
            using (MachineServicesDbContext dbContext 
                = new MachineServicesDbContext() )
            {
                return dbContext.Roles.ToList();
            }
        }

        public Staff GetStaffByLogin(string login)
        {
            using (MachineServicesDbContext dbContext
                = new MachineServicesDbContext())
            {
                return dbContext.Staff.FirstOrDefault(s => s.Login == login);
            }
        }
    }
}
