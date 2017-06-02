using System;
using System.Collections.Generic;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository.Interface
{
    public interface iCartissimaRepository : IDisposable
    {
        /// <summary>
        /// Ritorna tutte le segnalazioni
        /// </summary>
        /// <returns>Ritorna tutte le segnalazioni effettuate nel database</returns>
        IEnumerable<Cartissima> GetRecords();

        /// <summary>
        /// Ritorna la segnalazione mediante id
        /// </summary>
        /// <param name="Id"></param>
        Cartissima GetRecordsById(Guid? Id);

        /// <summary>
        /// Inserisce la segnalazione
        /// </summary>
        /// <param name="value"></param>
        void Insert(Cartissima value);

        /// <summary>
        /// Aggiorna la segnalazione
        /// </summary>
        /// <param name="updateOrder"></param>
        void Update(Cartissima value);

        /// <summary>
        /// Cancella la segnalazione
        /// </summary>
        /// <param name="Id"></param>
        void Delete(Guid? Id);

        /// <summary>
        /// Salva
        /// </summary>
        void Save();
    }
}
