
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
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                if (request.RequestId == 0)
                {
                    if (request.Client != null)
                    {
                        if (request.Client.ClientId > 0)
                        {
                            dbContext.Clients.Attach(request.Client);
                            dbContext.Entry(request.Client).State = EntityState.Unchanged;
                            request.ClientId = request.Client.ClientId;
                        }
                        else
                        {
                            request.ClientId = 0;
                        }
                    }
                    if (request.Master != null && request.Master.StaffId > 0)
                    {
                        dbContext.Staff.Attach(request.Master);
                        dbContext.Entry(request.Master).State = EntityState.Unchanged;
                        request.MasterId = request.Master.StaffId;
                    }
                    else if (request.MasterId.HasValue && request.MasterId.Value > 0)
                    {
                        var master = dbContext.Staff.Find(request.MasterId.Value);
                        if (master != null)
                        {
                            request.Master = master;
                        }
                    }

                    dbContext.Requests.Add(request);
                }
                else
                {
                    var existingRequest = dbContext.Requests
                        .Include(r => r.Client)
                        .Include(r => r.Master)
                        .FirstOrDefault(r => r.RequestId == request.RequestId);

                    if (existingRequest == null)
                    {
                        throw new ArgumentException($"Заявка с ID {request.RequestId} не найдена");
                    }

                    existingRequest.ServiceAddress = request.ServiceAddress;
                    existingRequest.MasterId = request.MasterId;

                    if (request.Client != null)
                    {
                        if (request.Client.ClientId == 0)
                        {
                            existingRequest.Client = request.Client;
                            existingRequest.ClientId = 0;
                        }
                        else if (request.Client.ClientId != existingRequest.ClientId)
                        {
                            var client = dbContext.Clients.Find(request.Client.ClientId);
                            if (client != null)
                            {
                                existingRequest.Client = client;
                                existingRequest.ClientId = client.ClientId;
                            }
                        }
                    }

                    if (request.Master != null && request.Master.StaffId != existingRequest.MasterId)
                    {
                        var master = dbContext.Staff.Find(request.Master.StaffId);
                        if (master != null)
                        {
                            existingRequest.Master = master;
                            existingRequest.MasterId = master.StaffId;
                        }
                    }
                    else if (request.MasterId.HasValue && request.MasterId != existingRequest.MasterId)
                    {
                        var master = dbContext.Staff.Find(request.MasterId.Value);
                        if (master != null)
                        {
                            existingRequest.Master = master;
                            existingRequest.MasterId = master.StaffId;
                        }
                    }

                    dbContext.Entry(existingRequest).State = EntityState.Modified;
                }

                dbContext.SaveChanges();
            }
        }


        public List<Client> GetClients()
        {
            using (MachineServicesDbContext dbContext
                = new MachineServicesDbContext())
            {
                return dbContext.Clients.ToList();
            }
        }

        public List<Relevantrequeststate> GetRelevantrequests(Request request)
        {
            using (MachineServicesDbContext dbContext
                 = new MachineServicesDbContext())
            {
                return dbContext.Relevantrequeststates
                    .Include(rqs => rqs.RequestState)
                    .Where(rqs => rqs.RequestId == request.RequestId)
                    .OrderByDescending(rqs => rqs.SetDate) 
                    .ToList();
            }
        }

        public List<Request> GetRequests(bool includeClients = true, bool includeMasters = true, bool showUnassigned = true, int masterId = 0)
        {
            using (MachineServicesDbContext dbContext
                 = new MachineServicesDbContext())
            {
                IQueryable<Request> query = dbContext.Requests;
                
                if (includeClients)
                    query = query.Include(r => r.Client);
                if (includeMasters)
                    query = query.Include(r => r.Master);

                if (masterId != 0 && showUnassigned)
                    query = query.Where(r => r.MasterId == masterId || r.MasterId == null);
                if (masterId != 0 && !showUnassigned)
                    query = query.Where(r => r.MasterId == masterId);
                if (masterId == 0 && !showUnassigned)
                    query = query.Where(r => r.MasterId != null);
                 
                return query.ToList();
            }
            
        }

        public List<Requeststate> GetRequeststates()
        {
            throw new NotImplementedException();
        }

        public Role GetRoleByStaffID(int id)
        {
            using (MachineServicesDbContext dbContext
                 = new MachineServicesDbContext())
            {
                return dbContext.Staff
                    .Include(s => s.Role)
                    .FirstOrDefault( s => s.StaffId == id)?
                    .Role;
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
