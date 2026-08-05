using FluentValidation;

namespace PAS.AspNetCore.Paging;

public abstract class PagedQueryValidator<T> : AbstractValidator<T> where T : IPagedQuery {
    public PagedQueryValidator() {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
    }
}
