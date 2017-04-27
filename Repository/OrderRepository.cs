using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;

namespace SystemWeb.Repository
{
    public static class OrderRepository
    {
        public static void Add(Carico value)
        {
            var context = new MyDbContext();
            context.Carico.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<Carico> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.Carico.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(Carico value)
        {
            var context = new MyDbContext();
            Carico result = context.Carico.Where(o => o.Id == value.Id).FirstOrDefault();
            if (result != null)
            {
                result.Id = value.Id;
                result.yearId = value.yearId;
                result.Ordine = value.Ordine;
                result.cData = value.cData;
                result.Documento = value.Documento;
                result.Numero = value.Numero;
                result.rData = value.rData;
                result.Emittente = value.Emittente;
                result.Benzina = value.Benzina;
                result.Gasolio = value.Gasolio;
                result.Note = value.Note;

                context.Entry(result).CurrentValues.SetValues(value);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void Update(List<Carico> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                Carico result = context.Carico.Where(o => o.Id == temp.Id).FirstOrDefault();
                if (result != null)
                {
                    result.Id = temp.Id;
                    result.yearId = temp.yearId;
                    result.Ordine = temp.Ordine;
                    result.cData = temp.cData;
                    result.Documento = temp.Documento;
                    result.Numero = temp.Numero;
                    result.rData = temp.rData;
                    result.Emittente = temp.Emittente;
                    result.Benzina = temp.Benzina;
                    result.Gasolio = temp.Gasolio;
                    result.Note = temp.Note;

                    context.Entry(result).CurrentValues.SetValues(temp);
                    context.Entry(result).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }
    }
}