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
            result.Value1 = value.Value1;
            result.Value2 = value.Value2;
            result.Value3 = value.Value3;
            result.Value4 = value.Value4;
            result.Value5 = value.Value5;
            result.Value6 = value.Value6;
            result.Value7 = value.Value7;

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
                result.Value1 = temp.Value1;
                result.Value2 = temp.Value2;
                result.Value3 = temp.Value3;
                result.Value4 = temp.Value4;
                result.Value5 = temp.Value5;
                result.Value6 = temp.Value6;
                result.Value7 = temp.Value7;
                result.FieldDate = temp.FieldDate;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}