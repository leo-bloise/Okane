namespace Okane.Core.Data.Requests;

public interface IPageRequest
{
    public int Page { get; set; }

    public int PageSize { get; set; }    
}
