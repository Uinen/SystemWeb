using System;
using System.Collections.Generic;
using System.Linq;
using GestioniDirette.Database.Repository.Interface;
using System.Data.Entity;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository
{
    public class CartissimaRepository : iCartissimaRepository
    {
        private readonly MyDbContext _db;

        public CartissimaRepository(MyDbContext context)
        {
            _db = context;
        }

        public IEnumerable<Cartissima> GetRecords()
        {
            return _db.Cartissima.Include(s => s.Pv).ToList();
        }

        public Cartissima GetRecordsById(Guid? Id)
        {
            return _db.Cartissima.Find(Id);
        }

        public void Insert(Cartissima value)
        {
            _db.Cartissima.Add(value);
        }

        public void Update(Cartissima value)
        {
            _db.Entry(value).State = EntityState.Modified;
        }
        public void Delete(Guid? Id)
        {
            var delete = _db.Cartissima.Find(Id);
            _db.Cartissima.Remove(delete);
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