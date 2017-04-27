using System;
using System.Collections.Generic;
using System.Linq;
using SystemWeb.Models;
using SystemWeb.Repository.Interface;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace SystemWeb.Repository
{
    public class PvCaliRepository : iPvCaliRepository
    {
        private MyDbContext db;

        public PvCaliRepository(MyDbContext context)
        {
            this.db = context;
        }

        public IList<PvCali> GetRecords()
        {
            return db.PvCali.Include(s => s.PvTank).ToList();
        }

        public PvCali GetRecordsById(Guid? Id)
        {
            return db.PvCali.Find(Id);
        }

        public void InsertRecords(PvCali insertRecords)
        {
            db.PvCali.Add(insertRecords);
        }

        public void UpdateRecords(PvCali updateRecords)
        {
            db.Entry(updateRecords).State = EntityState.Modified;
        }
        public void DeleteRecords(Guid? Id)
        {
            PvCali deleteRecords = db.PvCali.Find(Id);
            db.PvCali.Remove(deleteRecords);
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