using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.ImportSessions.Dtos
{
    public class ImportSessionDto : EntityDto<Guid>
    {
        public int SessionNo { get; set; }
        public string? ImportSourceType { get; set; }
        public string? ImportSourceName { get; set; }
        public string DataSourceType { get; set; }
        public string DataSourceName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? CompletedStatus { get; set; }
        public string? Notes { get; set; }
        public Guid? PoolId { get; set; }
    }
}
