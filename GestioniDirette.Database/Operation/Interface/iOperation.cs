using GestioniDirette.Database.Entity;
using System;
using System.Collections.Generic;

namespace GestioniDirette.Database.Operation.Interface
{
    public interface iOperation : IDisposable
    {
        IEnumerable<PvErogatori> GetErogatori();
        IEnumerable<PvErogatori> GetErogatoriByParam(DateTime? dal, DateTime? a);
        IEnumerable<PvErogatori> GetErogatoriLastYear();
        IEnumerable<PvDeficienze> GetDeficienze();
        IEnumerable<PvCali> GetCali();
        IEnumerable<Carico> GetCarico();
        IEnumerable<Licenza> GetLicenza();
        IEnumerable<PvTank> GetTank();
        IEnumerable<PvProfile> GetPvProfile();
        int? DoSSPBOperation();
        int? DoDSLOperation();
        int? DoSSPBOperationByParam(DateTime? dal, DateTime? a);
        int? DoDSLOperationByParam(DateTime? dal, DateTime? a);
        int? DoSSPBOperationShort();
        int? DoDSLOperationShort();
        int? DoSSPBOperationForLastYear();
        int? DoDSLOperationForLastYear();
        int? DoSSPBOperationForLastYearShort();
        int? DoDSLOperationForLastYearShort();
        int DoSSPBDeficienze();
        int DoDSLDeficienze();
        int DoSSPBCali();
        int DoDSLCali();
        decimal DoSSPBTankPercentageById();
    }
}
