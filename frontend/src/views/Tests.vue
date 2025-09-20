<template>
    <div class="window">
      <div class="window__header">
        <h2 class="window__title">Планы и отчёты</h2>
      </div>
      <div class="window__body">
        <form action="" class="form">
            <label for="siteName">
                <input class="main-input" v-model="siteData.name" type="text" name="siteName" id="siteName" placeholder="Поиск">
            </label>
            <button class="btn-export">Экспорт</button>
            <main-button @click="$router.push({ path: '/tests/plan-test' })" >Добавить отчёт</main-button>
        </form>
        <div class="List">
            <h4>Статус</h4>
            <h4>Статус</h4>
            <h4>Статус</h4>
            <h4>Статус</h4>
            <h4>Статус</h4>
            <h4>Статус</h4>
            <h4>Статус</h4>
        </div>
      </div>
    </div>
</template>

<script>
import { mapActions } from 'vuex/dist/vuex.cjs.js';
import { sitesModule } from '../store/sitesModule';

    export default{
        data() {
            return{
                siteData: {
                    url: '',
                    name: '',
                }
            }
        },
        methods: {
            ...mapActions({
                addSite: 'sites/addSite'
            }),
            async handleNewSite() {
                try {
                    await this.addSite(this.siteData)
                } finally{
                    this.$router.push('/')
                }
            }
        }
    }
</script>

<style lang="scss" scoped>
    .form{
        display: flex;
        flex-direction: row;
        gap: 20px;

        label{
            display: flex;
            // flex-direction: column;
            color: #969696;
            gap: 7px; 
            width: 100%;
        }
    }
    .main-btn{
        width: max-content;
        display: inline;
        text-align: center;
        float: left;
    }
    .btn-export{
        cursor: pointer;
        height: 45px;
        font-size: 16px;
        color: #000000;
        padding: 0 30px;
        background-color: #ffffff;
        border: 2px solid #000;
        border-color: lightgray;
        border-radius: 8px;
        box-shadow: none;
        display: flex;
        align-items: center;
        justify-content: center;
    }
    .List{
        display: flex;
        gap: 25px;

    }
</style>