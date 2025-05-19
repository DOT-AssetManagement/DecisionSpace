using DSS.Entities;
using DSS.Goals.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DSS.Goals
{
    public class GoalAppService : ApplicationService, IGoalAppService
    {
        private readonly IGoalRepository _goalrepository;

        public GoalAppService(IGoalRepository goalrepository)
        {
            _goalrepository = goalrepository;
        }

        public async Task<GoalDto> CreateAsync(GoalCreateDto input)
        {
            var goal = ObjectMapper.Map<GoalCreateDto, Goal>(input);
            var response = await _goalrepository.InsertAsync(goal);
            return ObjectMapper.Map<Goal, GoalDto>(response);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _goalrepository.DeleteAsync(id);
        }

        [HttpGet("api/app/goal/getAll")]
        public async Task<List<GoalDto>> GetAllAsync()
        {
            var goals = await _goalrepository.GetListAsync();
            return ObjectMapper.Map<List<Goal>, List<GoalDto>>(goals);
        }

        public async Task<GoalDto> GetAsync(Guid id)
        {
            var goal = await _goalrepository.GetAsync(id);
            return ObjectMapper.Map<Goal, GoalDto>(goal);
        }

        public async Task<PagedResultDto<GoalDto>> GetListAsync(GetGoalInput input)
        {
            var goals = await _goalrepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);
            var goalsDtos = ObjectMapper.Map<List<Goal>, List<GoalDto>>(goals);

            return new PagedResultDto<GoalDto>(goalsDtos.Count, goalsDtos);

        }

        public async Task<GoalDto> UpdateAsync(Guid id, GoalUpdateDto input)
        {
            var goal = await _goalrepository.GetAsync(id);
            var updatedGoal = ObjectMapper.Map(input, goal);
            var response = await _goalrepository.UpdateAsync(updatedGoal);

            return ObjectMapper.Map<Goal, GoalDto>(response);
        }
    }
}
