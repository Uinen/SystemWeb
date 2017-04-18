using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;
using SystemWeb.Repository.Interface;

namespace SystemWeb.Repository
{
    public class ProductRepository : iProductRepository
    {
        private MyDbContext db;

        public ProductRepository(MyDbContext context)
        {
            this.db = context;
        }

        public void DeleteProduct(Guid? Id)
        {
            Product deleteProduct = db.Product.Find(Id);
            db.Product.Remove(deleteProduct);
        }

        public IEnumerable<Product> GetProducts()
        {
            return db.Product.Include(z => z.PvTank).ToList();
        }

        public Product GetProductsById(Guid? Id)
        {
            return db.Product.Find(Id);
        }

        public void InsertProduct(Product insertProduct)
        {
            db.Product.Add(insertProduct);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void UpdateProduct(Product updateProduct)
        {
            db.Entry(updateProduct).State = EntityState.Modified;
        }

        #region IDisposable Support
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
        #endregion
    }
}