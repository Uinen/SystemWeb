using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GestioniDirette.Database.Entity
{ 
    public enum FileType
    {
        Avatar = 1,
        Photo,
        Document,
        Das,
        Rdm,
        Xab
    }

    public enum TipoReclamo
    {
        Rimborso,
        ProdottoInquinato,
        Malleazioni,
        Metrico
    }

    public enum DocumentoIdentità
    {
        CartaIdentità,
        Patente,
        Passaporto
    }

    #region Liste
    public class Liste
    {
        public IEnumerable<SelectListItem> Reclamo
        {
            get
            {
                var data = Enum.GetValues(typeof(TipoReclamo)).Cast<TipoReclamo>()
                    .Select(v => new SelectListItem
                    {
                        Text = v.ToString(),
                        Value = v.ToString()
                    }).ToList();

                return data;
            }
        }

        public IEnumerable<SelectListItem> Documento
        {
            get
            {
                var data = Enum.GetValues(typeof(DocumentoIdentità)).Cast<DocumentoIdentità>()
                    .Select(v => new SelectListItem
                    {
                        Text = v.ToString(),
                        Value = v.ToString()
                    }).ToList();

                return data;
            }
        }
    }

    #endregion
}