using GymWebApp.Application.WebModels.Client;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;

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

    public static ClientListWebModel ToClientListWebModel(this Client client, string email)
    {
        if (client == null) return null!;

        var currentMembership = client.GymMemberships
            .Where(m => !m.Removed)
            .OrderByDescending(m => m.StartDate)
            .FirstOrDefault();

        string? membershipStatus = null;
        string? planName = null;

        if (currentMembership != null)
        {
            membershipStatus = currentMembership.Status switch
            {
                MembershipStatus.Active => "Active",
                MembershipStatus.PendingCancellation => "Active",
                MembershipStatus.Cancelled => "Cancelled",
                MembershipStatus.Expired => "Expired",
                _ => "None"
            };
            planName = currentMembership.MembershipPlan?.Type;
        }
        else
        {
            membershipStatus = "None";
        }

        return new ClientListWebModel
        {
            Id = client.Id,
            FirstName = client.Name,
            LastName = client.Surname,
            Email = email,
            MembershipStatus = membershipStatus,
            CurrentPlanName = planName
        };
    }

    public static ClientDetailsWebModel ToClientDetailsWebModel(this Client client, string email, string? phoneNumber)
    {
        if (client == null) return null!;

        var currentMembership = client.GymMemberships
            .Where(m => !m.Removed)
            .OrderByDescending(m => m.StartDate)
            .FirstOrDefault();

        ClientMembershipWebModel? membershipModel = null;

        if (currentMembership != null)
        {
            membershipModel = new ClientMembershipWebModel
            {
                Id = currentMembership.Id,
                PlanName = currentMembership.MembershipPlan?.Type ?? "Unknown",
                PlanDescription = currentMembership.MembershipPlan?.Description ?? "",
                Status = currentMembership.Status switch
                {
                    MembershipStatus.Active => "Active",
                    MembershipStatus.PendingCancellation => "Active",
                    MembershipStatus.Cancelled => "Cancelled",
                    MembershipStatus.Expired => "Expired",
                    _ => "Unknown"
                },
                StartDate = currentMembership.StartDate,
                EndDate = currentMembership.EndDate,
                Price = currentMembership.MembershipPlan?.Price ?? 0,
                CanAccessGroupTraining = currentMembership.MembershipPlan?.CanAccessGroupTraining ?? false,
                CanAccessPersonalTraining = currentMembership.MembershipPlan?.CanAccessPersonalTraining ?? false
            };
        }

        return new ClientDetailsWebModel
        {
            Id = client.Id,
            FirstName = client.Name,
            LastName = client.Surname,
            Email = email,
            PhoneNumber = phoneNumber,
            DateOfBirth = client.DateOfBirth,
            RegistrationDate = client.RegistrationDate,
            Address = client.Address,
            CurrentMembership = membershipModel
        };
    }
}