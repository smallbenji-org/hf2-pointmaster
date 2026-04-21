<script setup lang="ts">
import { BNavbar, BNavbarItem } from 'buefy';

</script>

<template>
    <b-navbar :shadow="true">
        <template #brand>
            <b-navbar-item tag="router-link" :to="{ path: '/' }">
                <h1 class="title">
                    PointMaster
                </h1>
            </b-navbar-item>
        </template>
        <template #end>
            <b-navbar-item tag="div">
                <router-link to="/patruljer" class="button is-primary">
                    Patruljer
                </router-link>
            </b-navbar-item>
            <b-navbar-item tag="div">
                <router-link to="/poster" class="button is-primary">
                    Poster
                </router-link>
            </b-navbar-item>
            <b-navbar-item tag="div">
                <router-link to="/point" class="button is-primary">
                    Point
                </router-link>
            </b-navbar-item>
        </template>
    </b-navbar>
</template>
<style lang="scss">
// .topbar {
//     // border-bottom: 1px solid black;
//     box-shadow: 0 5px 5px 0 rgba(0, 0, 0, 0.2);
//     padding: 1rem;
//     display: flex;
//     z-index: 1;
//     a {
//         // color: black;
//         text-decoration: none;
//     }

//     &-links {
//         margin-left: auto;
//         display: flex;
//         gap: 5px;
//     }
// }
</style>