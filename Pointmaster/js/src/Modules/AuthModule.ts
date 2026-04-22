import { defineStore } from "pinia";
import { computed, ref } from "vue";
import AuthService from "@/Services/AuthService";

export const useAuthStore = defineStore('Auth', () => {
    const authService = new AuthService();

    const Me = ref<Me>();

    const ME = computed(() => Me.value);

    async function GET_ME() {
        const data = await authService.me()
        Me.value = data;

        return data;
    }

    async function LOGOUT() {
        const data = await authService.logout();
        await GET_ME();

        return data;
    }

    async function LOGIN(username: string, password: string) {
        const data = await authService.login(username, password);

        return data;
    }

    async function REGISTER(username: string, password: string) {
        const data = await authService.register(username, password);

        return data;
    }

    return {
        Me,
        ME,
        GET_ME, REGISTER, LOGIN, LOGOUT
    }
});