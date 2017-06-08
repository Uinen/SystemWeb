using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository
{
    public static class DispenserRepository
    {
        public static void Add(Dispenser value)
        {
            var context = new MyDbContext();

            if (value.isActive == true)
            {
                value.isActive = true;
            }
            else
            {
                value.isActive = false;
            }

            context.Dispenser.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<Dispenser> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();

                if (temp.isActive == true)
                {
                    temp.isActive = true;
                }
                else
                {
                    temp.isActive = false;
                }

                context.Dispenser.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(Dispenser value)
        {
            var context = new MyDbContext();
            var result = context.Dispenser.FirstOrDefault(o => o.DispenserId == value.DispenserId);
            if (result == null) return;
            result.DispenserId = value.DispenserId;
            result.PvTankId = value.PvTankId;
            result.Modello = value.Modello;
            result.isActive = value.isActive;
            result.Scadenza = value.Scadenza;
                
            context.Entry(result).CurrentValues.SetValues(value);
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
        }

        public static void Update(List<Dispenser> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                var result = context.Dispenser.FirstOrDefault(o => o.DispenserId == temp.DispenserId);
                if (result == null) continue;
                result.DispenserId = temp.DispenserId;
                result.PvTankId = temp.PvTankId;
                result.Modello = temp.Modello;
                result.isActive = temp.isActive;
                result.Scadenza = temp.Scadenza;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}