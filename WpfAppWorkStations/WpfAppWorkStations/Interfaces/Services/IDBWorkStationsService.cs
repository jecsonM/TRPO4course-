using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.EntityFramework;

namespace WpfAppWorkStations.Interfaces.Services
{
    public interface IDBWorkStationsService
    {
        public List<Role> GetRoles();
        public void AddNewUser(Staff userToAdd);
        public Staff GetStaffByLogin(string login);
        public Role GetRoleByStaffID(int id);

        public List<Request> GetRequests(
            bool includeClients = true,
            bool includeMasters = true,
            bool showUnassigned = true, 
            int masterId = 0 );

        public List<Client> GetClients();
        public List<Relevantrequeststate> GetRelevantrequests(Request request);
        public List<Requeststate> GetRequeststates();
        public void AddRelevantRequestState(Relevantrequeststate relevantrequeststate);
        public void AddOrEditRequest(Request request);


        
        List<Order> GetOrders();
        List<Orderstate> GetOrderstates();
        List<Relevantorderstate> GetRelevantorderstates(Order order);
        void AddRelevantOrderState(Relevantorderstate relevantOrderState);
        void AddOrEditOrder(Order order);
        List<Machine> GetMachines();
        void AddOrEditMachine(Machine machine);
        void DeleteMachine(int machineId);
        List<Machine> GetMachinesByClient(int clientId);


        
        List<Machineservice> GetServices();
        List<Relevantcost> GetRelevantCostsByService(int serviceId);
        void AddOrEditService(Machineservice service);
        void AddRelevantCost(Relevantcost relevantCost);
        void DeleteService(int serviceId);


    }
}
