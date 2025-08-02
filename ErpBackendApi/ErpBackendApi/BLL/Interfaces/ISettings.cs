using ErpBackendApi.DAL.Models;

namespace ErpBackendApi.BLL.Interfaces
{
    public interface ISettings
    {
        Task<IEnumerable<Setting>> GetAllSettingsAsync();
        Task<Setting> GetSettingByIdAsync(int id);
        Task<Setting> AddSettingAsync(Setting setting);
        Task<Setting> UpdateSettingAsync(Setting setting);
        Task<bool> DeleteSettingAsync(int id);
    }
}
