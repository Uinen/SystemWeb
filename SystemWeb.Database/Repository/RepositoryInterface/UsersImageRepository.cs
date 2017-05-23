using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Database.Entity;
using SystemWeb.Database.Repository.Interface;

namespace SystemWeb.Database.Repository
{
    public class UsersImageRepository : iUsersImageRepository
    {
        private MyDbContext db;

        public UsersImageRepository(MyDbContext context)
        {
            this.db = context;
        }
        public void DeleteUsersImages(Guid? Id)
        {
            UsersImage deleteUsersImage = db.UsersImage.Find(Id);
            db.UsersImage.Remove(deleteUsersImage);
        }

        public IEnumerable<UsersImage> GetUsersImages()
        {
            return db.UsersImage.Include(z => z.UserProfiles).ToList();
        }

        public UsersImage GetUsersImagesById(Guid? Id)
        {
            return db.UsersImage.Find(Id);
        }

        public void InsertUsersImages(UsersImage insertUsersImages)
        {
            db.UsersImage.Add(insertUsersImages);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public void UpdateUsersImages(UsersImage updateUsersImages)
        {
            db.Entry(updateUsersImages).State = EntityState.Modified;
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