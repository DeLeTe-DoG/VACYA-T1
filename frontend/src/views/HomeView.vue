<template>
    <div class="cards-wrapper">
        <div class="site-card" v-if="sites" v-for="site in sites" @click="openDashboard(site.id)">
            <h3 class="site-card__name">{{ site.name }}</h3>
            <p class="site-card__status" :style="{color: site.isAvailable ? '#site.isAvailable' : '#F62E2E'}">{{ site.isAvailable ? 'Доступен' : 'Не доступен' }}</p>
            <h2 class="site-card__errors">{{ site.totalErrors }}</h2>
        </div>
        <button class="site-card add-card" @click="$router.push({ path: '/add-site' })">
            <span>+</span>
            Добавить
        </button>
    </div>
    <div class="charts-wrapper">
        <MainChart v-for="site in sites" :chartData="site.webSiteData" />
    </div>
</template>

<script>
    import MainChart from "../components/layouts/main-page/MainChart.vue";
    import { mapActions, mapMutations, mapState } from "vuex/dist/vuex.cjs.js";
    export default{
        components: { MainChart },
        computed: {
            ...mapState({
                sites: (state) => state.sites.sites,
            }),
        },
        methods: {
            ...mapActions({
            getSites: "sites/getSites",
            }),
            ...mapMutations({
                setActiveSite: 'sites/setActiveSite',
            }),
            openDashboard(site_id) {
                this.$router.push({
                    path: '/dashboard',
                    query: {
                        project: site_id
                    }
                })
                this.setActiveSite(site_id)
            }
        },
        mounted() {
            this.token = localStorage.getItem("token");
            this.getSites();
            console.log(this.sites)
        },
    }
</script>

<style lang="scss" scoped>
    .cards-wrapper{
        display: flex;
        flex-direction: row;
        justify-content: flex-start;
        gap: 10px;
        .site-card{
            cursor: pointer;
            width: 100%;
            max-width: 300px;
            height: 180px;
            border: none;
            outline: none;
            background-color: #fff;
            border-radius: 12px;
            padding: 15px;
            &__name{
                font-size: 24px;
            }
            &__status{
                margin: 10px 0 30px 0;
            }
            &__errors{
                font-size: 40px;
            }
            &.add-card{
                background-color: #7AAFFF;
                font-size: 32px;
                color: #fff;
                display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
                span{
                    font-size: 80px;
                    color: #fff;
                    line-height: 50%;
                }
            }
        }
    }
    .charts-wrapper{
        display: grid;
        grid-template-columns: repeat(2, 1fr);
    }
</style>