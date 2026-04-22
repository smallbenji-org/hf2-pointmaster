import { defineStore } from "pinia";
import { computed, ref } from "vue";
import AuthService from "@/Services/AuthService";

export const useAuthStore = defineStore('Auth', () => {
    const authService = new AuthService();

    const Me = ref<Me>();
    const Members = ref<TenantMember[]>([]);
    const ActiveTenantId = ref<string | null>(localStorage.getItem('pm_tenant_id'));

    const ME = computed(() => Me.value);
    const TENANTS = computed(() => Me.value?.tenants ?? []);
    const ACTIVE_TENANT_ID = computed(() => ActiveTenantId.value ?? Me.value?.currentTenantId ?? null);
    const ROLE = computed(() => Me.value?.role ?? null);

    const IS_ADMIN = computed(() => {
        if (Me.value?.isSuperUser) {
            return true;
        }

        return ROLE.value === 'Administrator';
    });

    const CAN_GIVE_POINTS = computed(() => {
        if (Me.value?.isSuperUser) {
            return true;
        }

        return ROLE.value === 'Administrator' || ROLE.value === 'PostUser';
    });

    async function GET_ME() {
        const data = await authService.me()
        if (data.authenticated) {
            if (ActiveTenantId.value && data.tenants.some(t => t.id === ActiveTenantId.value)) {
                data.currentTenantId = ActiveTenantId.value;
            }

            if (!data.currentTenantId && data.tenants.length > 0) {
                data.currentTenantId = data.tenants[0].id;
            }

            if (data.currentTenantId) {
                ActiveTenantId.value = data.currentTenantId;
                localStorage.setItem('pm_tenant_id', data.currentTenantId);
            }
        }

        Me.value = data;

        return data;
    }

    async function LOGOUT() {
        const data = await authService.logout();
        ActiveTenantId.value = null;
        localStorage.removeItem('pm_tenant_id');
        await GET_ME();

        return data;
    }

    async function LOGIN(username: string, password: string) {
        const data = await authService.login(username, password);
        if (data.success) {
            await GET_ME();
        }

        return data;
    }

    async function REGISTER(username: string, password: string) {
        const data = await authService.register(username, password);
        if (data.success) {
            await GET_ME();
        }

        return data;
    }

    async function REFRESH_TENANTS() {
        const tenants = await authService.getTenants();
        if (Me.value) {
            Me.value.tenants = tenants;
        }
        return tenants;
    }

    function SET_ACTIVE_TENANT(tenantId: string) {
        ActiveTenantId.value = tenantId;
        localStorage.setItem('pm_tenant_id', tenantId);
        if (Me.value) {
            Me.value.currentTenantId = tenantId;
        }
    }

    async function CREATE_TENANT(name: string) {
        const tenant = await authService.createTenant(name);
        if (tenant) {
            await GET_ME();
            SET_ACTIVE_TENANT(tenant.id);
        }
        return tenant;
    }

    async function LOAD_MEMBERS() {
        const tenantId = ACTIVE_TENANT_ID.value;
        if (!tenantId) {
            Members.value = [];
            return [];
        }

        const members = await authService.getTenantMembers(tenantId);
        Members.value = members;
        return members;
    }

    async function ADD_MEMBER(username: string, roleName: TenantRole) {
        const tenantId = ACTIVE_TENANT_ID.value;
        if (!tenantId) {
            return false;
        }

        const ok = await authService.addTenantMember(tenantId, username, roleName);
        if (ok) {
            await LOAD_MEMBERS();
        }
        return ok;
    }

    async function UPDATE_MEMBER_ROLE(memberUserId: string, roleName: TenantRole) {
        const tenantId = ACTIVE_TENANT_ID.value;
        if (!tenantId) {
            return false;
        }

        const ok = await authService.updateTenantMemberRole(tenantId, memberUserId, roleName);
        if (ok) {
            await LOAD_MEMBERS();
        }
        return ok;
    }

    return {
        Me,
        ME,
        Members,
        TENANTS,
        ACTIVE_TENANT_ID,
        ROLE,
        IS_ADMIN,
        CAN_GIVE_POINTS,
        GET_ME,
        REGISTER,
        LOGIN,
        LOGOUT,
        SET_ACTIVE_TENANT,
        CREATE_TENANT,
        REFRESH_TENANTS,
        LOAD_MEMBERS,
        ADD_MEMBER,
        UPDATE_MEMBER_ROLE
    }
});