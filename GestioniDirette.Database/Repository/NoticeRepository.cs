using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GestioniDirette.Database.Entity;

namespace GestioniDirette.Database.Repository
{
    public static class NoticeRepository
    {
        private static readonly MyDbContext _context = new MyDbContext();
        public static void GetAll()
        {
            _context.Notice.OrderBy(o => o.CreateDate).ToList();
        }
        public static void Add(Notice value)
        {
            _context.Notice.Add(value);
            _context.SaveChanges();
        }

        public static void Add(List<Notice> value)
        {
            foreach (var temp in value)
            {
                _context.Notice.Add(temp);
                _context.SaveChanges();
            }
        }

        public static void Update(Notice value)
        {
            var result = _context.Notice.FirstOrDefault(o => o.NoticeId == value.NoticeId);
            if (result == null) return;
            result.NoticeName = value.NoticeName;
            result.TextBox = value.TextBox;
            result.Description = value.Description;
            result.UsersId = value.UsersId;
            result.CreateDate = value.CreateDate;

            _context.Entry(result).CurrentValues.SetValues(value);
            _context.Entry(result).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public static void Update(List<Notice> value)
        {
            foreach (var temp in value)
            {
                var result = _context.Notice.FirstOrDefault(o => o.NoticeId == temp.NoticeId);
                if (result == null) continue;
                result.NoticeName = temp.NoticeName;
                result.TextBox = temp.TextBox;
                result.Description = temp.Description;
                result.UsersId = temp.UsersId;
                result.CreateDate = temp.CreateDate;

                _context.Entry(result).CurrentValues.SetValues(temp);
                _context.Entry(result).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}
