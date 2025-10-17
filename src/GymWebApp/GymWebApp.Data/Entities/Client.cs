using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities
{
    public class Client : User
    {
        public List<GymMembership> GymMemberships { get; set; } = new List<GymMembership>();
        public List<IndividualTraining> IndividualTrainings { get; set; } = new List<IndividualTraining>();
        public List<GroupTrainingParticipation> GroupTrainingsParticipations { get; set; } = new List<GroupTrainingParticipation>();
        public List<TrainingPlan> TrainingPlans { get; set; } = new List<TrainingPlan>();
    }
}
