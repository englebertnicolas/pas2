namespace PAS.AspNetCore.Paging;

public interface IPagedQuery {
    int PageNumber { get; }
    int PageSize { get; }
}
