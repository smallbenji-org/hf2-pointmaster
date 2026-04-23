<script setup lang="ts">
import TenantPicker from './tenantPicker.vue';

</script>

<template>
    <footer class="custom-footer has-text-grey-light">
        v0.0.1
        <TenantPicker></TenantPicker>
    </footer>
</template>
<style lang="scss">
.custom-footer {
    border-top: 1px solid rgba(0, 0, 0, 0.1);
    padding: 1rem;
    display: flex;
    justify-content: space-between;
}
</style>