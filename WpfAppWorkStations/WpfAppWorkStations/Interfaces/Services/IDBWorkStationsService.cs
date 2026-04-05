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
            bool includeCliens = true,
            bool showUnasigned = true, 
            int mastersId = 0 );

        public void AddOrEditRequest(Request request);
    }
}
