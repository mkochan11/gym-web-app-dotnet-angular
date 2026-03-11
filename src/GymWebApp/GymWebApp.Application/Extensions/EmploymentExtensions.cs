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
            StartDate = employment.StartDate, EndDate = employment.EndDate, HourlyRate = employment.HourlyRate
        };
    }
}