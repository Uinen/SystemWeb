using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;

namespace SystemWeb.Repository
{
    public static class CartissimaRepositorySync
    {
        public static void Add(Cartissima value)
        {
            var context = new MyDbContext();
            context.Cartissima.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<Cartissima> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.Cartissima.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(Cartissima value)
        {
            var context = new MyDbContext();
            var result = context.Cartissima.FirstOrDefault(o => o.sCartId == value.sCartId);

            if (result == null) return;

            result.sCartId = value.sCartId;
            result.pvID = value.pvID;
            result.sCartProcessed = value.sCartProcessed;
            result.sCartName = value.sCartName;
            result.sCartSurname = value.sCartSurname;
            result.sCartEmail = value.sCartEmail;
            result.sCartPhone = value.sCartPhone;
            result.sCartCompany = value.sCartCompany;
            result.sCartIva = value.sCartIva;
            result.sCartLocation = value.sCartLocation;
            result.sCartVeichle = value.sCartVeichle;
            result.sCartVeichleType = value.sCartVeichleType;

            context.Entry(result).CurrentValues.SetValues(value);
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
        }

        public static void Update(List<Cartissima> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                var result = context.Cartissima.FirstOrDefault(o => o.sCartId == temp.sCartId);
                if (result == null) continue;

                result.sCartId = temp.sCartId;
                result.pvID = temp.pvID;
                result.sCartProcessed = temp.sCartProcessed;
                result.sCartName = temp.sCartName;
                result.sCartSurname = temp.sCartSurname;
                result.sCartEmail = temp.sCartEmail;
                result.sCartPhone = temp.sCartPhone;
                result.sCartCompany = temp.sCartCompany;
                result.sCartIva = temp.sCartIva;
                result.sCartLocation = temp.sCartLocation;
                result.sCartVeichle = temp.sCartVeichle;
                result.sCartVeichleType = temp.sCartVeichleType;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}