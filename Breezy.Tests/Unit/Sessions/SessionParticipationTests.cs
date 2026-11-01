using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.Sessions.Exceptions;
using Breezy.Domain.Sessions.ValueObjects;

namespace Breezy.Tests.Domain.Sessions;

public class SessionParticipationTests
{
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    private CollaborationSession CreateSessionWithParticipant()
    {
        var session = CollaborationSession.Create(_ownerId, "owner", "Test session", maxParticipants: 5);
        session.AddParticipant(_userId, "participant");
        return session;
    }

    [Fact]
    public void Leave_ActiveParticipant_MarksAsLeft()
    {
        var session = CreateSessionWithParticipant();

        session.Leave(_userId);

        var participant = session.Participants.Single(p => p.UserId == _userId);
        Assert.False(participant.IsActive);
        Assert.Equal(ParticipantExitReason.Left, participant.ExitReason);
    }

    [Fact]
    public void Leave_Owner_Throws()
    {
        var session = CreateSessionWithParticipant();

        Assert.Throws<SessionDomainException>(() => session.Leave(_ownerId));
    }

    [Fact]
    public void AddParticipant_AfterLeaving_Rejoins()
    {
        var session = CreateSessionWithParticipant();
        session.Leave(_userId);

        session.AddParticipant(_userId, "participant");

        Assert.True(session.Participants.Single(p => p.UserId == _userId).IsActive);
        Assert.Equal(2, session.Participants.Count);
    }

    [Fact]
    public void RemoveParticipant_ByOwner_MarksAsRemoved()
    {
        var session = CreateSessionWithParticipant();

        session.RemoveParticipant(_ownerId, _userId);

        var participant = session.Participants.Single(p => p.UserId == _userId);
        Assert.False(participant.IsActive);
        Assert.Equal(ParticipantExitReason.RemovedByOwner, participant.ExitReason);
    }

    [Fact]
    public void RemoveParticipant_ByNonOwner_Throws()
    {
        var session = CreateSessionWithParticipant();

        Assert.Throws<SessionDomainException>(() => session.RemoveParticipant(_userId, _ownerId));
    }

    [Fact]
    public void AddParticipant_AfterBeingRemoved_Throws()
    {
        var session = CreateSessionWithParticipant();
        session.RemoveParticipant(_ownerId, _userId);

        Assert.Throws<SessionDomainException>(() => session.AddParticipant(_userId, "participant"));
    }

    [Fact]
    public void ArchiveForUser_OwnerOnActiveSession_Throws()
    {
        var session = CreateSessionWithParticipant();

        Assert.Throws<SessionDomainException>(() => session.ArchiveForUser(_ownerId));
    }

    [Fact]
    public void ArchiveForUser_ThenRestore_TogglesArchived()
    {
        var session = CreateSessionWithParticipant();

        session.ArchiveForUser(_userId);
        Assert.True(session.Participants.Single(p => p.UserId == _userId).IsArchived);

        session.RestoreForUser(_userId);
        Assert.False(session.Participants.Single(p => p.UserId == _userId).IsArchived);
    }

    [Fact]
    public void GetParticipantCount_ExcludesFormerParticipants()
    {
        var session = CreateSessionWithParticipant();
        session.Leave(_userId);

        Assert.Equal(1, session.GetParticipantCount());
    }
}