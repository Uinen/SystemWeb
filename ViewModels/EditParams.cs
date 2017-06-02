using System;
using System.Collections.Generic;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Models
{
    public class EditParams
    {
        //Paging Params
        public int Skip { get; set; }
        public int Take { get; set; }

        //Grid Action Params
        public string Action { get; set; }
        public Guid Key { get; set; }
        public string KeyColumn { get; set; }
        public Carico Value { get; set; }

        //Batch Edit Params
        public IEnumerable<Carico> Added { get; set; }
        public IEnumerable<Carico> Changed { get; set; }
        public IEnumerable<Carico> Deleted { get; set; }
    }
}