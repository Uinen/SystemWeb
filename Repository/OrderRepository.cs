using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SystemWeb.Dto;
//using SystemWeb.Models;
using SystemWeb.Models.LinqToSql;

namespace SystemWeb.Repository
{
    public static class OrderRepository
    {
        public static IList<CaricoDto> GetAllRecords()
        {
            IList<CaricoDto> orders = (IList<CaricoDto>)HttpContext.Current.Session["Orders"];

            if (orders == null)
            {
                var context = new SystemWebDataContext();
                HttpContext.Current.Session["Orders"] = orders = (from ord in context.Carico
                                                                  join year in context.Year on ord.yearId equals year.yearId
                                                                  join pv in context.Pv on ord.pvID equals pv.pvID
                                                                  select new CaricoDto
                                                                  {
                                                                      Id = ord.Id,
                                                                      pvID = ord.pvID,
                                                                      pvName = pv.pvName,
                                                                      yearId = year.Anno.Year,
                                                                      Ordine = ord.Ordine,
                                                                      cData = ord.cData,
                                                                      Documento = ord.Documento,
                                                                      Numero = ord.Numero,
                                                                      rData = ord.rData,
                                                                      Emittente = ord.Emittente,
                                                                      Benzina = ord.Benzina,
                                                                      Gasolio = ord.Gasolio,
                                                                      Note = ord.Note
                                                                  }).ToList();
                
            }
            return orders;
        }

        public static void Add(CaricoDto order)
        {
            GetAllRecords().Add(order);
        }

        public static void Add(List<CaricoDto> order)
        {
            foreach (var temp in order)
                GetAllRecords().Add(temp);
        }

        public static void Delete(Guid Id)
        {
            CaricoDto result = GetAllRecords().Where(o => o.Id == Id).FirstOrDefault();
            GetAllRecords().Remove(result);
        }

        public static void Delete(List<CaricoDto> order)
        {
            foreach (var temp in order)
            {
                CaricoDto result = GetAllRecords().Where(o => o.Id == temp.Id).FirstOrDefault();
                GetAllRecords().Remove(result);
            }
        }

        public static void Update(CaricoDto order)
        {
            CaricoDto result = GetAllRecords().Where(o => o.Id == order.Id).FirstOrDefault();
            if (result != null)
            {
                result.Id = order.Id;
                result.pvName = order.pvName;
                result.yearId = order.yearId;
                result.Ordine = order.Ordine;
                result.cData = order.cData;
                result.Documento = order.Documento;
                result.Numero = order.Numero;
                result.rData = order.rData;
                result.Emittente = order.Emittente;
                result.Benzina = order.Benzina;
                result.Gasolio = order.Gasolio;
                result.Note = order.Note;
            }
        }

        public static void Update(List<CaricoDto> order)
        {
            foreach (var temp in order)
            {
                CaricoDto result = GetAllRecords().Where(o => o.Id == temp.Id).FirstOrDefault();
                if (result != null)
                {
                    result.Id = temp.Id;
                    result.pvName = temp.pvName;
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
                }
            }
        }
    }
}