using System.ComponentModel.DataAnnotations;
using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities;

public class Trainer : Employee
{
    public List<IndividualTraining> IndividualTrainings { get; set; } = [];

    public List<GroupTraining> GroupTrainings { get; set; } = [];

    public List<TrainingPlan> TrainingPlans { get; set;} = [];
    
    [MaxLength(1000)]
    public string? Specializations { get; set; }
}
