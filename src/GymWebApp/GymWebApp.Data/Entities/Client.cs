using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities;

public class Client : User
{
    public List<GymMembership> GymMemberships { get; set; } = [];
    public List<IndividualTraining> IndividualTrainings { get; set; } = [];
    public List<GroupTrainingParticipation> GroupTrainingsParticipations { get; set; } = [];
    public List<TrainingPlan> TrainingPlans { get; set; } = [];
}
