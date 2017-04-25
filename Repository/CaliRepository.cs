using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;

namespace SystemWeb.Repository
{
    public static class CaliRepository
    {   
        public static void Add(PvCali cali)
        {
            var context = new MyDbContext();
            context.PvCali.Add(cali);
            context.SaveChanges();
        }

        public static void Add(List<PvCali> cali)
        {
            foreach (var temp in cali)
            {
                var context = new MyDbContext();
                context.PvCali.Add(temp);
                context.SaveChanges();
            }
        }
        /*
        public static void Delete(Guid id)
        {
            MyDbContext context = new MyDbContext();
            var all = (from c in context.PvCali
                       select c);
            var result = all.Single(p => p.PvCaliId == id);
            context.PvCali.Remove(result);
            context.SaveChanges();
        }

        public static void Delete(List<CaliDto> cali)
        {
            foreach (var temp in cali)
            {
                var context = new MyDbContext();
                PvCali result = context.PvCali.Where(o => o.PvCaliId == temp.PvCaliId).FirstOrDefault();
                context.PvCali.Remove(result);
                context.SaveChanges();
            }
        }
        */
        public static void Update(PvCali cali)
        {
            var context = new MyDbContext();
            PvCali result = context.PvCali.Where(o => o.PvCaliId == cali.PvCaliId).FirstOrDefault();
            if (result != null)
            {
                result.PvCaliId = cali.PvCaliId;
                result.PvTankId = cali.PvTankId;
                result.Value = cali.Value;
                result.FieldDate = cali.FieldDate;

                context.Entry(result).CurrentValues.SetValues(cali);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void Update(List<PvCali> cali)
        {
            foreach (var temp in cali)
            {
                var context = new MyDbContext();
                PvCali result = context.PvCali.Where(o => o.PvCaliId == temp.PvCaliId).FirstOrDefault();
                if (result != null)
                {
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
}