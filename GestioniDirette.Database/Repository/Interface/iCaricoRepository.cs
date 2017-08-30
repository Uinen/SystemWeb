using System;
using System.Collections.Generic;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository.Interface
{
    public interface iCaricoRepository : IDisposable
    {
        /// <summary>
        /// GET ALL FUEL ORDERS 
        /// </summary>
        /// <returns>It return all orders placed in the Database</returns>
        IEnumerable<Carico> GetOrders();

        /// <summary>
        /// GET ORDERS BY ID
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>It return the order by given Id</returns>
        Carico GetOrdersById(Guid? Id);

        /// <summary>
        /// Insert specified order
        /// </summary>
        /// <param name="insertOrder"></param>
        void InsertOrder(Carico insertOrder);

        /// <summary>
        /// Update specified order
        /// </summary>
        /// <param name="updateOrder"></param>
        void UpdateOrder(Carico updateOrder);

        /// <summary>
        /// Delete specified order
        /// </summary>
        /// <param name="Id"></param>
        void DeleteOrder(Guid? Id);

        /// <summary>
        /// Save method
        /// </summary>
        void Save();
    }
}
