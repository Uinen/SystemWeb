using GestioniDirette.Database.Entity;
using GestioniDirette.Database.Operation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace GestioniDirette.Database.Operation
{
    public class Operation : iOperation
    {
        #region Field
        private MyDbContext _db;
        private bool disposed = false;
        private UserInfo _userInfo = new UserInfo();
        public DateTime da { get; set; }
        public DateTime al { get; set; }
        public int lastYear { get; set; }
        public string Ly { get; set; }
        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }
        #endregion

        #region Costruttore
        public Operation(MyDbContext context)
        {
            this._db = context;
            da = new DateTime(2016, 12, 31);
            al = DateTime.Now;
            lastYear = DateTime.Today.Year;
            Ly = lastYear.ToString();
            dateFrom = new DateTime(2015, 12, 31);
            dateTo = DateTime.Now.AddYears(-1);
        }
        #endregion

        /// <summary>
        /// Questo metodo ritorna tutti i contatori a seconda del utente e della data impostata
        /// </summary
        /// <returns></returns>
        public IEnumerable<PvErogatori> GetErogatori()
        {
            #region Variabili
            string _pvID = _userInfo.GetPVID();
            #endregion

            var getPv = _db.PvErogatori
                .Include(i => i.Product)
                .Include(i => i.Dispenser)
                .Include(i => i.Pv)
                .Where(w => _pvID == w.pvID.ToString() && w.FieldDate >= da && w.FieldDate <= al);
            return getPv;
        }
        public IEnumerable<PvErogatori> GetErogatoriByParam(DateTime? dal, DateTime? a)
        {
            #region Variabili
            string _pvID = _userInfo.GetPVID();
            #endregion

            var getPv = _db.PvErogatori
                .Include(i => i.Product)
                .Include(i => i.Dispenser)
                .Include(i => i.Pv)
                .Where(w => _pvID == w.pvID.ToString() && w.FieldDate >= dal && w.FieldDate <= a);
            return getPv;
        }
        /// <summary>
        /// Questo metodo ritorna tutti i contatori dell'anno precedente a seconda del utente 
        /// </summary>
        /// <returns>getPv</returns>
        public IEnumerable<PvErogatori> GetErogatoriLastYear()
        {
            #region Variabili
            string _pvID = _userInfo.GetPVID();
            #endregion

            var getPv = _db.PvErogatori
                .Include(i => i.Product)
                .Include(i => i.Dispenser)
                .Include(i => i.Pv)
                .Where(w => _pvID == w.pvID.ToString() && w.FieldDate >= dateFrom && w.FieldDate <= dateTo);
            return getPv;
        }

        /// <summary>
        /// Questo metodo ritorna tutte le deficienze a seconda del utente e dell'anno corrente
        /// </summary>
        /// <returns>getDeficienze</returns>
        public IEnumerable<PvDeficienze> GetDeficienze()
        {
            #region Variabili
            string _pvID = _userInfo.GetPVID();
            #endregion

            var getDeficienze = _db.PvDeficienze
                .Include(i => i.PvTank)
                .Where(w => _pvID == w.PvTank.pvID.ToString() && w.FieldDate >= da && w.FieldDate <= al);
            return getDeficienze;
        }

        /// <summary>
        /// Questo metodo ritorna tutti i cali a seconda del utente e dell'anno corrente
        /// </summary>
        /// <returns>getCali</returns>
        public IEnumerable<PvCali> GetCali()
        {
            #region Variabili
            string _pvID = _userInfo.GetPVID();
            #endregion

            var getCali = _db.PvCali
                .Include(i => i.PvTank)
                .Where(w => _pvID == w.PvTank.pvID.ToString() && w.FieldDate >= da && w.FieldDate <= al);
            return getCali;
        }

        /// <summary>
        /// Questo metodo ritorna tutti gli ordini a seconda del utente e dell'anno corrente
        /// </summary>
        /// <returns>getCarico</returns>
        public IEnumerable<Carico> GetCarico()
        {
            #region Variabili
            string _pvID = _userInfo.GetPVID();
            #endregion

            var getCarico = _db.Carico.Where(w => _pvID == w.pvID.ToString() && w.Year.Anno.ToString().Contains(Ly));
            return getCarico;
        }

        /// <summary>
        /// Questo metodo ritorna la Licenza del punto vendita corrente
        /// </summary>
        /// <returns>getLicenza</returns>
        public IEnumerable<Licenza> GetLicenza()
        {
            #region Variabili
            string _pvID = _userInfo.GetPVID();
            #endregion

            var getLicenza = _db.Licenza.Where(w => _pvID == w.pvID.ToString());
            return getLicenza;
        }

        /// <summary>
        /// Questo metodo ritorna le cisterne del punto vendita corrente
        /// </summary>
        /// <returns>getTank</returns>
        public IEnumerable<PvTank> GetTank()
        {
            #region Variabili
            string _pvID = _userInfo.GetPVID();
            #endregion

            var getTank = _db.PvTank
                .Include(i => i.Product)
                .Where(w => _pvID == w.pvID.ToString());
            return getTank;
        }

        /// <summary>
        /// Questo metodo ritorna i dati del profilo punto vendita corrente
        /// </summary>
        /// <returns>getPvProfile</returns>
        public IEnumerable<PvProfile> GetPvProfile()
        {
            #region Variabili
            string _pvID = _userInfo.GetPVID();
            #endregion

            var getPvProfile = _db.PvProfile.Where(w => _pvID == w.pvID.ToString());
            return getPvProfile;
        }

        /// <summary>
        /// Questo metodo calcola la differenza tra massimo e minimo per la Benzina. Secondo la formula Venduto = Contatore Massimo - Contatore Minimo
        /// </summary>
        /// <returns>totalB</returns>
        public int? DoSSPBOperation()
        {
            #region Benzina
            var max1B = GetErogatori()
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value);

            var min1B = GetErogatori()
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value);

            var max2B = GetErogatori()
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value1);

            var min2B = GetErogatori()
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value1);

            var max3B = GetErogatori()
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value2);

            var min3B = GetErogatori()
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value2);

            var max4B = GetErogatori()
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value3);

            var min4B = GetErogatori()
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value3);

            #endregion
            
            #region Variabili

            var pist1B = max1B - min1B;
            var pist2B = max2B - min2B;
            var pist3B = max3B - min3B;
            var pist4B = max4B - min4B;
            var totalB = (pist1B + pist2B + pist3B + pist4B);

            #endregion

            return totalB;
        }

        /// <summary>
        /// Questo metodo calcola la differenza tra massimo e minimo per il Diesel. Secondo la formula Venduto = Contatore Massimo - Contatore Minimo
        /// </summary>
        /// <returns>totalG</returns>
        public int? DoDSLOperation()
        {
            #region Diesel
            var max1G = GetErogatori()
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value);

            var min1G = GetErogatori()
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value);

            var max2G = GetErogatori()
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value1);

            var min2G = GetErogatori()
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value1);

            var max3G = GetErogatori()
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value2);

            var min3G = GetErogatori()
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value2);

            var max4G = GetErogatori()
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value3);

            var min4G = GetErogatori()
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value3);

            var max5G = GetErogatori()
                .Where(z => z.Value4 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value4);

            var min5G = GetErogatori()
                .Where(z => z.Value4 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value4);

            var max6G = GetErogatori()
                .Where(z => z.Value5 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value5);

            var min6G = GetErogatori()
                .Where(z => z.Value5 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value5);

            var max7G = GetErogatori()
                .Where(z => z.Value6 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value6);

            var min7G = GetErogatori()
                .Where(z => z.Value6 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value6);

            var max8G = GetErogatori()
                .Where(z => z.Value7 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value7);

            var min8G = GetErogatori()
                .Where(z => z.Value7 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value7);
            #endregion

            #region Variabili

            var pist1G = max1G - min1G;
            var pist2G = max2G - min2G;
            var pist3G = max3G - min3G;
            var pist4G = max4G - min4G;
            var pist5G = max5G - min5G;
            var pist6G = max6G - min6G;
            var pist7G = max7G - min7G;
            var pist8G = max8G - min8G;
            var totalG = (pist1G + pist2G + pist3G + pist4G + pist5G + pist6G + pist7G + pist8G);

            #endregion

            return totalG;
        }
        /// <summary>
        /// Questo metodo calcola la differenza tra massimo e minimo per la Benzina. Secondo la formula Venduto = Contatore Massimo - Contatore Minimo tenendo conto della data passata come parametro dal controller, al metodo repository GetErogatoryByParam ed infine al operazione numerica
        /// </summary>
        /// <returns>totalB</returns>
        public int? DoSSPBOperationByParam(DateTime? dal, DateTime? a)
        {
            #region Benzina
            var max1B = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value);

            var min1B = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value);

            var max2B = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value1);

            var min2B = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value1);

            var max3B = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value2);

            var min3B = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value2);

            var max4B = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value3);

            var min4B = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value3);

            #endregion

            #region Variabili

            var pist1B = max1B - min1B;
            var pist2B = max2B - min2B;
            var pist3B = max3B - min3B;
            var pist4B = max4B - min4B;
            var totalB = (pist1B + pist2B + pist3B + pist4B);

            #endregion

            return totalB;
        }

        /// <summary>
        /// Questo metodo calcola la differenza tra massimo e minimo per il Diesel. Secondo la formula Venduto = Contatore Massimo - Contatore Minimo tenendo conto della data passata come parametro dal controller, al metodo repository GetErogatoryByParam ed infine al operazione numerica
        /// </summary>
        /// <returns>totalG</returns>
        public int? DoDSLOperationByParam(DateTime? dal, DateTime? a)
        {
            #region Diesel
            var max1G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value);

            var min1G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value);

            var max2G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value1);

            var min2G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value1);

            var max3G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value2);

            var min3G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value2);

            var max4G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value3);

            var min4G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value3);

            var max5G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value4 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value4);

            var min5G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value4 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value4);

            var max6G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value5 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value5);

            var min6G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value5 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value5);

            var max7G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value6 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value6);

            var min7G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value6 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value6);

            var max8G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value7 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value7);

            var min8G = GetErogatoriByParam(dal: dal, a: a)
                .Where(z => z.Value7 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value7);
            #endregion

            #region Variabili

            var pist1G = max1G - min1G;
            var pist2G = max2G - min2G;
            var pist3G = max3G - min3G;
            var pist4G = max4G - min4G;
            var pist5G = max5G - min5G;
            var pist6G = max6G - min6G;
            var pist7G = max7G - min7G;
            var pist8G = max8G - min8G;
            var totalG = (pist1G + pist2G + pist3G + pist4G + pist5G + pist6G + pist7G + pist8G);

            #endregion

            return totalG;
        }
        /// <summary>
        /// Questo metodo calcola la differenza tra massimo e minimo per la Benzina nei punti vendita con meno di 2 erogatori. Secondo la formula Venduto = Contatore Massimo - Contatore Minimo
        /// </summary>
        /// <returns>totalB</returns>
        public int? DoSSPBOperationShort()
        {
            #region Benzina

            var max1B = GetErogatori()
                .Where(z => z.Value > 0 && (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value);

            var min1B = GetErogatori()
                .Where(z => z.Value > 0 && (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value);

            var max2B = GetErogatori()
                .Where(z => z.Value1 > 0 && (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value1);

            var min2B = GetErogatori()
                .Where(z => z.Value1 > 0 && (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value1);

            var max3B = GetErogatori()
                .Where(z => z.Value2 > 0 && (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value2);

            var min3B = GetErogatori()
                .Where(z => z.Value2 > 0 && (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value2);

            var max4B = GetErogatori()
                .Where(z => z.Value3 > 0 && (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value3);

            var min4B = GetErogatori()
                .Where(z => z.Value3 > 0 && (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value3);

            #endregion

            #region Variabili

            var pist1 = max1B - min1B;
            var pist2 = max2B - min2B;
            var pist3 = max3B - min3B;
            var pist4 = max4B - min4B;

            var totalB = pist1 + pist2 + pist3 + pist4;

            #endregion


            return totalB;
        }

        /// <summary>
        /// Questo metodo calcola la differenza tra massimo e minimo per il Diesel nei punti vendita con meno di 2 erogatori. Secondo la formula Venduto = Contatore Massimo - Contatore Minimo
        /// </summary>
        /// <returns>totalG</returns>
        public int? DoDSLOperationShort()
        {
            #region Diesel
            var max1G = GetErogatori()
                .Where(z => z.Value > 0 && (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value);

            var min1G = GetErogatori()
                .Where(z => z.Value > 0 && (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value);

            var max2G = GetErogatori()
                .Where(z => z.Value1 > 0 && (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value1);

            var min2G = GetErogatori()
                .Where(z => z.Value1 > 0 && (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value1);

            var max3G = GetErogatori()
                .Where(z => z.Value2 > 0 && (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value2);

            var min3G = GetErogatori()
                .Where(z => z.Value2 > 0 && (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value2);

            #endregion

            #region Variabili

            var pist1 = max1G - min1G;
            var pist2 = max2G - min2G;
            var pist3 = max3G - min3G;

            var totalG = pist1 + pist2 + pist3;

            #endregion

            return totalG;
        }

        /// <summary>
        /// Questo metodo calcola la differenza tra massimo e minimo per la Benzina nell'anno precedente. Secondo la formula Venduto = Contatore Massimo - Contatore Minimo
        /// </summary>
        /// <returns>totalB</returns>
        public int? DoSSPBOperationForLastYear()
        {
            #region Benzina
            var max1B = GetErogatoriLastYear()
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value);

            var min1B = GetErogatoriLastYear()
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value);

            var max2B = GetErogatoriLastYear()
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value1);

            var min2B = GetErogatoriLastYear()
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value1);

            var max3B = GetErogatoriLastYear()
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value2);

            var min3B = GetErogatoriLastYear()
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value2);

            var max4B = GetErogatoriLastYear()
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value3);

            var min4B = GetErogatoriLastYear()
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value3);

            #endregion

            #region Variabili

            var pist1B = max1B - min1B;
            var pist2B = max2B - min2B;
            var pist3B = max3B - min3B;
            var pist4B = max4B - min4B;
            var totalB = (pist1B + pist2B + pist3B + pist4B);

            #endregion

            return totalB;
        }

        /// <summary>
        /// Questo metodo calcola la differenza tra massimo e minimo per il Diesel nell'anno precedente. Secondo la formula Venduto = Contatore Massimo - Contatore Minimo
        /// </summary>
        /// <returns>totalG</returns>
        public int? DoDSLOperationForLastYear()
        {
            #region Diesel
            var max1G = GetErogatoriLastYear()
                        .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                        .Max(s => s.Value);

            var min1G = GetErogatoriLastYear()
                .Where(z => z.Value > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value);

            var max2G = GetErogatoriLastYear()
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value1);

            var min2G = GetErogatoriLastYear()
                .Where(z => z.Value1 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value1);

            var max3G = GetErogatoriLastYear()
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value2);

            var min3G = GetErogatoriLastYear()
                .Where(z => z.Value2 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value2);

            var max4G = GetErogatoriLastYear()
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value3);

            var min4G = GetErogatoriLastYear()
                .Where(z => z.Value3 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value3);

            var max5G = GetErogatoriLastYear()
                .Where(z => z.Value4 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value4);

            var min5G = GetErogatoriLastYear()
                .Where(z => z.Value4 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value4);

            var max6G = GetErogatoriLastYear()
                .Where(z => z.Value5 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value5);

            var min6G = GetErogatoriLastYear()
                .Where(z => z.Value5 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value5);

            var max7G = GetErogatoriLastYear()
                .Where(z => z.Value6 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value6);

            var min7G = GetErogatoriLastYear()
                .Where(z => z.Value6 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value6);

            var max8G = GetErogatoriLastYear()
                .Where(z => z.Value7 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value7);

            var min8G = GetErogatoriLastYear()
                .Where(z => z.Value7 > 0 & (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value7);
            #endregion

            #region Variabili

            var pist1G = max1G - min1G;
            var pist2G = max2G - min2G;
            var pist3G = max3G - min3G;
            var pist4G = max4G - min4G;
            var pist5G = max5G - min5G;
            var pist6G = max6G - min6G;
            var pist7G = max7G - min7G;
            var pist8G = max8G - min8G;
            var totalG = (pist1G + pist2G + pist3G + pist4G + pist5G + pist6G + pist7G + pist8G);

            #endregion

            return totalG;
        }

        /// <summary>
        /// Questo metodo calcola la differenza tra massimo e minimo per la Benzina nell'anno precedente nei punti vendita con meno di 2 erogatori. Secondo la formula Venduto = Contatore Massimo - Contatore Minimo
        /// </summary>
        /// <returns>totalB</returns>
        public int? DoSSPBOperationForLastYearShort()
        {
            #region Benzina
            var max1B = GetErogatoriLastYear()
                .Where(z => (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Max(s => s.Value);

            var min1B = GetErogatoriLastYear()
                .Where(z => (z.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785")))
                .Min(s => s.Value);
            #endregion

            #region Variabili
            var totalB = max1B - min1B;
            #endregion

            return totalB;
        }

        /// <summary>
        /// Questo metodo calcola la differenza tra massimo e minimo per il Diesel nell'anno precedente nei punti vendita con meno di 2 erogatori. Secondo la formula Venduto = Contatore Massimo - Contatore Minimo
        /// </summary>
        /// <returns>totalG</returns>
        public int? DoDSLOperationForLastYearShort()
        {
            #region Diesel
            var max1G = GetErogatoriLastYear()
                .Where(z => (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Max(s => s.Value);

            var min1G = GetErogatoriLastYear()
                .Where(z => (z.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0")))
                .Min(s => s.Value);
            #endregion

            #region Variabili
            var totalG = max1G - min1G;
            #endregion

            return totalG;
        }

        /// <summary>
        /// Questo metodo somma le deficienze per la Benzina a seconda del prodotto e del punto vendita corrente
        /// </summary>
        /// <returns>sumdB</returns>
        public int DoSSPBDeficienze()
        {
            var sumdB = GetDeficienze()
                        .Where(z => z.PvTank.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785"))
                        .Sum(row => row.Value);
            return sumdB;
        }

        /// <summary>
        /// Questo metodo somma le deficienze per il Diesel a seconda del prodotto e del punto vendita corrente
        /// </summary>
        /// <returns>sumdG</returns>
        public int DoDSLDeficienze()
        {
            var sumdG = GetDeficienze()
                        .Where(z => z.PvTank.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0"))
                        .Sum(row => row.Value);
            return sumdG;
        }

        /// <summary>
        /// Questo metodo somma i cali per la Benzina a seconda del prodotto e del punto vendita corrente
        /// </summary>
        /// <returns>sumcB</returns>
        public int DoSSPBCali()
        {
            var sumcB = GetCali()
                        .Where(z => z.PvTank.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785"))
                        .Sum(row => row.Value);
            return sumcB;
        }

        /// <summary>
        /// Questo metodo somma i cali per il Diesel a seconda del prodotto e del punto vendita corrente
        /// </summary>
        /// <returns>sumcD</returns>
        public int DoDSLCali()
        {
            var sumcG = GetCali()
                        .Where(z => z.PvTank.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0"))
                        .Sum(row => row.Value);
            return sumcG;
        }

        public double DoSSPBTankPercentageById()
        {
            #region Count
            var countTank = GetTank()
                .Where(w => w.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785"))
                .Count();

            double divide = (100 / countTank);
            #endregion

            var percentage = GetTank()
                .Where(w => w.Product.ProductId.ToString().Contains("e906a6fa-c5d7-4850-9b8e-3e1b5a342785"))
                .Select(s => (((double)s.Giacenza) / s.Capienza) * divide).Sum();

            return percentage;
        }
        public double DoDSLTankPercentageById()
        {
            #region Count
            var countTank = GetTank()
                .Where(w => w.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0"))
                .Count();

            double divide = (100 / countTank);
            #endregion

            var percentage = GetTank()
                .Where(w => w.Product.ProductId.ToString().Contains("0ac61d1f-db50-4781-b147-d43325718dc0"))
                .Select(s => (((double)s.Giacenza) / s.Capienza) * divide).Sum();

            return percentage;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
