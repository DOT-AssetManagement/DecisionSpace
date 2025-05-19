using Castle.Core.Configuration;
using DSS.Counties;
using DSS.Counties.Dtos;
using DSS.Entities;
using DSS.ProjectCandidates;
using DSS.ProjectCandidates.Dtos;
using DSS.WorkCandidates;
using DSS.WorkCandidates.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DSS.ImportSessions;
using Volo.Abp.Users;
using Microsoft.IdentityModel.Tokens;

namespace DSS.Web.Pages.WorkCandidates
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IProjectCandidateAppService _projectCandidateAppService;
        private readonly IWorkCandidatesAppService _workCandidatesAppService;
        private readonly IImportSessionAppService _importSessionsAppService;
        private readonly ICountyAppService _countyAppService;
        private readonly string _connectionString = string.Empty;
        private readonly ICurrentUser _currentUser;

        public WorkCandidatesDto WorkCandidates { get; set; }
        public string PoolId { get; set; }
        public Guid? WorkCandidateId { get; set; }

        public List<CountyDto> Districts { get; set; }
        public List<DistrictModel> DistrictList { get; set; }
        public List<CountyModel> CountyList { get; set; }
        public List<RouteModel> RouteList { get; set; }
        public List<SectionModel> SectionList { get; set; }
        public List<BPNModel> BPNList { get; set; }
        public List<BRKEYModel> BRKEYList { get; set; }
        public List<WorkTypeModal> WorkTypeList { get; set; }
        
        public List<CountyDto> Counties { get; set; }
        public List<String> ProjectNames { get; set; }

        public CreateModel(IWorkCandidatesAppService workCandidatesAppService, ICountyAppService countyAppService, Microsoft.Extensions.Configuration.IConfiguration configuration, IProjectCandidateAppService projectCandidateAppService, IImportSessionAppService importSessionsAppService, ICurrentUser currentUser)
        {
            _projectCandidateAppService = projectCandidateAppService;
            _workCandidatesAppService = workCandidatesAppService;
            _importSessionsAppService = importSessionsAppService;
            _countyAppService = countyAppService;
            _connectionString = configuration.GetConnectionString("Default");
            _currentUser = currentUser;
        }
        public async Task OnGet(Guid id, Guid? candidateId)
        {
            if (candidateId != null)
            {
                WorkCandidates = await _workCandidatesAppService.GetAsync(candidateId.Value);
                WorkCandidateId = candidateId;
            }
            else
            {
                WorkCandidates = new WorkCandidatesDto();
            }
            Districts = await _countyAppService.GetAllDistricts();
            PoolId = id.ToString();

            string storedProcedure = "dbo.UspUIGetDistricts";
            DataTable dataTable = new DataTable();

            if (!string.IsNullOrEmpty(storedProcedure))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = storedProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 24000;

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }

            DistrictList = new List<DistrictModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                DistrictList.Add(new DistrictModel
                {
                    DistrictId = Convert.ToInt32(row["District"]),
                    DistrictName = row["Descr"].ToString(),
                });
            }

            ProjectNames = await _projectCandidateAppService.GetDistinctDescriptions();


            storedProcedure = "dbo.UspUIGetWorkTypes";
            dataTable = new DataTable();            
            if (!string.IsNullOrEmpty(storedProcedure))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = storedProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 24000;

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }

            WorkTypeList = new List<WorkTypeModal>();
            foreach (DataRow row in dataTable.Rows)
            {
                WorkTypeList.Add(new WorkTypeModal
                {
                    WorkType = row["WorkType"].ToString(),
                });
            }
        }

        public async Task<IActionResult> OnGetGetCounties(int id)
        {


            string storedProcedure = "dbo.UspUIGetCounties";
            DataTable dataTable = new DataTable();

            if (!string.IsNullOrEmpty(storedProcedure))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = storedProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 24000;
                        if (id != null)
                        {
                            command.Parameters.Add("@District", SqlDbType.Int).Value = id;
                        }

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            CountyList = new List<CountyModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                CountyList.Add(new CountyModel
                {
                    cnty = Convert.ToInt32(row["Cnty"]),
                    countyName = row["County"].ToString(),
                });
            }
            



            Counties = await _countyAppService.GetCountiesByDistrict(id);
            return new JsonResult(CountyList);
        }

        public async Task<IActionResult> OnGetGetRoutes(int Countyid, int Districtid)
        {


            string storedProcedure = "dbo.UspUIGetRoutes";
            DataTable dataTable = new DataTable();

            if (!string.IsNullOrEmpty(storedProcedure))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = storedProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 24000;
                        if (Countyid != null)
                        {
                            command.Parameters.Add("@Cnty", SqlDbType.Int).Value = Countyid;
                        }
                        if (Districtid != null)
                        {
                            command.Parameters.Add("@District", SqlDbType.Int).Value = Districtid;
                        }


                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            RouteList = new List<RouteModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                RouteList.Add(new RouteModel
                {
                    Rte = row["Rte"].ToString(),
                });
            }

            return new JsonResult(RouteList);
        }

        public async Task<IActionResult> OnGetGetSectionPairs(int Countyid, int Districtid, int Route)
        {


            string storedProcedure = "dbo.UspUIGetSectionPairs";
            DataTable dataTable = new DataTable();

            if (!string.IsNullOrEmpty(storedProcedure))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = storedProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 24000;
                        if (Countyid != null)
                        {
                            command.Parameters.Add("@Cnty", SqlDbType.Int).Value = Countyid;
                        }
                        if (Districtid != null)
                        {
                            command.Parameters.Add("@District", SqlDbType.Int).Value = Districtid;
                        }
                        if (Route != null)
                        {
                            command.Parameters.Add("@Route", SqlDbType.Int).Value = Route;
                        }
                        command.Parameters.Add("@Direction", SqlDbType.Bit).Value = 1;

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            SectionList = new List<SectionModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                SectionList.Add(new SectionModel
                {
                    FromSection = row["FromSection"].ToString(),
                    ToSection = row["ToSection"].ToString(),
                    FromTo = row["FromTo"].ToString(),
                });
            }

            return new JsonResult(SectionList);
        }

        public async Task<IActionResult> OnGetGetBrkeyAndBMSID(int Countyid, int Districtid, int Route)
        {


            string storedProcedure = "dbo.UspUIGetBrkeyAndBMSID";
            DataTable dataTable = new DataTable();

            if (!string.IsNullOrEmpty(storedProcedure))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = storedProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 24000;
                        if (Countyid != null)
                        {
                            command.Parameters.Add("@Cnty", SqlDbType.Int).Value = Countyid;
                        }
                        if (Districtid != null)
                        {
                            command.Parameters.Add("@District", SqlDbType.Int).Value = Districtid;
                        }
                        if (Route != null)
                        {
                            command.Parameters.Add("@Route", SqlDbType.Int).Value = Route;
                        }

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            BRKEYList = new List<BRKEYModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                BRKEYList.Add(new BRKEYModel
                {
                    BRKEY = row["BRKEY"].ToString(),
                    BMSID = row["BMSID"].ToString(),
                });
            }

            return new JsonResult(BRKEYList);
        }


        


        public async Task<IActionResult> OnPostCreateWorkCandidateAsync(Guid PoolId, WorkCandidatesCreateDto input)
        {

            //var response = await _importSessionsAppService.GetByPoolId(PoolId);
            input.PoolId = PoolId;

            if (!string.IsNullOrEmpty(input.ToSection.ToString()))
            {
                var sections = input.ToSection.Split('-');
                if (sections.Length == 2)
                {
                    if (int.TryParse(sections[0], out int fromSection))
                    {
                        input.FromSection = fromSection;
                    }

                    if (int.TryParse(sections[1], out int toSection))
                    {
                        input.ToSection = toSection.ToString();
                    }
                }
            }


            var User = _currentUser.UserName;
            input.CreatedBy = User;
            input.CreatedAt = DateTime.Now;

            await _workCandidatesAppService.CreateWorkCandidateAsync(input, PoolId);
            return RedirectToPage("/WorkCandidates/Index", new { id = PoolId });
        }

        public async Task<IActionResult> OnPostUpdateWorkCandidateAsync(Guid id, Guid PoolId, WorkCandidatesUpdateDto input)
        {

            if (!string.IsNullOrEmpty(input.ToSection.ToString()))
            {
                var sections = input.ToSection.Split('-');
                if (sections.Length == 2)
                {
                    if (int.TryParse(sections[0], out int fromSection))
                    {
                        input.FromSection = fromSection;
                    }

                    if (int.TryParse(sections[1], out int toSection))
                    {
                        input.ToSection = toSection.ToString();
                    }
                }
            }


            await _workCandidatesAppService.UpdateWorkCandidateAsync(id, input, PoolId);
            return RedirectToPage("/WorkCandidates/Index", new { id = PoolId });
        }

        public async Task<IActionResult> OnGetGetBPN()
        {
            string storedProcedure = "dbo.UspUIGetBPN";
            DataTable dataTable = new DataTable();

            if (!string.IsNullOrEmpty(storedProcedure))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = storedProcedure;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 24000;

                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            BPNList = new List<BPNModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                BPNList.Add(new BPNModel
                {
                    Code = row["Code"].ToString(),
                    ShortDescription = row["ShortDescr"].ToString(),
                    LongDescription = row["LongDescr"].ToString(),
                });
            }

            return new JsonResult(BPNList);
        }


    }
}




public class DistrictModel
{
    public int DistrictId { get; set; }
    public string DistrictName { get; set; }
}

public class CountyModel
{
    public int cnty { get; set; }
    public string countyName { get; set; }
}

public class RouteModel
{
    public string Rte { get; set; }
}

public class SectionModel
{
    public string FromSection { get; set; }
    public string ToSection { get; set; }
    public string FromTo { get; set; }
}

public class BPNModel
{
    public string Code { get; set; }
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
}

public class BRKEYModel
{
    public string BRKEY { get; set; }
    public string BMSID { get; set; }
}

public class WorkTypeModal
{
    public string WorkType { get; set; }
}
