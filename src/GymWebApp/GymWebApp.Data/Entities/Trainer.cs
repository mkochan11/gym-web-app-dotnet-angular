using System.ComponentModel.DataAnnotations;
using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities
{
    public class Trainer : Employee
    {
        public List<IndividualTraining> IndividualTrainings { get; set; } = new List<IndividualTraining>();
        public List<GroupTraining> GroupTrainings { get; set; } = new List<GroupTraining>();
        
        [MaxLength(1000)]
        public string? Specializations { get; set; }
    }
}
