<script setup lang="ts">
</script>

<template>
  <router-view></router-view>
</template>

<style lang="scss">
:root {
  --primary: #007bff;
  --primary-header: #004080;
  --secondary-1: #f8f9fa;
  --secondary-2: #e9ecef;
  --secondary-3: #dee2e6;
  --secondary-4: #495057;
  --danger: #ff4d4d;

}

h1 {
  font-size: 24px;
  font-weight: 800;
  color: var(--primary-header)
}

h2 {
  font-size: 20px;
  font-weight: 600;
  color: var(--primary)
}

p {
  font-size: 14px;
}

.button {
  padding: 6px;
  border: solid 1px rgba(0, 0, 0, 0.2);
  box-shadow: 2px 2px 5px 0 rgba(0, 0, 0, 0.2);
  border-radius: 5px;
  cursor: pointer;
}
</style>