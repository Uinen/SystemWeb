using System;
using System.Collections.Generic;
using System.Linq;
using SystemWeb.Database.Repository.Interface;
using System.Data.Entity;
using SystemWeb.Database.Entity;

namespace SystemWeb.Database.Repository
{
    public class PvDeficienzeRepository : iPvDeficienzeRepository
    {
        private MyDbContext db;

        public PvDeficienzeRepository(MyDbContext context)
        {
            this.db = context;
        }

        public IList<PvDeficienze> GetRecords()
        {
            return db.PvDeficienze.Include(s => s.PvTank).ToList();
        }

        public PvDeficienze GetRecordsById(Guid? Id)
        {
            return db.PvDeficienze.Find(Id);
        }

        public void InsertRecords(PvDeficienze insertRecords)
        {
            db.PvDeficienze.Add(insertRecords);
        }

        public void UpdateRecords(PvDeficienze updateRecords)
        {
            db.Entry(updateRecords).State = EntityState.Modified;
        }
        public void DeleteRecords(Guid? Id)
        {
            PvDeficienze deleteRecords = db.PvDeficienze.Find(Id);
            db.PvDeficienze.Remove(deleteRecords);
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