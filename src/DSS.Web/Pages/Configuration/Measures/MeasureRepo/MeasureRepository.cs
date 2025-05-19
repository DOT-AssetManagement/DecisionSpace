using DSS.Web.Pages.Configuration.Measures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

public class MeasureRepository : IMeasureRepository<Measure, Guid>
{
    private readonly IRepository<Measure, Guid> _measureRepository;

    public MeasureRepository(IRepository<Measure, Guid> measureRepository)
    {
        _measureRepository = measureRepository;
    }
 
    public async Task Create(Measure measure)
    {
        try
        {
            await _measureRepository.InsertAsync(measure);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    public async Task DeleteAsync(Guid id)
    {
        await _measureRepository.DeleteAsync(id);
    }

    public async Task<Measure> GetAsync(Guid id)
    {
        return await _measureRepository.GetAsync(id);
    }

    public async Task UpdateAsync(Measure MeasureEntityEdit)
    {
        await _measureRepository.UpdateAsync(MeasureEntityEdit);
    }
    public async Task<List<Measure>> GetAllAsync()
    {
        return await _measureRepository.GetListAsync();
    }

}
