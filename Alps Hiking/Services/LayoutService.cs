using Alps_Hiking.DAL;
using Alps_Hiking.Entities;
using Microsoft.AspNetCore.Identity;

namespace Alps_Hiking.Services
{
    public class LayoutService
    {
        private readonly AlpsHikingDbContext _context;
        public LayoutService(AlpsHikingDbContext context)
        {
            _context = context;
        }

        public List<Setting> GetSettings()
        {
            List<Setting> settings = _context.Settings.ToList();
            return settings;
        }
    }
}
