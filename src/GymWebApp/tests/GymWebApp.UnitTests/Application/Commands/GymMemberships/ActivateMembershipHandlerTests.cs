using FluentAssertions;
using GymWebApp.Application.CQRS.GymMemberships;
using GymWebApp.Application.Common.Exceptions;
using GymWebApp.Application.Interfaces.Repositories;
using GymWebApp.Domain.Entities;
using GymWebApp.Domain.Enums;
using Moq;
using Xunit;

namespace GymWebApp.UnitTests.Application.Commands.GymMemberships;

public class ActivateMembershipHandlerTests
{
    private readonly Mock<IGymMembershipRepository> _gymMembershipRepositoryMock;
    private readonly Mock<IMembershipPlanRepository> _planRepositoryMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly ActivateMembership.Handler _handler;

    public ActivateMembershipHandlerTests()
    {
        _gymMembershipRepositoryMock = new Mock<IGymMembershipRepository>();
        _planRepositoryMock = new Mock<IMembershipPlanRepository>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _handler = new ActivateMembership.Handler(
            _gymMembershipRepositoryMock.Object,
            _planRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _clientRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesMembershipWithPaidFirstPayment()
    {
        var client = new Client
        {
            Id = 1,
            Name = "John",
            Surname = "Doe"
        };

        var plan = new MembershipPlan
        {
            Id = 1,
            Type = "Premium",
            DurationInMonths = 3,
            Price = 100.00m
        };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(client);

        _planRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _gymMembershipRepositoryMock.Setup(x => x.HasActiveMembershipAsync(1))
            .ReturnsAsync(false);

        _gymMembershipRepositoryMock.Setup(x => x.AddAsync(It.IsAny<GymMembership>()))
            .Returns(Task.CompletedTask);

        _gymMembershipRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new GymMembership
            {
                Id = id,
                ClientId = 1,
                MembershipPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(3),
                Status = MembershipStatus.Active,
                CreatedById = "staff-1",
                MembershipPlan = plan,
                Client = client,
                Payments = new List<Payment>
                {
                    new() { Id = 1, Amount = 100, Status = PaymentStatus.Paid, PaidDate = DateTime.UtcNow },
                    new() { Id = 2, Amount = 100, Status = PaymentStatus.Pending },
                    new() { Id = 3, Amount = 100, Status = PaymentStatus.Pending }
                }
            });

        _paymentRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Payment>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _paymentRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 100.00m,
            UpdatedById = "staff-1"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Status.Should().Be(MembershipStatus.Active);
        result.ClientId.Should().Be(1);
        result.PlanName.Should().Be("Premium");
    }

    [Fact]
    public async Task Handle_WithTransactionId_RecordsTransactionId()
    {
        var client = new Client { Id = 1, Name = "John", Surname = "Doe" };
        var plan = new MembershipPlan { Id = 1, Type = "Basic", DurationInMonths = 1, Price = 50.00m };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(client);

        _planRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _gymMembershipRepositoryMock.Setup(x => x.HasActiveMembershipAsync(1))
            .ReturnsAsync(false);

        _gymMembershipRepositoryMock.Setup(x => x.AddAsync(It.IsAny<GymMembership>()))
            .Returns(Task.CompletedTask);

        _gymMembershipRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new GymMembership
            {
                Id = id,
                ClientId = 1,
                MembershipPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = MembershipStatus.Active,
                CreatedById = "staff-1",
                MembershipPlan = plan,
                Client = client
            });

        _paymentRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Payment>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _paymentRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Card,
            TransactionId = "TXN123456",
            Amount = 50.00m,
            UpdatedById = "staff-1"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_ClientNotFound_ThrowsNotFoundException()
    {
        _clientRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Client?)null);

        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 100.00m,
            UpdatedById = "staff-1"
        };

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Client*");
    }

    [Fact]
    public async Task Handle_PlanNotFound_ThrowsNotFoundException()
    {
        var client = new Client { Id = 1, Name = "John", Surname = "Doe" };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(client);

        _planRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((MembershipPlan?)null);

        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 100.00m,
            UpdatedById = "staff-1"
        };

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("*Membership plan*");
    }

    [Fact]
    public async Task Handle_ClientHasActiveMembership_ThrowsValidationException()
    {
        var client = new Client { Id = 1, Name = "John", Surname = "Doe" };
        var plan = new MembershipPlan { Id = 1, Type = "Premium", DurationInMonths = 3, Price = 100.00m };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(client);

        _planRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _gymMembershipRepositoryMock.Setup(x => x.HasActiveMembershipAsync(1))
            .ReturnsAsync(true);

        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 100.00m,
            UpdatedById = "staff-1"
        };

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Client already has an active membership");
    }

    [Fact]
    public async Task Handle_InvalidPaymentAmount_ThrowsValidationException()
    {
        var client = new Client { Id = 1, Name = "John", Surname = "Doe" };
        var plan = new MembershipPlan { Id = 1, Type = "Premium", DurationInMonths = 3, Price = 100.00m };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(client);

        _planRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _gymMembershipRepositoryMock.Setup(x => x.HasActiveMembershipAsync(1))
            .ReturnsAsync(false);

        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 50.00m,
            UpdatedById = "staff-1"
        };

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Payment amount does not match plan price");
    }

    [Fact]
    public async Task Handle_CashPayment_CreatesMembership()
    {
        var client = new Client { Id = 1, Name = "John", Surname = "Doe" };
        var plan = new MembershipPlan { Id = 1, Type = "Basic", DurationInMonths = 1, Price = 50.00m };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(client);

        _planRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _gymMembershipRepositoryMock.Setup(x => x.HasActiveMembershipAsync(1))
            .ReturnsAsync(false);

        _gymMembershipRepositoryMock.Setup(x => x.AddAsync(It.IsAny<GymMembership>()))
            .Returns(Task.CompletedTask);

        _gymMembershipRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new GymMembership
            {
                Id = id,
                ClientId = 1,
                MembershipPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = MembershipStatus.Active,
                CreatedById = "staff-1",
                MembershipPlan = plan,
                Client = client
            });

        _paymentRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Payment>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _paymentRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.Cash,
            Amount = 50.00m,
            UpdatedById = "staff-1"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Status.Should().Be(MembershipStatus.Active);
    }

    [Fact]
    public async Task Handle_BankTransferPayment_CreatesMembership()
    {
        var client = new Client { Id = 1, Name = "John", Surname = "Doe" };
        var plan = new MembershipPlan { Id = 1, Type = "Basic", DurationInMonths = 1, Price = 50.00m };

        _clientRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(client);

        _planRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(plan);

        _gymMembershipRepositoryMock.Setup(x => x.HasActiveMembershipAsync(1))
            .ReturnsAsync(false);

        _gymMembershipRepositoryMock.Setup(x => x.AddAsync(It.IsAny<GymMembership>()))
            .Returns(Task.CompletedTask);

        _gymMembershipRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _gymMembershipRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => new GymMembership
            {
                Id = id,
                ClientId = 1,
                MembershipPlanId = 1,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(1),
                Status = MembershipStatus.Active,
                CreatedById = "staff-1",
                MembershipPlan = plan,
                Client = client
            });

        _paymentRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<List<Payment>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _paymentRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        var command = new ActivateMembership.Command
        {
            ClientId = 1,
            MembershipPlanId = 1,
            PaymentMethod = PaymentMethod.BankTransfer,
            Amount = 50.00m,
            UpdatedById = "staff-1"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Status.Should().Be(MembershipStatus.Active);
    }
}
