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
            <div class="error-history">
                <table>
                    <thead>
                        <tr>
                            <th>СТАТУС</th>
                            <th>ДАТА И ВРЕМЯ</th>
                            <th>ИСПОЛНИТЕЛЬ</th>
                            <th>РЕЗУЛЬТАТ</th>
                            <th>КОММЕНТАРИЙ</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="status success">Успешно пройдена</td>
                            <td>20.09.2025 13:19:10</td>
                            <td>Аяз Кайзер</td>
                            <td>Запрос выполнен</td>
                            <td>Надо починить фронт...</td>
                        </tr>
                        <tr>
                            <td class="status error">Запланировано</td>
                            <td>20.09.2025 13:20:15</td>
                            <td>Азат Сагдетдинов</td>
                            <td>Страница не найдена</td>
                            <td>Надо поменять дизайн...</td>
                        </tr>
                        <tr>
                            <td class="status mega-error">Просрочено</td>
                            <td>20.09.2025 13:20:15</td>
                            <td>Владислав Григорьев</td>
                            <td>Запрос выполнен</td>
                            <td>Надо починить бэк...</td>
                        </tr>
                    </tbody>
                </table>
            </div>
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
.error-history {
    // max-width: 100%;
    // margin: 20px auto;
    // font-family: Arial, sans-serif;
    width: 100%;
//   max-height: 400px; /* задаем максимальную высоту */
//   overflow: auto; /* добавляем прокрутку при необходимости */
//   border: 1px solid #ccc;
  position: relative;
}

table {
    max-width: 130%;
    border-collapse: collapse;
    margin-bottom: 20px;    
    position: relative;
}

th, td {
    padding: 12px;
    text-align: left;
    // border: 1px solid #ddd;
    word-break: break-word; /* переносим слова при необходимости */
    white-space: nowrap; /* предотвращаем перенос текста */
    overflow: hidden; /* обрезаем текст */
    text-overflow: ellipsis; /* добавляем многоточие при обрезке */
    max-width: 400px; /* равное распределение ширины */

}


th {
    // background-color: #f2f2f2;   
    font-weight: bold;
    color: #969696;
    font-size: large;
}

// tr:nth-child(even) {
//     background-color: #f9f9f9;
// }

.status {
    padding: 4px 8px;
    border-radius: 4px;
    font-weight: bold;
}
.success {
    background-color: #d4edda;
    color: #198754;
}

.error {
    background-color: #f8e5d7;
    color: #72551c;
}
.mega-error{
    background-color: #f8d7da;
    color: #721c24;
}

@media (max-width: 768px) {
    th, td {
        padding: 8px;
        font-size: 14px;
    }
}




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