using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.ERPDataContext;
using ErpBackendApi.DAL.Models;
using Microsoft.EntityFrameworkCore;
using static ErpBackendApi.Utilities.Helper.LoggerClass;

namespace ErpBackendApi.BLL.Services
{
    public class SettingService : ISettings
    {
        private readonly AppDataContext _context;
        public SettingService(AppDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Setting>> GetAllSettingsAsync()
        {
            return await _context.settings.ToListAsync();
        }

        public async Task<Setting> GetSettingByIdAsync(int id)
        {
            return await _context.settings.FirstOrDefaultAsync(s => s.id == id);
        }

        public async Task<Setting> AddSettingAsync(Setting setting)
        {
            var existingKey = await _context.settings.FirstOrDefaultAsync(s => s.key == setting.key);
            if (existingKey != null)
            {
                Logger("Error! Same key already exists.");
                return null;
            }
            setting.updated_at = DateTime.UtcNow;
            _context.settings.Add(setting);
            await _context.SaveChangesAsync();
            return setting;
        }

        public async Task<Setting> UpdateSettingAsync(Setting setting)
        {
            var existingSettings = await _context.settings.FirstOrDefaultAsync(s => s.id == setting.id);
            var existingKey = await _context.settings.FirstOrDefaultAsync(s => s.key == setting.key && s.id != setting.id);
            if (existingSettings == null)
            {
                Logger("Error! Failed to update settings. Setting not found.");
                return null;
            }
            if (existingKey != null)
            {
                Logger("Key cannot be same as other keys. Key should be unique.");
                return null;
            }
            existingSettings.key = setting.key;
            existingSettings.value = setting.value;
            existingSettings.updated_at = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existingSettings;
        }

        public async Task<bool> DeleteSettingAsync(int id)
        {
            var existingSetting = await _context.settings.FirstOrDefaultAsync(s => s.id == id);
            if (existingSetting == null)
            {
                Logger("Setting cannot be deleted. Key not found.");
                return false;
            }
            _context.settings.Remove(existingSetting);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
