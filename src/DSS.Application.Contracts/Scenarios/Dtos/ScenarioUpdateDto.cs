using System;
using Volo.Abp.Application.Dtos;

namespace DSS.Scenarios.Dtos
{
    public class ScenarioUpdateDto: EntityDto<Guid>
    {
        public string ScenNo { get; set; }
        public string ScenName { get; set; }
        public string Description { get; set; }
        public string DeaInputSproc { get; set; }
        public string DeaOutputSproc { get; set; }
        public Guid PoolId { get; set; }
    }
}
