namespace GymHub.Api.Core;

public abstract class BaseEntity
{
    public int Id { get; set; }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class JsonCollectionAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
