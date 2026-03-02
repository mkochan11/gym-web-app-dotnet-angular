using GymWebApp.Application.WebModels.Client;
using GymWebApp.Domain.Entities;

namespace GymWebApp.Application.Extensions;

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