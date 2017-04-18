using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;
using SystemWeb.Repository.Interface;

namespace SystemWeb.Repository
{
    public class PvErogatoriRepository : iPvErogatoriRepository
    {
        private MyDbContext db;

        public PvErogatoriRepository(MyDbContext context)
        {
            this.db = context;
        }

        public void DeletePvErogatori(Guid? Id)
        {
            PvErogatori deletePvErogatori = db.PvErogatori.Find(Id);
            db.PvErogatori.Remove(deletePvErogatori);
        }

        public IEnumerable<PvErogatori> GetPvErogatori()
        {
            return db.PvErogatori.Include(s => s.Product).ToList();
        }

        public PvErogatori GetPvErogatoriById(Guid? Id)
        {
            return db.PvErogatori.Find(Id);
        }

        public void InsertPvErogatori(PvErogatori insertPvErogatori)
        {
            db.PvErogatori.Add(insertPvErogatori);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void UpdatePvErogatori(PvErogatori updatePvErogatori)
        {
            db.Entry(updatePvErogatori).State = EntityState.Modified;
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