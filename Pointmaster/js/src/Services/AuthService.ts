import axios, { type AxiosResponse } from 'axios';

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
                withCredentials: true
            });

            return {
                authenticated: response.status === 200,
                username: response.data?.username ?? null
            };
        } catch {
            return {
                authenticated: false,
                username: null
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
}
