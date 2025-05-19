
using DSS.Pool;
using DSS.Pool.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Volo.Abp.Data;

namespace DSS.Web.Pages.Outputs.Reports
{
    public class WorkCandidateDetailModel : PageModel
    {
        private readonly string _connectionString = string.Empty;
        private readonly IPoolAppService _poolsappservice;
        public List<PoolDto> Pools { get; set; }

        public Guid? PoolId { get; set; }
        public string PoolName { get; set; }

        public string jsonResult { get; set; }


        public WorkCandidateDetailModel(IPoolAppService poolsappservice, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
            _poolsappservice = poolsappservice;
        }
        public async Task OnGetAsync(Guid? poolId, string searchString = "")
        {
            string viewQuery = "SELECT * FROM dbo.VwReportWorkCandidateDetail";
            DataTable dataTable = new DataTable();

            if (!string.IsNullOrEmpty(viewQuery))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand();
                    command.Connection = connection;
                    command.CommandText = viewQuery;
                    command.CommandType = CommandType.Text; 
                    command.CommandTimeout = 24000;

                    if (poolId != null)
                    {
                        command.CommandText += " WHERE PoolId = @PoolId";
                        command.Parameters.Add("@PoolId", SqlDbType.UniqueIdentifier).Value = poolId;
                    }

                    using (var dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }

            jsonResult = JsonConvert.SerializeObject(dataTable);

            if (poolId.HasValue)
            {
                var pool = await _poolsappservice.GetAsync(poolId.Value);
                PoolId = poolId.Value;
                PoolName = pool.Name;
            }
            

            Pools = await _poolsappservice.GetAllAsync("", "");
        }
    }
}
