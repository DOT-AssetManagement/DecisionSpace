using DSS.Goals.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.TenantManagement;

namespace DSS.Goals
{
    public interface IGoalAppService: ICrudAppService<GoalDto, Guid, GetGoalInput, GoalCreateDto, GoalUpdateDto>
    {
        Task<List<GoalDto>> GetAllAsync();
    }
}
