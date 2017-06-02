using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository
{
    public static class DepotRepository
    {
        public static void Add(Deposito value)
        {
            var context = new MyDbContext();
            context.Deposito.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<Deposito> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.Deposito.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(Deposito value)
        {
            var context = new MyDbContext();
            var result = context.Deposito.FirstOrDefault(o => o.depId == value.depId);

            if (result == null) return;

            result.depId = value.depId;
            result.Nome = value.Nome;
            context.Entry(result).CurrentValues.SetValues(value);
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
        }

        public static void Update(List<Deposito> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                var result = context.Deposito.FirstOrDefault(o => o.depId == temp.depId);
                if (result == null) continue;

                result.depId = temp.depId;
                result.Nome = temp.Nome;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
