namespace Pointmaster.Repositories
{
    public interface ITenantAccessService
    {
        Task<bool> CanAccessTenant(string userId, string tenantId);
        Task<bool> IsInRole(string userId, string tenantId, params string[] roles);
    }

    public class TenantAccessService : ITenantAccessService
    {
        private readonly ITenantRepository tenantRepository;

        public TenantAccessService(ITenantRepository tenantRepository)
        {
            this.tenantRepository = tenantRepository;
        }

        public async Task<bool> CanAccessTenant(string userId, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(tenantId))
            {
                return false;
            }

            if (await tenantRepository.IsSuperUser(userId))
            {
                return true;
            }

            var role = await tenantRepository.GetUserRole(userId, tenantId);
            return !string.IsNullOrWhiteSpace(role);
        }

        public async Task<bool> IsInRole(string userId, string tenantId, params string[] roles)
        {
            if (await tenantRepository.IsSuperUser(userId))
            {
                return true;
            }

            var role = await tenantRepository.GetUserRole(userId, tenantId);
            if (string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            return roles.Any(r => string.Equals(r, role, StringComparison.OrdinalIgnoreCase));
        }
    }
}
