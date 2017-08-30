using System;
using System.Collections.Generic;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository.Interface
{
    public interface iPvErogatoriRepository: IDisposable
    {
        /// <summary>
        /// GET ALL PvErogatori
        /// </summary>
        /// <returns>It return all PvErogatori placed in the Database</returns>
        IEnumerable<PvErogatori> GetPvErogatori();

        /// <summary>
        /// GET PvErogatori BY ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>It return the PvErogatori by given Id</returns>
        PvErogatori GetPvErogatoriById(Guid? Id);

        /// <summary>
        /// Insert specified PvErogatori
        /// </summary>
        /// <param name="insertPvErogatori"></param>
        void InsertPvErogatori(PvErogatori insertPvErogatori);

        /// <summary>
        /// Update specified PvErogatori
        /// </summary>
        /// <param name="updatePvErogatori"></param>
        void UpdatePvErogatori(PvErogatori updatePvErogatori);

        /// <summary>
        /// Delete specified PvErogatori
        /// </summary>
        /// <param name="Id"></param>
        void DeletePvErogatori(Guid? Id);

        /// <summary>
        /// Save method
        /// </summary>
        void Save();
    }
}
