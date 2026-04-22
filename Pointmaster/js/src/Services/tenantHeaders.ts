export function getTenantId(): string | null {
    return localStorage.getItem('pm_tenant_id');
}

export function getTenantHeaders(): Record<string, string> {
    const tenantId = getTenantId();
    if (!tenantId) {
        return {};
    }

    return {
        'X-Tenant-Id': tenantId
    };
}
