using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository
{
    public static class ContatoriRepository
    {
        public static void Add(PvErogatori value)
        {
            var context = new MyDbContext();
            context.PvErogatori.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<PvErogatori> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.PvErogatori.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(PvErogatori value)
        {
            var context = new MyDbContext();
            var result = context.PvErogatori.FirstOrDefault(o => o.PvErogatoriId == value.PvErogatoriId);
            if (result == null) return;
            result.PvErogatoriId = value.PvErogatoriId;
            result.ProductId = value.ProductId;
            result.DispenserId = value.DispenserId;
            result.pvID = value.pvID;
            result.Value = value.Value;
            result.FieldDate = value.FieldDate;

            context.Entry(result).CurrentValues.SetValues(value);
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
        }

        public static void Update(List<PvErogatori> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                var result = context.PvErogatori.FirstOrDefault(o => o.PvErogatoriId == temp.PvErogatoriId);
                if (result == null) continue;
                result.PvErogatoriId = temp.PvErogatoriId;
                result.ProductId = temp.ProductId;
                result.DispenserId = temp.DispenserId;
                result.pvID = temp.pvID;
                result.Value = temp.Value;
                result.FieldDate = temp.FieldDate;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}