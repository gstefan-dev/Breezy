using Breezy.Application.Interfaces;
using Breezy.Domain.Sessions.Entities;
using Breezy.Domain.Sessions.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Breezy.Infrastructure.Persistence.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly BreezyDbContext _context;

    public SessionRepository(BreezyDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CollaborationSession session)
    {
        await _context.Sessions.AddAsync(session);
        await _context.SaveChangesAsync();
    }

    public async Task<CollaborationSession?> GetByCodeAsync(SessionCode code)
    {
        return await _context.Sessions
            .Include(s => s.Participants)
            .Include(s => s.Submissions)
            .FirstOrDefaultAsync(s => s.Code.Value == code.Value);
    }

    public async Task<CollaborationSession?> GetByIdAsync(Guid id)
    {
        return await _context.Sessions.FindAsync(id);
    }

    public async Task UpdateAsync(CollaborationSession session)
    {
        await _context.SaveChangesAsync();
    }
    
    public async Task AddParticipantAsync(SessionParticipant participant)
    {
        await _context.Set<SessionParticipant>().AddAsync(participant);
        await _context.SaveChangesAsync();
    }
    public async Task AddSubmissionAsync(SessionSubmission submission)
    {
        await _context.Set<SessionSubmission>().AddAsync(submission);
        await _context.SaveChangesAsync();
    }
}