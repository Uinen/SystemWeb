using GestioniDirette.Database.Entity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace GestioniDirette.Database.Repository
{
    public static class ReclamiRepository
    {
        public static void Add(Reclami value)
        {
            var context = new MyDbContext();
            context.Reclami.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<Reclami> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.Reclami.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(Reclami value)
        {
            var context = new MyDbContext();
            var result = context.Reclami.FirstOrDefault(o => o.ReclamiID == value.ReclamiID);
            if (result == null) return;
            result.ReclamiID = value.ReclamiID;
            result.pvID = value.pvID;
            result.Tipologia = value.Tipologia;
            result.Reclamante = value.Reclamante;
            result.Documento = value.Documento;
            result.NumeroDocumento = value.NumeroDocumento;
            result.Cellulare = value.Cellulare;
            result.ImportoInserito = value.ImportoInserito;
            result.ImportoMancato = value.ImportoMancato;
            result.ImportoRimanente = value.ImportoRimanente;
            result.NumeroInterno = value.NumeroInterno;
            result.NumeroAssegnato = value.NumeroAssegnato;
            result.Note = value.Note;
            result.DataEvento = value.DataEvento;
            result.DataCreazione = value.DataCreazione;

            context.Entry(result).CurrentValues.SetValues(value);
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
        }

        public static void Update(List<Reclami> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                var result = context.Reclami.FirstOrDefault(o => o.ReclamiID == temp.ReclamiID);
                if (result == null) continue;
                result.ReclamiID = temp.ReclamiID;
                result.pvID = temp.pvID;
                result.Tipologia = temp.Tipologia;
                result.Reclamante = temp.Reclamante;
                result.Documento = temp.Documento;
                result.NumeroDocumento = temp.NumeroDocumento;
                result.Cellulare = temp.Cellulare;
                result.ImportoInserito = temp.ImportoInserito;
                result.ImportoMancato = temp.ImportoMancato;
                result.ImportoRimanente = temp.ImportoRimanente;
                result.NumeroInterno = temp.NumeroInterno;
                result.NumeroAssegnato = temp.NumeroAssegnato;
                result.Note = temp.Note;
                result.DataEvento = temp.DataEvento;
                result.DataCreazione = temp.DataCreazione;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
