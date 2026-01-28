namespace Native.FluentValidation.Core;

public readonly struct PropertySelector<T, TValue>
{
    public Func<T, TValue> Accessor { get; }
    public string Name { get; }

    public PropertySelector(Func<T, TValue> accessor, string name)
    {
        Accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Property name must be provided.", nameof(name))
            : name;
    }
}
