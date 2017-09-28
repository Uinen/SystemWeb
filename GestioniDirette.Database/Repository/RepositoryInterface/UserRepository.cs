using GestioniDirette.Database.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository.RepositoryInterface
{
    public class UserRepository : iUserRepository
    {
        private MyDbContext _db;
        private bool _disposed = false;
        public UserRepository(MyDbContext context)
        {
            this._db = context;
        }

        public string GetAll()
        {
            return _db.Users.Count().ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
