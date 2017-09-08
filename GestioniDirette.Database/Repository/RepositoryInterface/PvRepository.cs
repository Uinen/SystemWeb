using System;
using System.Collections.Generic;
using System.Linq;
using GestioniDirette.Database.Repository.Interface;
using System.Data.Entity;
using GestioniDirette.Database.Entity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

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
        /// <summary>
        /// Metodo che ritorna l'ID del punto vendita usando l'ID utente fornito dalla classe HttpContext
        /// </summary>
        /// <returns></returns>
        public string GetMyPv()
        {
            ApplicationUser user = HttpContext.Current.GetOwinContext()
                .GetUserManager<ApplicationUserManager>()
                .FindById(HttpContext.Current.User.Identity.GetUserId());

            var _pvID = db.Users.Where(w => w.Id == user.Id).Select(s => s.pvID.ToString()).SingleOrDefault();
            return _pvID;
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