using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Database.Entity;

namespace SystemWeb.Database.Repository
{
    public class OrderRepository
    {
        public static void Add(Carico value)
        {
            var _context = new MyDbContext();
            var thisYear = DateTime.Now.Year;
            var _selectThisYear = (from a in _context.Year
                                  where a.Anno.Year == thisYear
                                  select a.yearId).Single();

            value.yearId = _selectThisYear;
            _context.Carico.Add(value);
            _context.SaveChanges();
        }

        public static void Add(List<Carico> value)
        {
            foreach (var temp in value)
            {
                var _context = new MyDbContext();
                _context.Carico.Add(temp);
                _context.SaveChanges();
            }
        }

        public static void Update(Carico value)
        {
            var _context = new MyDbContext();
            var result = _context.Carico.Single(o => o.Id == value.Id);
            if (result == null) return;
            //result.Id = value.Id;
            //result.pvID = value.pvID;
            //result.yearId = value.yearId;
            result.Ordine = value.Ordine;
            result.cData = value.cData;
            result.documentoId = value.documentoId;
            result.Numero = value.Numero;
            result.rData = value.rData;
            result.Deposito = value.Deposito;
            result.Benzina = value.Benzina;
            result.Gasolio = value.Gasolio;
            //result.HiQb = value.HiQb;
            //result.HiQd = value.HiQd;
            result.Note = value.Note;

            _context.Entry(result).CurrentValues.SetValues(value);
            _context.Entry(result).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public static void Update(List<Carico> value)
        {
            foreach (var temp in value)
            {
                var _context = new MyDbContext();
                var result = _context.Carico.FirstOrDefault(o => o.Id == temp.Id);
                if (result == null) continue;
                result.Id = temp.Id;
                result.pvID = temp.pvID;
                result.yearId = temp.yearId;
                result.Ordine = temp.Ordine;
                result.cData = temp.cData;
                result.documentoId = temp.documentoId;
                result.Numero = temp.Numero;
                result.rData = temp.rData;
                result.Deposito = temp.Deposito;
                result.Benzina = temp.Benzina;
                result.Gasolio = temp.Gasolio;
                result.HiQb = temp.HiQb;
                result.HiQd = temp.HiQd;
                result.Note = temp.Note;

                _context.Entry(result).CurrentValues.SetValues(temp);
                _context.Entry(result).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}