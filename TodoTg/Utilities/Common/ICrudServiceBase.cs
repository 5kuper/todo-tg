using Utilities.Common;

namespace Utilities.Application
{
    public interface ICrudServiceBase<TDto, TInput, TPatch>
    {
        Task<TDto?> GetAsync(Guid id);

        Task<IList<TDto>> ListAsync();
        Task<PagedResult<TDto>> ListAsync(int page, int size = 10);

        Task<TDto> CreateAsync(TInput input);
        Task UpdateAsync(Guid id, TPatch patch);
        Task DeleteAsync(Guid id);
    }
}
