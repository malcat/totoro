using System.ComponentModel.DataAnnotations;
using Totoro.Attributes;

namespace Totoro.Web.Options;

[Option("Core:Web:Culture")]
public class CultureOptions
{
    [Required]
    public string Default { get; set; } = "pt-BR";

    [Required]
    public IEnumerable<string> Supported { get; set; } = new[] { "pt-BR" };
}
