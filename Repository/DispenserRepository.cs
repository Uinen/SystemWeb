﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using SystemWeb.Models;

namespace SystemWeb.Repository
{
    public static class DispenserRepository
    {
        public static void Add(Dispenser value)
        {
            var context = new MyDbContext();

            if (value.isActive == true)
            {
                value.isActive = true;
            }
            else
            {
                value.isActive = false;
            }

            context.Dispenser.Add(value);
            context.SaveChanges();
        }

        public static void Add(List<Dispenser> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();

                if (temp.isActive == true)
                {
                    temp.isActive = true;
                }
                else
                {
                    temp.isActive = false;
                }

                context.Dispenser.Add(temp);
                context.SaveChanges();
            }
        }

        public static void Update(Dispenser value)
        {
            var context = new MyDbContext();
            Dispenser result = context.Dispenser.Where(o => o.DispenserId == value.DispenserId).FirstOrDefault();
            if (result != null)
            {
                result.DispenserId = value.DispenserId;
                result.PvTankId = value.PvTankId;
                result.Modello = value.Modello;
                result.isActive = value.isActive;
                
                context.Entry(result).CurrentValues.SetValues(value);
                context.Entry(result).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void Update(List<Dispenser> value)
        {
            foreach (var temp in value)
            {
                var context = new MyDbContext();
                Dispenser result = context.Dispenser.Where(o => o.DispenserId == temp.DispenserId).FirstOrDefault();
                if (result != null)
                {
                    result.DispenserId = temp.DispenserId;
                    result.PvTankId = temp.PvTankId;
                    result.Modello = temp.Modello;
                    result.isActive = temp.isActive;

                    context.Entry(result).CurrentValues.SetValues(temp);
                    context.Entry(result).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }
    }
}