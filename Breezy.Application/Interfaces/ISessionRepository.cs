using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.Sessions.ValueObjects;
using Breezy.Domain.SoundPacks.Entities;

namespace Breezy.Application.Interfaces;

public interface ISessionRepository
{
    Task AddAsync(CollaborationSession session);
    Task<CollaborationSession?> GetByCodeAsync(SessionCode code);
    Task<CollaborationSession?> GetByIdAsync(Guid id);
    Task UpdateAsync(CollaborationSession session);
    Task AddParticipantAsync(SessionParticipant participant);
    Task AddSubmissionAsync(SessionSubmission submission);
    Task<IEnumerable<CollaborationSession>> GetByParticipantAsync(Guid userId);



}