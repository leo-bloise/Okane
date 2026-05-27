namespace Okane.Core.Data;

public class Page<TEntity>
{
    public IReadOnlyCollection<TEntity> Items { get; set; } = new List<TEntity>();

    public int TotalPages { get; set; }

    public int PageSize { get; set; }

    public int PageIndex { get; set; }
}
