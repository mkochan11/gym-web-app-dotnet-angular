using GymWebApp.ApplicationCore.Models.Client;
using GymWebApp.ApplicationCore.Models.Trainer;
using GymWebApp.Data.Entities;

namespace GymWebApp.ApplicationCore.Extensions;

public static class ClientExtensions
{
    public static ClientWebModel ToClientWebModel(this Client client)
    {
        if (client == null) return null!;

        return new ClientWebModel
        {
            Id = client.Id,
            FirstName = client.Name,
            LastName = client.Surname,
        };
    }
}