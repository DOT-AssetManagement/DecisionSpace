using AutoMapper;
using DSS.AssetTypeParametersData.Dtos;
using DSS.Counties.Dtos;
using DSS.Entities;
using DSS.Goals.Dtos;
using DSS.ImportSessions.Dtos;
using DSS.OthersParameters.Dtos;
using DSS.Pool.Dtos;
using DSS.PoolScoreParameters.Dtos;
using DSS.ProjectCandidates.Dtos;
using DSS.Scenarios.Dtos;
using DSS.WorkCandidates.Dtos;

namespace DSS;

public class DSSApplicationAutoMapperProfile : Profile
{
    public DSSApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        #region Goals

        CreateMap<Entities.Goal, GoalDto>().ReverseMap();
        CreateMap<Entities.Goal, GoalCreateDto>().ReverseMap();
        CreateMap<Entities.Goal, GoalUpdateDto>().ReverseMap();

        #endregion

        #region Scenarios

        CreateMap<Entities.Scenario, ScenarioDto>()
            .ForMember(x => x.PoolName, y => y.MapFrom(z => z.Pool.Name))
            .ReverseMap();
        CreateMap<Entities.Scenario, ScenarioCreateDto>().ReverseMap();
        CreateMap<Entities.Scenario, ScenarioUpdateDto>().ReverseMap();

        #endregion

        #region Pools

        CreateMap<Entities.CandidatePool, PoolDto>().ReverseMap();
        CreateMap<Entities.CandidatePool, PoolCreateDto>()
            .ForMember(x=> x.LibNo, y=> y.MapFrom(z=> z.LibNo))
            .ReverseMap();
        CreateMap<PoolUpdateDto, CandidatePool>()
            .ForMember(x => x.LibNo, y => y.MapFrom(z => z.LibNo))
            .ReverseMap();

        #endregion

        #region Pool Score Parametere

        CreateMap<Entities.PoolScoreParameter, PoolScoreParametersDto>().ReverseMap();
        CreateMap<Entities.PoolScoreParameter, PoolScoreParametersCreateDto>()
            .ReverseMap();
        CreateMap<PoolScoreParametersUpdateDto, PoolScoreParameter>()
            .ReverseMap();

        #endregion

        CreateMap<Entities.WorkCandidate, WorkCandidatesDto>().ReverseMap();
        CreateMap<Entities.WorkCandidate, WorkCandidatesCreateDto>().ReverseMap();
        CreateMap<Entities.WorkCandidate, WorkCandidatesUpdateDto>().ReverseMap();


        CreateMap<Entities.ProjectCandidate, ProjectCandidateDto>().ReverseMap();
        CreateMap<Entities.ProjectCandidate, ProjectCandidateCreateDto>().ReverseMap();
        CreateMap<Entities.ProjectCandidate, ProjectCandidateUpdateDto>().ReverseMap();



        CreateMap<Entities.AssetTypeParametersData, AssetTypeParameterDataDto>().ReverseMap();
        CreateMap<Entities.AssetTypeParametersData, AssetTypeParameterDataCreateDto>().ReverseMap();
        CreateMap<Entities.AssetTypeParametersData, AssetTypeParameterDataUpdateDto>().ReverseMap();


        CreateMap<Entities.TblOtherParameters, TblOtherParameterDto>().ReverseMap();
        CreateMap<Entities.TblOtherParameters, TblOtherParameterCreateDto>().ReverseMap();
        CreateMap<Entities.TblOtherParameters, TblOtherParameterUpdateDto>().ReverseMap();

        CreateMap<Entities.County, CountyDto>().ReverseMap();
        CreateMap<Entities.County, CountyCreateDto>().ReverseMap();
        CreateMap<Entities.County, CountyUpdateDto>().ReverseMap();

        CreateMap<Entities.ImportSession, ImportSessionDto>().ReverseMap();

    }
}
