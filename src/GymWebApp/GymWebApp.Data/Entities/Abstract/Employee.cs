namespace GymWebApp.Data.Entities.Abstract;

public abstract class Employee : User
{
    public List<Employment> Employments { get; set; } = [];
}
