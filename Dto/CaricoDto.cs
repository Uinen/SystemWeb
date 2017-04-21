using System;
using System.ComponentModel.DataAnnotations;
using SystemWeb.Models;

namespace SystemWeb.Dto
{
    public class CaricoDto
    {
        public Guid Id { get; set; }
        public Guid pvID { get; set; }
        public string pvName { get; set; }
        public int yearId { get; set; }
        public int Ordine { get; set; }
        public DateTime cData { get; set; }
        public string Documento { get; set; }
        public string Numero { get; set; }
        public DateTime rData { get; set; }
        public string Emittente { get; set; }
        public int Benzina { get; set; }
        public int Gasolio { get; set; }
        public string Note { get; set; }
        public Pv pv { get; set; }
        public Year year { get; set; }
    }
}