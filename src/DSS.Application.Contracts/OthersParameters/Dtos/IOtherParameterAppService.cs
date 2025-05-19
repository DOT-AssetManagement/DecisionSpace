using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DSS.OthersParameters.Dtos
{
    public interface IOtherParameterAppService:ICrudAppService<TblOtherParameterDto,Guid,TblOtherParameterCreateDto,TblOtherParameterUpdateDto, GetOtherParameterInput>
    {
        Task<List<TblOtherParameterDto>> GetAllAsync();
        Task<TblOtherParameterDto> GetParameterAsync(string code);
        Task DeleteOtherParameterAsync(string code);
        Task<TblOtherParameterDto> UpdateAsync(string code, TblOtherParameterUpdateDto input);
        Task<TblOtherParameterDto> CreateParameterAsync(TblOtherParameterCreateDto input);
    }
}
