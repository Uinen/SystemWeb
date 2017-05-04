using System;
using System.Collections.Generic;
using System.Linq;
using SystemWeb.Models;
using SystemWeb.Repository.Interface;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace SystemWeb.Repository
{
    public class CaricoRepository : iCaricoRepository
    {
        private readonly MyDbContext _db;

        public CaricoRepository(MyDbContext context)
        {
            _db = context;
        }

        public IEnumerable<Carico> GetOrders()
        {
            return _db.Carico.Include(s=>s.Pv).Include(s=> s.Year).ToList();
        }

        public Carico GetOrdersById(Guid? Id)
        {
            return _db.Carico.Find(Id);
        }

        public void InsertOrder(Carico insertOrder)
        {
            _db.Carico.Add(insertOrder);
        }

        public void UpdateOrder(Carico updateOrder)
        {
            _db.Entry(updateOrder).State = EntityState.Modified;
        }
        public void DeleteOrder(Guid? Id)
        {
            var deleteOrder = _db.Carico.Find(Id);
            _db.Carico.Remove(deleteOrder);
        }
        public void Save()
        {
            _db.SaveChanges();
        }

        private bool _disposed = false;

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