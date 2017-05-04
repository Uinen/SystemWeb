using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;

namespace SystemWeb.Repository
{
    public static class TankRepository
    {
        public static void Add(PvTank value)
        {
            var context = new MyDbContext();
            context.PvTank.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<PvTank> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.PvTank.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(PvTank value)
        {
            var context = new MyDbContext();
            var result = context.PvTank.FirstOrDefault(o => o.PvTankId == value.PvTankId);
            if (result == null) return;
            result.PvTankId = value.PvTankId;
            result.pvID = value.pvID;
            result.ProductId = value.ProductId;
            result.Modello = value.Modello;
            result.Capienza = value.Capienza;
            result.Giacenza = value.Giacenza;
            result.LastDate = value.LastDate;
            result.Descrizione = value.Descrizione;

            context.Entry(result).CurrentValues.SetValues(value);
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
        }

        public static void Update(List<PvTank> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                var result = context.PvTank.FirstOrDefault(o => o.PvTankId == temp.PvTankId);
                if (result == null) continue;
                result.PvTankId = temp.PvTankId;
                result.pvID = temp.pvID;
                result.ProductId = temp.ProductId;
                result.Modello = temp.Modello;
                result.Capienza = temp.Capienza;
                result.Giacenza = temp.Giacenza;
                result.LastDate = temp.LastDate;
                result.Descrizione = temp.Descrizione;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}