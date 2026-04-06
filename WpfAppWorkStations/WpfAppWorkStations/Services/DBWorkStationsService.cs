
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

        public void AddOrEditOrder(Order order)
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                if (order.OrderId == 0)
                {
                    // НОВЫЙ ЗАКАЗ

                    // Обрабатываем связанную заявку
                    if (order.Request != null)
                    {
                        if (order.Request.RequestId > 0)
                        {
                            dbContext.Requests.Attach(order.Request);
                            dbContext.Entry(order.Request).State = EntityState.Unchanged;
                            order.RequestId = order.Request.RequestId;
                        }
                        else
                        {
                            order.RequestId = 0;
                        }
                    }
                    else if (order.RequestId > 0)
                    {
                        var request = dbContext.Requests.Find(order.RequestId);
                        if (request != null)
                        {
                            order.Request = request;
                        }
                    }

                    // Обрабатываем оборудование
                    if (order.Machines != null && order.Machines.Any())
                    {
                        foreach (var machine in order.Machines)
                        {
                            if (machine.MachineId > 0)
                            {
                                dbContext.Machines.Attach(machine);
                                dbContext.Entry(machine).State = EntityState.Unchanged;
                            }
                        }
                    }

                    // Обрабатываем услуги
                    if (order.Serviceprovisions != null && order.Serviceprovisions.Any())
                    {
                        foreach (var serviceProvision in order.Serviceprovisions)
                        {
                            if (serviceProvision.ServiceId > 0)
                            {
                                dbContext.Machineservices.Attach(serviceProvision.Service);
                                dbContext.Entry(serviceProvision.Service).State = EntityState.Unchanged;
                            }
                        }
                    }

                    dbContext.Orders.Add(order);
                    dbContext.SaveChanges();

                    
                    Relevantorderstate relevantOrderState = new Relevantorderstate()
                    {
                        OrderId = order.OrderId,
                        OrderStateId = dbContext.Orderstates
                            .First(os => os.OrderStateName == "Сформирован")
                            .OrderStateId,
                        SetDate = DateTime.UtcNow
                    };
                    dbContext.Relevantorderstates.Add(relevantOrderState);
                }
                else
                {
                    
                    var existingOrder = dbContext.Orders
                        .Include(o => o.Request)
                        .Include(o => o.Machines)
                        .Include(o => o.Serviceprovisions)
                        .FirstOrDefault(o => o.OrderId == order.OrderId);

                    if (existingOrder == null)
                    {
                        throw new ArgumentException($"Заказ с ID {order.OrderId} не найден");
                    }

                    
                    existingOrder.CreationDate = order.CreationDate;

                    
                    if (order.Request != null)
                    {
                        if (order.Request.RequestId == 0)
                        {
                            existingOrder.Request = order.Request;
                            existingOrder.RequestId = 0;
                        }
                        else if (order.Request.RequestId != existingOrder.RequestId)
                        {
                            var request = dbContext.Requests.Find(order.Request.RequestId);
                            if (request != null)
                            {
                                existingOrder.Request = request;
                                existingOrder.RequestId = request.RequestId;
                            }
                        }
                    }
                    else if (order.RequestId > 0 && order.RequestId != existingOrder.RequestId)
                    {
                        var request = dbContext.Requests.Find(order.RequestId);
                        if (request != null)
                        {
                            existingOrder.Request = request;
                            existingOrder.RequestId = request.RequestId;
                        }
                    }

                    
                    if (order.Machines != null)
                    {
                        existingOrder.Machines.Clear();
                        foreach (var machine in order.Machines)
                        {
                            if (machine.MachineId > 0)
                            {
                                var attachedMachine = dbContext.Machines.Find(machine.MachineId);
                                if (attachedMachine != null)
                                {
                                    existingOrder.Machines.Add(attachedMachine);
                                }
                            }
                            else
                            {
                                existingOrder.Machines.Add(machine);
                            }
                        }
                    }

                    // Обновляем услуги
                    if (order.Serviceprovisions != null)
                    {
                        existingOrder.Serviceprovisions.Clear();
                        foreach (var serviceProvision in order.Serviceprovisions)
                        {
                            if (serviceProvision.ServiceId > 0)
                            {
                                var attachedService = dbContext.Machineservices.Find(serviceProvision.ServiceId);
                                if (attachedService != null)
                                {
                                    serviceProvision.Service = attachedService;
                                    existingOrder.Serviceprovisions.Add(serviceProvision);
                                }
                            }
                            else
                            {
                                existingOrder.Serviceprovisions.Add(serviceProvision);
                            }
                        }
                    }

                    dbContext.Entry(existingOrder).State = EntityState.Modified;
                }

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

                    dbContext.SaveChanges();

                    Relevantrequeststate relevantrequeststate = 
                        new Relevantrequeststate() { 
                            RequestId = request.RequestId, 
                            RequestStateId = dbContext.Requeststates
                            .First(rs => rs.RequestStateName == "Создана")
                            .RequestStateId,
                            SetDate = DateTime.UtcNow
                        };
                    dbContext.Relevantrequeststates.Add(relevantrequeststate);

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

        public void AddRelevantOrderState(Relevantorderstate relevantOrderState)
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                var newState = new Relevantorderstate
                {
                    OrderId = relevantOrderState.OrderId,
                    OrderStateId = relevantOrderState.OrderStateId,
                    SetDate = relevantOrderState.SetDate == default ? DateTime.UtcNow : relevantOrderState.SetDate
                };

                dbContext.Relevantorderstates.Add(newState);
                dbContext.SaveChanges();
            }
        }

        public void AddRelevantRequestState(Relevantrequeststate relevantrequeststate)
        {
            using (MachineServicesDbContext dbContext
                = new MachineServicesDbContext())
            {
                var newState = new Relevantrequeststate
                {
                    RequestId = relevantrequeststate.RequestId,
                    RequestStateId = relevantrequeststate.RequestStateId,
                    SetDate = relevantrequeststate.SetDate
                };

                dbContext.Relevantrequeststates.Add(newState);
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

        public List<Order> GetOrders()
        {
            using (MachineServicesDbContext dbContext
                = new MachineServicesDbContext())
            {
                return dbContext.Orders
                    .Include(o => o.Request)
                    .Include(o => o.Serviceprovisions)
                        .ThenInclude(sp => sp.Service)
                        .ThenInclude(sp => sp.Relevantcosts)
                    .Include(o => o.Machines)
                    .ToList();
            }
        }

        public List<Orderstate> GetOrderstates()
        {
            using (MachineServicesDbContext dbContext
                = new MachineServicesDbContext())
            {
                return dbContext.Orderstates.ToList();
            }
        }

        public List<Relevantorderstate> GetRelevantorderstates(Order order)
        {
            using (MachineServicesDbContext dbContext
                 = new MachineServicesDbContext())
            {
                return dbContext.Relevantorderstates
                    .Include(ros => ros.OrderState)
                    .Where(ros => ros.OrderId == order.OrderId)
                    .OrderByDescending(ros => ros.SetDate)
                    .ToList();
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
        public void DeleteMachine(int machineId)
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                var machine = dbContext.Machines.Find(machineId);

                if (machine == null)
                {
                    throw new ArgumentException($"Оборудование с ID {machineId} не найдено");
                }

                // Проверяем, есть ли связанные заказы
                var hasOrders = dbContext.Orders.Any(o => o.Machines.Any(m => m.MachineId == machineId));
                if (hasOrders)
                {
                    throw new InvalidOperationException("Нельзя удалить оборудование, так как оно используется в заказах");
                }

                dbContext.Machines.Remove(machine);
                dbContext.SaveChanges();
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
            using (MachineServicesDbContext dbContext
                 = new MachineServicesDbContext())
            {
                return dbContext.Requeststates.ToList();
            }
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

        public List<Machine> GetMachines()
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                return dbContext.Machines
                    .Include(m => m.Client)
                    .ToList();
            }
        }

        public void AddOrEditMachine(Machine machine)
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                if (machine.MachineId == 0)
                {
                    if (machine.Client != null && machine.Client.ClientId > 0)
                    {
                        dbContext.Clients.Attach(machine.Client);
                        dbContext.Entry(machine.Client).State = EntityState.Unchanged;
                        machine.ClientId = machine.Client.ClientId;
                        machine.Client = null;
                    }
                    else if (machine.ClientId > 0)
                    {
                        var client = dbContext.Clients.Find(machine.ClientId);
                        if (client != null)
                        {
                            machine.Client = client;
                        }
                    }

                    dbContext.Machines.Add(machine);
                }
                else
                {
                    var existingMachine = dbContext.Machines.Find(machine.MachineId);

                    if (existingMachine == null)
                    {
                        throw new ArgumentException($"Оборудование с ID {machine.MachineId} не найдено");
                    }

                    existingMachine.Model = machine.Model;
                    existingMachine.SerialNumber = machine.SerialNumber;
                    existingMachine.MastersComment = machine.MastersComment;
                    existingMachine.ClientId = machine.ClientId;

                    if (machine.Client != null && machine.Client.ClientId != existingMachine.ClientId)
                    {
                        var client = dbContext.Clients.Find(machine.Client.ClientId);
                        if (client != null)
                        {
                            existingMachine.Client = client;
                            existingMachine.ClientId = client.ClientId;
                        }
                    }

                    dbContext.Entry(existingMachine).State = EntityState.Modified;
                }

                dbContext.SaveChanges();
            }
        }
        public List<Machine> GetMachinesByClient(int clientId)
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                return dbContext.Machines
                    .Include(m => m.Client)
                    .Where(m => m.ClientId == clientId)
                    .ToList();
            }
        }



        public List<Machineservice> GetServices()
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                return dbContext.Machineservices.ToList();
            }
        }

        public List<Relevantcost> GetRelevantCostsByService(int serviceId)
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                return dbContext.Relevantcosts
                    .Include(rc => rc.Creators)
                    .Where(rc => rc.ServiceId == serviceId)
                    .OrderByDescending(rc => rc.SetDate)
                    .ToList();
            }
        }

        public void AddOrEditService(Machineservice service)
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                if (service.ServiceId == 0)
                {
                    dbContext.Machineservices.Add(service);
                }
                else
                {
                    var existingService = dbContext.Machineservices.Find(service.ServiceId);
                    if (existingService == null)
                    {
                        throw new ArgumentException($"Услуга с ID {service.ServiceId} не найдена");
                    }

                    existingService.MachineServiceName = service.MachineServiceName;
                    dbContext.Entry(existingService).State = EntityState.Modified;
                }

                dbContext.SaveChanges();
            }
        }

        public void AddRelevantCost(Relevantcost relevantCost)
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                // Прикрепляем существующую услугу
                if (relevantCost.Service != null && relevantCost.ServiceId > 0)
                {
                    dbContext.Machineservices.Attach(relevantCost.Service);
                    dbContext.Entry(relevantCost.Service).State = EntityState.Unchanged;
                }

                // Прикрепляем создателя
                if (relevantCost.Creators != null && relevantCost.CreatorsId > 0)
                {
                    dbContext.Staff.Attach(relevantCost.Creators);
                    dbContext.Entry(relevantCost.Creators).State = EntityState.Unchanged;
                }

                dbContext.Relevantcosts.Add(relevantCost);
                dbContext.SaveChanges();
            }
        }

        public void DeleteService(int serviceId)
        {
            using (MachineServicesDbContext dbContext = new MachineServicesDbContext())
            {
                var service = dbContext.Machineservices.Find(serviceId);

                if (service == null)
                {
                    throw new ArgumentException($"Услуга с ID {serviceId} не найдена");
                }

                // Проверяем, есть ли связанные записи
                var hasServiceProvisions = dbContext.Serviceprovisions.Any(sp => sp.ServiceId == serviceId);
                if (hasServiceProvisions)
                {
                    throw new InvalidOperationException("Нельзя удалить услугу, так как она используется в заказах");
                }

                dbContext.Machineservices.Remove(service);
                dbContext.SaveChanges();
            }
        }


    }
}
