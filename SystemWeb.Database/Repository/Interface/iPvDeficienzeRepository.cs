using System;
using System.Collections.Generic;
using SystemWeb.Database.Entity;

namespace SystemWeb.Database.Repository.Interface
{
    public interface iPvDeficienzeRepository : IDisposable
    {
        /// <summary>
        /// GET ALL FUEL DEF 
        /// </summary>
        /// <returns>It return all defs placed in the Database</returns>
        IList<PvDeficienze> GetRecords();

        /// <summary>
        /// GET DEF BY ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>It return the def by given Id</returns>
        PvDeficienze GetRecordsById(Guid? Id);

        /// <summary>
        /// Insert specified def
        /// </summary>
        /// <param name="insertRecords"></param>
        void InsertRecords(PvDeficienze insertRecords);

        /// <summary>
        /// Update specified def
        /// </summary>
        /// <param name="updateRecords"></param>
        void UpdateRecords(PvDeficienze updateRecords);

        /// <summary>
        /// Delete specified def
        /// </summary>
        /// <param name="Id"></param>
        void DeleteRecords(Guid? Id);

        /// <summary>
        /// Save method
        /// </summary>
        void Save();
    }
}
