using DownNotifier.DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DownNotifier.DataAccess.Concrete
{
    internal class TargetApplicationRepository : EfEntityRepositoryBase<TargetApplication, DownNotifierDbContext>, ITargetApplicationRepository
    {
        private readonly DownNotifierDbContext _context;
        public TargetApplicationRepository(DownNotifierDbContext context)
            : base(context)
        {
            _context = context;
        }
        public async Task<List<TargetApplication>> GetAllAsync(string userId)
        {
            return await _context.TargetApplications
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }
        public async Task<TargetApplication> GetByIdAsync(int id)
        {
            return await _context.TargetApplications.FindAsync(id);
        }
        public async Task AddAsync(TargetApplication target)
        {
            await _context.TargetApplications.AddAsync(target);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(TargetApplication target)
        {
            _context.TargetApplications.Update(target);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var target = await _context.TargetApplications.FindAsync(id);
            if (target != null)
            {
                _context.TargetApplications.Remove(target);
                await _context.SaveChangesAsync();
            }
        }
    }
}