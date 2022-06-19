using System.ComponentModel.DataAnnotations;
using Totoro.Attributes;

namespace Totoro.Options;

[Option("Core:Protection")]
public class ProtectionOptions
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    [Environment("PROTECTION_KEYS_PATH")]
    public string Path { get; set; } = null!;
}
