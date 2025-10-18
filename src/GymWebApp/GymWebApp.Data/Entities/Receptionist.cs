using GymWebApp.Data.Entities.Abstract;

namespace GymWebApp.Data.Entities;

public class Receptionist : Employee
{
    public List<Shift> Shifts { get; set; } = [];
}
