using System;
using System.Collections.Generic;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository.Interface
{
    public interface iPvCaliRepository : IDisposable
    {
        /// <summary>
        /// GET ALL FUEL ORDERS 
        /// </summary>
        /// <returns>It return all orders placed in the Database</returns>
        IList<PvCali> GetRecords();

        /// <summary>
        /// GET ORDERS BY ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>It return the order by given Id</returns>
        PvCali GetRecordsById(Guid? Id);

        /// <summary>
        /// Insert specified order
        /// </summary>
        /// <param name="insertRecords"></param>
        void InsertRecords(PvCali insertRecords);

        /// <summary>
        /// Update specified order
        /// </summary>
        /// <param name="updateRecords"></param>
        void UpdateRecords(PvCali updateRecords);

        /// <summary>
        /// Delete specified order
        /// </summary>
        /// <param name="Id"></param>
        void DeleteRecords(Guid? Id);

        /// <summary>
        /// Save method
        /// </summary>
        void Save();
    }
}
