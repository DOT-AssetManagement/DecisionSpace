using DSS.Entities;
using DSS.ImportSessions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Users;

namespace DSS.ImportSessions
{
    public class ImportSessionAppService : ApplicationService, IImportSessionAppService
    {
        private readonly ICurrentUser _curruntUser;
        private readonly IImportSessionRepository _importSessionRepository;

        public ImportSessionAppService(ICurrentUser curruntUser, IImportSessionRepository importSessionRepository)
        {
            _curruntUser = curruntUser;
            _importSessionRepository = importSessionRepository;
        }

        public Task<ImportSessionDto> CreateAsync(ImportSessionDto input)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ImportSessionDto> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ImportSessionDto> GetByPoolId(Guid poolId)
        {
            var response = await _importSessionRepository.GetByPoolId(poolId);
            return ObjectMapper.Map<ImportSession, ImportSessionDto>(response);
        }
        public Task<PagedResultDto<ImportSessionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            throw new NotImplementedException();
        }

        public Task<ImportSessionDto> UpdateAsync(Guid id, ImportSessionDto input)
        {
            throw new NotImplementedException();
        }
    }
}
