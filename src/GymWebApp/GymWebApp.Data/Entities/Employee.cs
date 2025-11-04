using GymWebApp.Data.Entities.Abstract;
using GymWebApp.Data.Enums;

namespace GymWebApp.Data.Entities;

public class Employee : BaseUserEntity
{
    public List<Employment> Employments { get; set; } = [];

    public EmployeeRole Role { get; set; }
}