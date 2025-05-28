namespace DownNotifier.DataAccess.Abstract
{
    public interface ITargetApplicationRepository : IEntityRepository<TargetApplication>
    {
        Task<List<TargetApplication>> GetAllAsync(string userId);

        Task<TargetApplication> GetByIdAsync(int id);

        Task AddAsync(TargetApplication target);

        Task UpdateAsync(TargetApplication target);

        Task DeleteAsync(int id);
    }
}
