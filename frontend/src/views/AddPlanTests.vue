<template>
    <div class="window">
      <div class="window__header">
        <h2 class="window__title">Добавление планов и отчётов</h2>
      </div>
      <div class="window__body">
        <form action="" class="form">
            <div class="up-block"> 
                <h3>Статус:</h3>
                <label for="siteName">
                    <input class="main-input" v-model="siteData.name" type="text" name="siteName" id="siteName" placeholder="В планах">
                </label>
            </div>
            <div class="up-block">
                <h3>Дата просрочки плана:</h3>
                <label for="siteAdress">
                    <input class="main-input" v-model="siteData.url" type="text" name="siteAdress" id="siteAdress" placeholder="18.09.2025">
                </label>
            </div>
            <div class="down-block">
                <h3>Результат:</h3>
                <label for="siteAdress" style="padding-top: 10px;">
                    <input style="height: 150px;" class="main-input" v-model="siteData.url" type="text" name="siteAdress" id="siteAdress" placeholder="Введите результат">
                </label>
            </div>
            <div class="down-block">
                <h3>Комментарий:</h3>
                <label for="siteAdress" style="padding-top: 10px;">
                    <input style="height: 150px;" class="main-input" v-model="siteData.url" type="text" name="siteAdress" id="siteAdress" placeholder="Введите комментарий">
                </label>
            </div>

            <!-- <label for="siteTiming">
                <select class="main-select" name="siteTiming" id="siteTiming">
                    <option value="5">каждые 5 сек.</option>
                    <option value="10">каждые 10 сек.</option>
                    <option value="15">каждые 15 сек.</option>
                </select>
            </label> -->
        </form>
        <div class="buttons">
            <main-button class="dis-btn" @click="$router.push({ path: '/' })">Отмена</main-button>
            <main-button :disabled="!siteData.url" @click="handleNewSite">Добавить</main-button>
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
        flex-direction: column;
        gap: 35px;
        label{
            display: flex;
            flex-direction: column;
            color: #969696;
            gap: 7px;
        }
    }
    .buttons{
        display: flex;
        padding-top: 35px;
    }
    .dis-btn{
        background-color: rgba(150, 150, 150, 1);
    }
    .main-btn{
        width: max-content;
        margin: 0 auto;
    }
    .up-block{
        display: flex;
        align-items: center;
        gap: 20px;
    }
</style>