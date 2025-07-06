using AutoMapper;
using Utilities.Common;
using Utilities.Common.Domain;

namespace Utilities.Application
{
    public class CrudServiceBase<TEntity, TRepository, TDto, TInput, TPatch>(TRepository repo, IMapper mapper)
        : ICrudServiceBase<TDto, TInput, TPatch>

        where TEntity : Entity
        where TRepository : IRepository<TEntity>
        where TPatch : IPatch<TEntity>
    {
        protected TRepository Repository { get; } = repo;

        protected IMapper Mapper { get; } = mapper;

        public async Task<TDto?> GetAsync(Guid id)
        {
            var entity = await Repository.GetAsync(id);
            return Mapper.Map<TDto>(entity);
        }

        public async Task<IList<TDto>> ListAsync()
        {
            var entities = await Repository.ListAsync();
            return entities.Select(Mapper.Map<TDto>).ToList();
        }

        public async Task<PagedResult<TDto>> ListAsync(int page, int size = 10)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page), "Page cannot be less than one.");

            if (size < 1)
                throw new ArgumentOutOfRangeException(nameof(page), "Size cannot be less than one.");

            var totalCount = await Repository.CountAsync();
            var entities = await Repository.ListAsync(skip: (page - 1) * size, take: size);

            return new PagedResult<TDto>
            {
                TotalCount = totalCount,
                Items = entities.Select(Mapper.Map<TDto>).ToList(),
                CurrentPage = page,
                PageSize = size,
                NumPages = (int)Math.Ceiling((double)totalCount / size)
            };
        }

        public async Task<TDto> CreateAsync(TInput input)
        {
            var entity = await CreateUnmappedAsync(input);
            return Mapper.Map<TDto>(entity);
        }

        public async Task UpdateAsync(Guid id, TPatch patch)
        {
            var entity = await Repository.GetAsync(id)
                ?? throw new ArgumentException("Entity not found.", nameof(id));

            patch.Apply(entity);

            Repository.Update(entity);
            await Repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await Repository.GetAsync(id)
                ?? throw new ArgumentException("Entity not found.", nameof(id));

            if (entity != null)
                Repository.Remove(entity);

            await Repository.SaveChangesAsync();
        }

        protected async Task<TEntity> CreateUnmappedAsync(TInput input)
        {
            var entity = Mapper.Map<TEntity>(input);

            await Repository.AddAsync(entity);
            await Repository.SaveChangesAsync();

            return entity;
        }
    }
}
