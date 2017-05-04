using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;

namespace SystemWeb.Repository
{
    public static class CaliRepository
    {   
        public static void Add(PvCali value)
        {
            var context = new MyDbContext();
            context.PvCali.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<PvCali> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.PvCali.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(PvCali value)
        {
            var context = new MyDbContext();
            var result = context.PvCali.FirstOrDefault(o => o.PvCaliId == value.PvCaliId);
            if (result == null) return;
            result.PvCaliId = value.PvCaliId;
            result.PvTankId = value.PvTankId;
            result.Value = value.Value;
            result.FieldDate = value.FieldDate;

            context.Entry(result).CurrentValues.SetValues(value);
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
        }

        public static void Update(List<PvCali> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                var result = context.PvCali.FirstOrDefault(o => o.PvCaliId == temp.PvCaliId);
                if (result == null) continue;
                result.PvCaliId = temp.PvCaliId;
                result.PvTankId = temp.PvTankId;
                result.Value = temp.Value;
                result.FieldDate = temp.FieldDate;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}