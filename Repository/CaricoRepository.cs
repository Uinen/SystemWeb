using System;
using System.Collections.Generic;
using System.Linq;
using SystemWeb.Models;
using SystemWeb.Repository.Interface;
using System.Data.Entity;

namespace SystemWeb.Repository
{
    public class CaricoRepository : iCaricoRepository
    {
        private MyDbContext db;

        public CaricoRepository(MyDbContext context)
        {
            this.db = context;
        }

        public IEnumerable<Carico> GetOrders()
        {
            return db.Carico.Include(s=>s.Pv).Include(s=> s.Year).ToList();
        }

        public Carico GetOrdersById(Guid? Id)
        {
            return db.Carico.Find(Id);
        }

        public void InsertOrder(Carico insertOrder)
        {
            db.Carico.Add(insertOrder);
        }

        public void UpdateOrder(Carico updateOrder)
        {
            db.Entry(updateOrder).State = EntityState.Modified;
        }
        public void DeleteOrder(Guid? Id)
        {
            Carico deleteOrder = db.Carico.Find(Id);
            db.Carico.Remove(deleteOrder);
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