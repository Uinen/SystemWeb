using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;

namespace SystemWeb.Repository
{
    public static class ProductsRepository
    {
        public static void Add(Product value)
        {
            var context = new MyDbContext();
            context.Product.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<Product> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.Product.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(Product value)
        {
            var context = new MyDbContext();
            var result = context.Product.FirstOrDefault(o => o.ProductId == value.ProductId);
            if (result == null) return;
            result.ProductId = value.ProductId;
            result.Nome = value.Nome;
            result.Peso = value.Peso;
            result.Prezzo = value.Prezzo;

            context.Entry(result).CurrentValues.SetValues(value);
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
        }

        public static void Update(List<Product> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                var result = context.Product.FirstOrDefault(o => o.ProductId == temp.ProductId);
                if (result == null) continue;
                result.ProductId = temp.ProductId;
                result.Nome = temp.Nome;
                result.Peso = temp.Peso;
                result.Prezzo = temp.Prezzo;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}