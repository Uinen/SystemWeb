using GestioniDirette.Database.Utility;
using GestioniDirette.Database.Repository;
using GestioniDirette.Database.Repository.Interface;
using GestioniDirette.Database.Repository.RepositoryInterface;

namespace GestioniDirette.Database.Entity
{
    public class UserInfo
    {
        private readonly MyDbContext _db = new MyDbContext();
        private readonly InMemoryCache _cacheService = new InMemoryCache();
        private readonly iPvRepository _pvRepository;
        private readonly iUserRepository _userRepository;
        public UserInfo()
        {
            _pvRepository = new PvRepository(new MyDbContext());
            _userRepository = new UserRepository(new MyDbContext());
        }

        /// <summary>
        /// Metodo che ritorna il punto vendita in base al utenza loggata.
        /// </summary>
        /// <returns></returns>
        public string GetPVID()
        {
            var cache = _cacheService.GetOrSet("user.pvID", () => _pvRepository.GetMyPv());
            return cache;
        }

        /// <summary>
        /// Metodo che ritorna il numero di utenti salvati nel Database
        /// </summary>
        /// <returns></returns>
        public string GetTotalUsers()
        {
            var cache = _cacheService.GetOrSet("user.total", () => _userRepository.GetAll());
            return cache;
        }
    }
}
