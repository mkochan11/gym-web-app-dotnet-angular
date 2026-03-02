using GymWebApp.Domain.Entities.Abstract;
using GymWebApp.Domain.Enums;

namespace GymWebApp.Domain.Entities;

public class Client : BaseUserEntity
{
    public List<GymMembership> GymMemberships { get; set; } = [];
    public List<IndividualTraining> IndividualTrainings { get; set; } = [];
    public List<GroupTrainingParticipation> GroupTrainingsParticipations { get; set; } = [];
    public List<TrainingPlan> TrainingPlans { get; set; } = [];

    public Client () { }

    public Client(string accountId, string name, string surname, string address, DateTime dateOfBirth, Gender gender)
    {
        AccountId = accountId;
        Name = name;
        Surname = surname;
        Address = address;
        DateOfBirth = dateOfBirth;
        Gender = gender;
    }
}
