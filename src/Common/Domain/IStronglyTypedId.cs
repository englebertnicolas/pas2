using PAS.Core.Results;

namespace PAS.Domain;

public interface IStronglyTypedId<TId, TValue>
        where TId : IStronglyTypedId<TId, TValue>
        where TValue : notnull {

    TValue Value { get; }
    static abstract ErrorOr<TId> From(TValue value);
    static abstract TId Hydrate(TValue value);
}
