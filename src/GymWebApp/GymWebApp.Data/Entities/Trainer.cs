using GymWebApp.Data.Enums;

namespace GymWebApp.Data.Entities;

public class Trainer : Employee
{
    public List<IndividualTraining> IndividualTrainings { get; set; } = [];

    public List<GroupTraining> GroupTrainings { get; set; } = [];

    public List<TrainingPlan> TrainingPlans { get; set;} = [];

    public Trainer(string accountId, string name, string surname, string address, DateTime dateOfBirth, Gender gender, EmployeeRole role) : base(accountId, name, surname, address, dateOfBirth, gender, role)
    {
    }
}
