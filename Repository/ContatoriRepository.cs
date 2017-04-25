using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;

namespace SystemWeb.Repository
{
    public static class ContatoriRepository
    {
        public static void Add(PvErogatori order)
        {
            var context = new MyDbContext();
            context.PvErogatori.Add(order);
        }

        public static void Add(List<PvErogatori> order)
        {
            foreach (var temp in order)
            {
                var context = new MyDbContext();
                context.PvErogatori.Add(temp);
            }
        }

        public static void Update(PvErogatori order)
        {
            var context = new MyDbContext();
            PvErogatori result = context.PvErogatori.Where(o => o.PvErogatoriId == order.PvErogatoriId).FirstOrDefault();
            if (result != null)
            {
                result.PvErogatoriId = order.PvErogatoriId;
                result.ProductId = order.ProductId;
                result.DispenserId = order.DispenserId;
                result.pvID = order.pvID;
                result.Value = order.Value;
                result.FieldDate = order.FieldDate;

                context.Entry(result).CurrentValues.SetValues(order);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void Update(List<PvErogatori> order)
        {
            foreach (var temp in order)
            {
                var context = new MyDbContext();
                PvErogatori result = context.PvErogatori.Where(o => o.PvErogatoriId == temp.PvErogatoriId).FirstOrDefault();
                if (result != null)
                {
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
}