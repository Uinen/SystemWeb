﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;

namespace SystemWeb.Repository
{
    public static class DeficienzeRepository
    {
        public static void Add(PvDeficienze value)
        {
            var context = new MyDbContext();
            context.PvDeficienze.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<PvDeficienze> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                context.PvDeficienze.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(PvDeficienze value)
        {
            var context = new MyDbContext();
            PvDeficienze result = context.PvDeficienze.Where(o => o.PvDefId == value.PvDefId).FirstOrDefault();
            if (result != null)
            {
                result.PvDefId = value.PvDefId;
                result.PvTankId = value.PvTankId;
                result.Value = value.Value;
                result.FieldDate = value.FieldDate;

                context.Entry(result).CurrentValues.SetValues(value);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void Update(List<PvDeficienze> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                PvDeficienze result = context.PvDeficienze.Where(o => o.PvDefId == temp.PvDefId).FirstOrDefault();
                if (result != null)
                {
                    result.PvDefId = temp.PvDefId;
                    result.PvTankId = temp.PvTankId;
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