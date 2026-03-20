namespace GymWebApp.Application.WebModels.MembershipPlan;

public class MembershipPlanWebModel
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string DurationTime { get; set; } = null!;

    public int DurationInMonths { get; set; }

    public bool CanReserveTrainings { get; set; }

    public bool CanAccessGroupTraining { get; set; }

    public bool CanAccessPersonalTraining { get; set; }

    public bool CanReceiveTrainingPlans { get; set; }

    public int? MaxTrainingsPerMonth { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public class CreateMembershipPlanWebModel
{
    public string Type { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string DurationTime { get; set; } = null!;

    public int DurationInMonths { get; set; }

    public bool CanReserveTrainings { get; set; }

    public bool CanAccessGroupTraining { get; set; }

    public bool CanAccessPersonalTraining { get; set; }

    public bool CanReceiveTrainingPlans { get; set; }

    public int? MaxTrainingsPerMonth { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateMembershipPlanWebModel
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string DurationTime { get; set; } = null!;

    public int DurationInMonths { get; set; }

    public bool CanReserveTrainings { get; set; }

    public bool CanAccessGroupTraining { get; set; }

    public bool CanAccessPersonalTraining { get; set; }

    public bool CanReceiveTrainingPlans { get; set; }

    public int? MaxTrainingsPerMonth { get; set; }

    public bool IsActive { get; set; }
}
