using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository.Interface
{
    public interface iUserRepository : IDisposable
    {
        string GetAll();
    }
}
