using System;

namespace SystemWeb.Models
{
    public class LoadViewModels
    {
        public string vOrdine { get; set; }
        public DateTime vCData { get; set; }
        public string vNumero { get; set; }
        public DateTime vRData { get; set; }
        public string vEmittente { get; set; }
        public int vBenzina { get; set; }
        public int vGasolio { get; set; }
        public string vNote { get; set; }
        public int SSPBTotalAmount { get; set; }
        public int DieselTotalAmount { get; set; }
        public DateTime fData { get; set; }
        public DateTime tData { get; set; }
    }
}