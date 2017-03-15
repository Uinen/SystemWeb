using System;
using System.ComponentModel.DataAnnotations;

namespace SystemWeb.ViewModels
{
    public class PvErogatoriViewModel
    {
        public Guid PVEROGATORIID
        {
            get;
            set;
        }

        public string PV
        {
            get;
            set;
        }

        public string PRODUCT
        {
            get;
            set;
        }

        public string DISPENSER
        {
            get;
            set;
        }

        [DataType(DataType.Date), DisplayFormat(
            DataFormatString = "{0:dd/MM/yy}", 
            ApplyFormatInEditMode = true)]
        public DateTime DATE
        {
            get;
            set;
        }

        public int VALUE
        {
            get;
            set;
        }

        public int TOTSSPB
        {
            get;
            set;
        }

        public int TOTDSL
        {
            get;
            set;
        }
    }
}