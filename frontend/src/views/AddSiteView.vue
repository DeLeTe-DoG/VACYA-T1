<template>
    <div class="window">
      <div class="window__header">
        <h2 class="window__title">Добавление сайта к отслеживанию</h2>
      </div>
      <div class="window__body">
        <form action="" class="form">
            <label for="siteName">
                Отображаемое имя сайта:
                <input class="main-input" v-model="siteData.name" type="text" name="siteName" id="siteName" placeholder="Введите имя сайта">
            </label>
            <label for="siteAdress">
                Адрес сайта:
                <input class="main-input" v-model="siteData.url" type="text" name="siteAdress" id="siteAdress" placeholder="Введите адрес сайта">
            </label>
            <label for="siteTiming">
                Частота проверок сайта:
                <select class="main-select" name="siteTiming" id="siteTiming">
                    <option value="5">каждые 5 сек.</option>
                    <option value="10">каждые 10 сек.</option>
                    <option value="15">каждые 15 сек.</option>
                </select>
            </label>
        </form>
        <div class="buttons">
            <main-button class="dis-btn" @click="$router.push({ path: '/' })">Отмена</main-button>
            <main-button :disabled="!siteData.url" @click="handleNewSite">Сохранить</main-button>
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
        gap: 25px;
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        label{
            display: flex;
            flex-direction: column;
            color: #969696;
            gap: 7px; 
        }
    }
    .buttons{
        display: flex;
        
    }
    .dis-btn{
        background-color: rgba(150, 150, 150, 1);
    }
    .main-btn{
        width: max-content;
        margin: 0 auto;
    }
</style>