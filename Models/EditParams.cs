using System;
using System.Collections.Generic;

namespace SystemWeb.Models
{
    public class EditParams
    {
        //Paging Params
        public int skip { get; set; }
        public int take { get; set; }

        //Grid Action Params
        public string action { get; set; }
        public Guid key { get; set; }
        public string keyColumn { get; set; }
        public Carico value { get; set; }

        //Batch Edit Params
        public IEnumerable<Carico> added { get; set; }
        public IEnumerable<Carico> changed { get; set; }
        public IEnumerable<Carico> deleted { get; set; }
    }
}