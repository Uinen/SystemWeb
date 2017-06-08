using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository
{
    public static class LicenzaRepository
    {
        public static void Add(Licenza value)
        {
            var context = new MyDbContext();
            context.Licenza.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<Licenza> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.Licenza.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(Licenza value)
        {
            var context = new MyDbContext();
            var result = context.Licenza.FirstOrDefault(o => o.LicenzaID == value.LicenzaID);
            if (result == null) return;
            result.LicenzaID = value.LicenzaID;
            result.pvID = value.pvID;
            result.Codice = value.Codice;
            result.nPrecedente = value.nPrecedente;
            result.nSuccessivo = value.nSuccessivo;
            result.Scadenza = value.Scadenza;

            context.Entry(result).CurrentValues.SetValues(value);
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
        }

        public static void Update(List<Licenza> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                var result = context.Licenza.FirstOrDefault(o => o.LicenzaID == temp.LicenzaID);
                if (result == null) continue;
                result.LicenzaID = temp.LicenzaID;
                result.pvID = temp.pvID;
                result.Codice = temp.Codice;
                result.nPrecedente = temp.nPrecedente;
                result.nSuccessivo = temp.nSuccessivo;
                result.Scadenza = temp.Scadenza;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}