using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository
{
    public static class ProductsRepository
    {
        private static readonly MyDbContext _context = new MyDbContext();
        public static void GetAll()
        {
            _context.Product.OrderBy(o => o.Nome).ToList();
        }
        public static void Add(Product value)
        {
            _context.Product.Add(value);
            _context.SaveChanges();
        }

        public static void Add(List<Product> value)
        {
            foreach (var temp in value)
            {
                _context.Product.Add(temp);
                _context.SaveChanges();
            }
        }

        public static void Update(Product value)
        {
            var result = _context.Product.FirstOrDefault(o => o.ProductId == value.ProductId);
            if (result == null) return;
            result.ProductId = value.ProductId;
            result.Nome = value.Nome;
            result.Peso = value.Peso;
            result.Prezzo = value.Prezzo;

            _context.Entry(result).CurrentValues.SetValues(value);
            _context.Entry(result).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public static void Update(List<Product> value)
        {
            foreach (var temp in value)
            {
                var result = _context.Product.FirstOrDefault(o => o.ProductId == temp.ProductId);
                if (result == null) continue;
                result.ProductId = temp.ProductId;
                result.Nome = temp.Nome;
                result.Peso = temp.Peso;
                result.Prezzo = temp.Prezzo;

                _context.Entry(result).CurrentValues.SetValues(temp);
                _context.Entry(result).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}