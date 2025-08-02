using ErpBackendApi.BLL.Interfaces;
using ErpBackendApi.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace ErpBackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettings _set;
        public SettingController(ISettings set)
        {
            _set = set;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSettings()
        {
            var operation_GetAllSettings = await _set.GetAllSettingsAsync();
            return Ok(operation_GetAllSettings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSettingsById(int id)
        {
            var operation_GetSettingsById = await _set.GetSettingByIdAsync(id);
            if (operation_GetSettingsById == null)
            {
                return NotFound("Setting not found.");
            }
            return Ok(operation_GetSettingsById);
        }

        [HttpPost]
        public async Task<IActionResult> AddSetting(Setting setting)
        {
            var operation_AddSetting = await _set.AddSettingAsync(setting);
            if (operation_AddSetting == null)
            {
                return NotFound("Error! Same key already exists.");
            }
            return Ok("Setting added successfully.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateSetting(Setting setting)
        {
            var operation_UpdateSetting = await _set.UpdateSettingAsync(setting);
            if (operation_UpdateSetting == null)
            {
                return NotFound("Error! Setting not found or key should be unique.");
            }
            return Ok("Setting updated successfully.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSetting(int id)
        {
            var operation_DeleteSetting = await _set.DeleteSettingAsync(id);
            if (operation_DeleteSetting == false)
            {
                return NotFound("Setting cannot be deleted. Key not found.");
            }
            return Ok("Setting deleted successfully.");
        }
    }
}
