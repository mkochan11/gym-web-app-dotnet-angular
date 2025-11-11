using GymWebApp.Data.Entities.Abstract;
using GymWebApp.Data.Enums;

namespace GymWebApp.Data.Entities;

public class Employee : BaseUserEntity
{
    public List<Employment> Employments { get; set; } = [];

    public EmployeeRole Role { get; set; }

    public Employee() { }

    public Employee(string accountId, string name, string surname, string address, DateTime dateOfBirth, Gender gender, EmployeeRole role)
    {
        AccountId = accountId;
        Name = name;
        Surname = surname;
        Address = address;
        Role = role;
        DateOfBirth = dateOfBirth;
        Gender = gender;
    }
}