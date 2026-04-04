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
        public Staff GetStaffByLogin(string login);
        public Role GetRoleByStaffID(int id);
    }
}
