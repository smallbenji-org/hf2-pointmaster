<template>
    <section class="auth-page">
        <div class="auth-card box">
            <h1 class="title is-4">Login</h1>
            <BField label="Username">
                <BInput v-model="username" autocomplete="username"></BInput>
            </BField>
            <BField label="Password">
                <BInput type="password" password-reveal v-model="password" autocomplete="current-password"></BInput>
            </BField>
            <BButton type="is-primary" :loading="loading" expanded @click="submitLogin">Log ind</BButton>
            <p class="status" :class="success ? 'ok' : 'error'" v-if="statusMessage">{{ statusMessage }}</p>
            <router-link class="auth-link" to="/register">Har du ikke en bruger? Registrer her</router-link>
        </div>
    </section>
</template>

<script lang="ts" setup>
import { useAuthStore } from '@/Modules/AuthModule';
import { BButton, BField, BInput } from 'buefy';
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const authModule = useAuthStore();
const router = useRouter();

const username = ref('');
const password = ref('');
const loading = ref(false);
const statusMessage = ref('');
const success = ref(false);

const submitLogin = async () => {
    if (!username.value || !password.value) {
        success.value = false;
        statusMessage.value = 'Udfyld både brugernavn og adgangskode.';
        return;
    }

    loading.value = true;
    const result = await authModule.LOGIN(username.value, password.value);
    loading.value = false;

    success.value = result.success;
    statusMessage.value = result.message;

    if (result.success) {
        await router.push('/');
    }
};
</script>

<style lang="scss" scoped>
.auth-page {
    min-height: 100%;
    display: grid;
    place-items: center;
    padding: 1rem;
}

.auth-card {
    width: min(420px, 100%);
}

.status {
    margin-top: 0.75rem;
}

.ok {
    color: #2f855a;
}

.error {
    color: #c53030;
}

.auth-link {
    display: inline-block;
    margin-top: 1rem;
}
</style>
