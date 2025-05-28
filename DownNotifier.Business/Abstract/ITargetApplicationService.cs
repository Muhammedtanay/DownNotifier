using DownNotifier.ViewModels;

public interface ITargetApplicationService
{
    Task CheckTargetsAsync(int targetId);
    Task<IEnumerable<TargetApplicationViewModel>> GetAllAsync(string UserId);
    Task<TargetApplicationViewModel> GetByIdAsync(int id);
    Task AddAsync(TargetApplicationViewModel targetApplication, string UserId);
    Task UpdateAsync(TargetApplicationViewModel targetApplication, string UserId);
    Task DeleteAsync(int id);
}

