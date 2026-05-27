using Okane.Core.Data.Requests;
using System.ComponentModel.DataAnnotations;

namespace Okane.Infrastructure.Requests;

public class PageRequest : IPageRequest
{
    [Required]
    [Range(0, int.MaxValue)]
    public int Page { get; set; } = 0;

    [Required]
    [Range(1, 20)]
    public int PageSize { get; set; } = 20;
}
