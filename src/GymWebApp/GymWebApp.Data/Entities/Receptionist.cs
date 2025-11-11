using GymWebApp.Data.Enums;

namespace GymWebApp.Data.Entities;

public class Receptionist : Employee
{
    public List<Shift> Shifts { get; set; } = [];

    public Receptionist(string accountId, string name, string surname, string address, DateTime dateOfBirth, Gender gender, EmployeeRole role) : base(accountId, name, surname, address, dateOfBirth, gender, role)
    {
    }
}
