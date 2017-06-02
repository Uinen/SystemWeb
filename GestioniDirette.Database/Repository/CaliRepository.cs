using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository
{
    public static class CaliRepository
    {
        private static readonly MyDbContext _context = new MyDbContext();
        public static void Add(PvCali value)
        {
            _context.PvCali.Add(value);
            _context.SaveChanges();
        }

        public static void Add(List<PvCali> value)
        {
            foreach (var temp in value)
            {
                _context.PvCali.Add(temp);
                _context.SaveChanges();
            }
        }

        public static void Update(PvCali value)
        {
            var result = _context.PvCali.FirstOrDefault(o => o.PvCaliId == value.PvCaliId);
            if (result == null) return;
            result.PvCaliId = value.PvCaliId;
            result.PvTankId = value.PvTankId;
            result.Value = value.Value;
            result.FieldDate = value.FieldDate;

            _context.Entry(result).CurrentValues.SetValues(value);
            _context.Entry(result).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public static void Update(List<PvCali> value)
        {
            foreach (var temp in value)
            {
                var result = _context.PvCali.FirstOrDefault(o => o.PvCaliId == temp.PvCaliId);
                if (result == null) continue;
                result.PvCaliId = temp.PvCaliId;
                result.PvTankId = temp.PvTankId;
                result.Value = temp.Value;
                result.FieldDate = temp.FieldDate;

                _context.Entry(result).CurrentValues.SetValues(temp);
                _context.Entry(result).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}