using AutoMapper;
using DownNotifier.Business.Services;
using DownNotifier.DataAccess.Abstract;
using DownNotifier.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public class TargetApplicationManager(
        ITargetApplicationRepository _repository,
        INotificationService _notificationService,
        ILogger<TargetApplicationManager> _logger,
        UserManager<AppUser> _userManager,
        HttpClient _httpClient,
        IMapper _mapper,
        IRecurringJobManager _recurringJobManager) : ITargetApplicationService
{
    public async Task<IEnumerable<TargetApplicationViewModel>> GetAllAsync(string UserId)
    {
        var result = await _repository.GetAllAsync(UserId);
        return _mapper.Map<IEnumerable<TargetApplicationViewModel>>(result);
    }

    public async Task<TargetApplicationViewModel> GetByIdAsync(int id)
    {
        try
        {
            var result = await _repository.GetByIdAsync(id);
            _logger.LogInformation("Görev Listelendi. ID: {Id}", id);
            return _mapper.Map<TargetApplicationViewModel>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Listeleme sırasında hata oluştu. ID: {Id}", id);
            throw;
        }
    }
    public async Task AddAsync(TargetApplicationViewModel targetApplicationViewModel, string UserId)
    {
        var targetApplicationData = _mapper.Map<TargetApplication>(targetApplicationViewModel);
        try
        {
            targetApplicationData.UserId = UserId;
            await _repository.AddAsync(targetApplicationData);
           
            _recurringJobManager.AddOrUpdate(
                $"check-app-{targetApplicationData.Id}",
                () => CheckTargetsAsync(targetApplicationData.Id),
                Cron.MinuteInterval(targetApplicationViewModel.CheckIntervalInMinutes));

            //RecurringJob.AddOrUpdate(
            //    $"check-app-{targetApplicationData.Id}",
            //    () => CheckTargetsAsync(targetApplicationData.Id),
            //    Cron.MinuteInterval(targetApplicationViewModel.CheckIntervalInMinutes));

            _logger.LogInformation("Yeni görev eklendi: {@TargetApplicationData}", targetApplicationData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Görev ekleme hatası: {@TargetApplicationData}", targetApplicationData);
            throw;
        }
    }

    public async Task UpdateAsync(TargetApplicationViewModel targetApplicationViewModel, string UserId)
    {
        var targetApplicationData = _mapper.Map<TargetApplication>(targetApplicationViewModel);
        try
        {
            targetApplicationData.UserId = UserId;
            await _repository.UpdateAsync(targetApplicationData);

            _recurringJobManager.AddOrUpdate(
                $"check-app-{targetApplicationData.Id}",
                () => CheckTargetsAsync(targetApplicationData.Id),
                Cron.MinuteInterval(targetApplicationViewModel.CheckIntervalInMinutes));

            _logger.LogInformation("Görev güncellendi: {@TargetApplicationData}", targetApplicationData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Güncelleme hatası: {@TargetApplicationData}", targetApplicationData);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            _recurringJobManager.RemoveIfExists($"check-app-{id}");

            //RecurringJob.RemoveIfExists($"check-app-{id}");
            await _repository.DeleteAsync(id);
            _logger.LogInformation("Görev silindi. ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Silme hatası. ID: {Id}", id);
            throw;
        }
    }

    public async Task CheckTargetsAsync(int targetId)
    {
        var target = await _repository.GetByIdAsync(targetId);

        try
        {
            var response = await _httpClient.GetAsync(target.Url);
            if (!response.IsSuccessStatusCode)
            {
                var email = (await _userManager.FindByIdAsync(target.UserId))?.Email;
                await _notificationService.NotifyAsync(email, $"DOWN: {target.Name}", $"Hedef uygulama çalışmıyor. URL: {target.Url}");
            }

            _logger.LogInformation("URL kontrol edildi: {Url}, Durum: {StatusCode}", target.Url, response.StatusCode);


            target.LastChecked = DateTime.Now;
            target.LastStatusCode = (int)response.StatusCode;
            await _repository.UpdateAsync(target);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hedef URL kontrolünde hata: {TargetName}", target.Name);
        }
    }
}