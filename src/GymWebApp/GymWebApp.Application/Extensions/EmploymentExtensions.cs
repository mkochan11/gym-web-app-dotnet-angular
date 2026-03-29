using GymWebApp.Application.WebModels.Employee;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Extensions;

public static class EmploymentExtensions
{
    public static EmploymentWebModel ToEmploymentWebModel(this Employment employment)
    {
        if (employment == null) return null!;

        return new EmploymentWebModel()
        {
            Id = employment.Id,
            StartDate = employment.StartDate,
            EndDate = employment.EndDate,
            HourlyRate = employment.HourlyRate ?? 0,
            CreatedAt = employment.CreatedAt,
            CreatedBy = employment.CreatedBy?.UserName ?? "Unknown"
        };
    }
}