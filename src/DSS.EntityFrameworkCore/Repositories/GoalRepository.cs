using DSS.Entities;
using DSS.EntityFrameworkCore;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

public class GoalRepository : EfCoreRepository<DSSDbContext, Goal, Guid>, IGoalRepository
{
    public GoalRepository(IDbContextProvider<DSSDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
