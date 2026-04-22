import axios, { type AxiosResponse } from 'axios';
import { getTenantHeaders } from './tenantHeaders';

export default class AuthService {
    public async register(username: string, password: string): Promise<{ success: boolean; message: string }> {
        try {
            const response: AxiosResponse<{ message?: string }> = await axios({
                url: '/api/v1/auth/register',
                method: 'POST',
                data: { username, password },
                withCredentials: true
            });

            return {
                success: response.status === 200,
                message: response.data?.message ?? 'Registered successfully.'
            };
        } catch {
            return {
                success: false,
                message: 'Registration failed.'
            };
        }
    }

    public async login(username: string, password: string): Promise<{ success: boolean; message: string }> {
        try {
            const response: AxiosResponse<{ message?: string }> = await axios({
                url: '/api/v1/auth/login',
                method: 'POST',
                data: { username, password },
                withCredentials: true
            });

            return {
                success: response.status === 200,
                message: response.data?.message ?? 'Logged in successfully.'
            };
        } catch {
            return {
                success: false,
                message: 'Login failed. Check username/password.'
            };
        }
    }

    public async me(): Promise<Me> {
        try {
            const response: AxiosResponse<Me> = await axios({
                url: '/api/v1/auth/me',
                method: 'GET',
                headers: {
                    ...getTenantHeaders()
                },
                withCredentials: true
            });

            return {
                authenticated: response.status === 200,
                username: response.data?.username ?? null,
                isSuperUser: response.data?.isSuperUser ?? false,
                currentTenantId: response.data?.currentTenantId ?? null,
                role: response.data?.role ?? null,
                tenants: Array.isArray(response.data?.tenants) ? response.data.tenants : []
            };
        } catch {
            return {
                authenticated: false,
                username: null,
                isSuperUser: false,
                currentTenantId: null,
                role: null,
                tenants: []
            };
        }
    }

    public async logout(): Promise<boolean> {
        try {
            const response: AxiosResponse<{ message?: string }> = await axios({
                url: '/api/v1/auth/logout',
                method: 'POST',
                withCredentials: true
            });

            return response.status === 200;
        } catch {
            return false;
        }
    }

    public async getTenants(): Promise<Tenant[]> {
        try {
            const response: AxiosResponse<Tenant[]> = await axios({
                url: '/api/v1/auth/tenants',
                method: 'GET',
                withCredentials: true
            });

            return Array.isArray(response.data) ? response.data : [];
        } catch {
            return [];
        }
    }

    public async createTenant(name: string): Promise<Tenant | null> {
        try {
            const response: AxiosResponse<Tenant> = await axios({
                url: '/api/v1/auth/tenants',
                method: 'POST',
                data: { name },
                withCredentials: true
            });

            return response.status === 200 ? response.data : null;
        } catch {
            return null;
        }
    }

    public async getTenantMembers(tenantId: string): Promise<TenantMember[]> {
        try {
            const response: AxiosResponse<TenantMember[]> = await axios({
                url: `/api/v1/auth/tenants/${tenantId}/members`,
                method: 'GET',
                withCredentials: true,
                headers: {
                    ...getTenantHeaders()
                }
            });
            return Array.isArray(response.data) ? response.data : [];
        } catch {
            return [];
        }
    }

    public async addTenantMember(tenantId: string, username: string, roleName: TenantRole): Promise<boolean> {
        try {
            const response: AxiosResponse = await axios({
                url: `/api/v1/auth/tenants/${tenantId}/members`,
                method: 'POST',
                data: { username, roleName },
                withCredentials: true,
                headers: {
                    ...getTenantHeaders()
                }
            });
            return response.status === 200;
        } catch {
            return false;
        }
    }

    public async updateTenantMemberRole(tenantId: string, memberUserId: string, roleName: TenantRole): Promise<boolean> {
        try {
            const response: AxiosResponse = await axios({
                url: `/api/v1/auth/tenants/${tenantId}/members/${memberUserId}`,
                method: 'PUT',
                data: { roleName },
                withCredentials: true,
                headers: {
                    ...getTenantHeaders()
                }
            });
            return response.status === 200;
        } catch {
            return false;
        }
    }
}
