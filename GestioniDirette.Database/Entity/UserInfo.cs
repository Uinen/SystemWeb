using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System;
using GestioniDirette.Database.Utility;
using GestioniDirette.Database.Repository;
using GestioniDirette.Database.Repository.Interface;

namespace GestioniDirette.Database.Entity
{
    public class UserInfo
    {
        private readonly MyDbContext _db = new MyDbContext();
        private readonly iPvRepository _pvRepository;
        public UserInfo()
        {
            _pvRepository = new PvRepository(new MyDbContext());
        }

        public string CachePVID()
        {
            InMemoryCache cacheService = new InMemoryCache();
            var cache = cacheService.GetOrSet("user.pvID", () => _pvRepository.GetMyPv());
            return cache;
        }
    }
}
