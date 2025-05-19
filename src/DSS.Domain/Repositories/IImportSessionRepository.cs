using DSS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

public interface IImportSessionRepository : IBasicRepository<ImportSession, Guid>
{
    Task<ImportSession> GetByPoolId(Guid poolName);
}
