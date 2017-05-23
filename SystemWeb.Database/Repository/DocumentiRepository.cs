using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SystemWeb.Database.Entity;

namespace SystemWeb.Database.Repository
{
    public static class DocumentiRepository
    {
        public static void Add(Documento value)
        {
            var context = new MyDbContext();
            context.Documento.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<Documento> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.Documento.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(Documento value)
        {
            var context = new MyDbContext();
            var result = context.Documento.FirstOrDefault(o => o.DocumentoID == value.DocumentoID);

            if (result == null) return;

            result.DocumentoID = value.DocumentoID;
            result.Tipo = value.Tipo;
            context.Entry(result).CurrentValues.SetValues(value);
            context.Entry(result).State = EntityState.Modified;
            context.SaveChanges();
        }

        public static void Update(List<Documento> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                var result = context.Documento.FirstOrDefault(o => o.DocumentoID == temp.DocumentoID);
                if (result == null) continue;

                result.DocumentoID = temp.DocumentoID;
                result.Tipo = temp.Tipo;

                context.Entry(result).CurrentValues.SetValues(temp);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}

