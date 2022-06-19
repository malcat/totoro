namespace Totoro.Attributes;

/// <summary>
/// It will replace the property value with the corresponding environment variable value.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class EnvironmentAttribute : Attribute
{
    public EnvironmentAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// The name of the environment variable.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// If set to false, it will replace the environment value only if it is defined.
    /// </summary>
    public bool Priority { get; set; } = true;
}
