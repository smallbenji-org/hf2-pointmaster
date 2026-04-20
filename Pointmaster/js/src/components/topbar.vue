<template>
    <div class="topbar">
        <router-link to="/">
            PointMaster
        </router-link>
        <div class="topbar-links">
            <router-link to="/patruljer">
                Patruljer
            </router-link>
        </div>
    </div>
</template>
<style lang="scss">
.topbar {
    // border-bottom: 1px solid black;
    box-shadow: 0 5px 5px 0 rgba(0, 0, 0, 0.2);
    padding: 1rem;
    display: flex;
    a {
        color: black;
        text-decoration: none;
    }

    &-links {
        margin-left: auto;
        display: flex;
        gap: 5px;
    }
}
</style>