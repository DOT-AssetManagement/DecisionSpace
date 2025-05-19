using DSS.ImportSessions.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DSS.ImportSessions
{
    public interface IImportSessionAppService : ICrudAppService<ImportSessionDto, Guid>
    {
        Task<ImportSessionDto> GetByPoolId(Guid poolId);
    }
}
