using System;
using System.Collections.Generic;
using GestioniDirette.Database.Entity;
using System.Linq;

namespace GestioniDirette.Database.Repository.Interface
{
    public interface iPvRepository : IDisposable
    {
        /// <summary>
        /// GET ALL PV
        /// </summary>
        /// <returns>It return all pv placed in the Database</returns>
        IEnumerable<Pv> GetPvs();

        /// <summary>
        /// GET PV BY ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>It return the pv by given Id</returns>
        Pv GetPvsById(Guid? Id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GetMyPv();

        /// <summary>
        /// Insert specified pv
        /// </summary>
        /// <param name="insertPv"></param>
        void InsertPv(Pv insertPv);

        /// <summary>
        /// Update specified pv
        /// </summary>
        /// <param name="updatePv"></param>
        void UpdatePv(Pv updatePv);

        /// <summary>
        /// Delete specified pv
        /// </summary>
        /// <param name="Id"></param>
        void DeletePv(Guid? Id);

        /// <summary>
        /// Save method
        /// </summary>
        void Save();
    }
}
