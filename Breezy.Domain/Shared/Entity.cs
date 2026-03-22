namespace Breezy.Domain.Shared;

using System;
public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }
    
    public DateTime CreatedAt { get; protected set; }
    
    public DateTime UpdatedAt { get; protected set; }
    
    protected Entity()
    {
    }
    
    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("Entity ID cannot be empty");

        Id = id;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var entity = (Entity)obj;
        return Id == entity.Id;
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public static bool operator ==(Entity left, Entity right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }
    
    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
    
    public bool Equals(Entity other)
    {
        return Equals((object)other);
    }
}