using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppWorkStations.EntityFramework;
using WpfAppWorkStations.Interfaces.Services;

namespace WpfAppWorkStations.Services
{
    public class DBWorkStationsService : IDBWorkStationsService
    {
        public Role GetRoleByStaffID(int id)
        {
            throw new NotImplementedException();
        }

        public Staff GetStaffByLogin(string login)
        {
            throw new NotImplementedException();
        }
    }
}
