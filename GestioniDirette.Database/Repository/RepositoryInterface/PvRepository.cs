using System;
using System.Collections.Generic;
using System.Linq;
using GestioniDirette.Database.Repository.Interface;
using System.Data.Entity;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository
{
    public class PvRepository : iPvRepository
    {
        private MyDbContext db;

        public PvRepository(MyDbContext context)
        {
            this.db = context;
        }

        public IEnumerable<Pv> GetPvs()
        {
            return db.Pv.ToList();
        }

        public Pv GetPvsById(Guid? Id)
        {
            return db.Pv.Find(Id);
        }

        public void InsertPv(Pv insertPv)
        {
            db.Pv.Add(insertPv);
        }

        public void UpdatePv(Pv updatePv)
        {
            db.Entry(updatePv).State = EntityState.Modified;
        }
        public void DeletePv(Guid? Id)
        {
            Pv deletePv = db.Pv.Find(Id);
            db.Pv.Remove(deletePv);
        }
        public void Save()
        {
            db.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}