using System;
using System.Collections.Generic;
public class FiveIndicationsReportRequest
{
    public List<int> DepartmentIds { get; set; }
    public List<int> InstitutionIds { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int RoleId { get; set; }
}
