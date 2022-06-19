using System.ComponentModel.DataAnnotations;
using Totoro.Attributes;

namespace Totoro.Transport.Options;

[Option("Core:Transport:AmazonSqs")]
public class AmazonSqsOptions
{
    [Required]
    public string Prefix { get; set; } = null!;

    [Required]
    public string Host { get; set; } = null!;

    [Required]
    [Environment("AMAZONSQS_ACCESS_KEY")]
    public string AccessKey { get; set; } = null!;

    [Required]
    [Environment("AMAZONSQS_SECRET_KEY")]
    public string SecretKey { get; set; } = null!;
}
