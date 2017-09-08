using GestioniDirette.Database.Entity;
using System;
using System.Collections.Generic;

namespace GestioniDirette.Database.Operation.Interface
{
    public interface iOperation : IDisposable
    {
        IEnumerable<PvErogatori> GetErogatori();
        IEnumerable<PvErogatori> GetErogatoriLastYear();
        IEnumerable<PvDeficienze> GetDeficienze();
        IEnumerable<PvCali> GetCali();
        IEnumerable<Carico> GetCarico();
        IEnumerable<Licenza> GetLicenza();
        IEnumerable<PvTank> GetTank();
        IEnumerable<PvProfile> GetPvProfile();
        int? DoSSPBOperation();
        int? DoDSLOperation();
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
    }
}
