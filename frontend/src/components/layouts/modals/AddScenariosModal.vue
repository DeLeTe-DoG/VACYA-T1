<template>
    <div class="modal-wrapper">
        <div class="window modal">
            <div class="window__header">
                <h2 class="window__title">Добавление теста API</h2>
            </div>
            <div class="window__body">
                <form action="" class="form">
                    <label for="requestName">
                        Отображаемое имя теста:
                        <input class="main-input" type="text" name="siteName" id="siteName" placeholder="Введите имя сайта">
                    </label>
                    <label for="requesAdress">
                        Адрес сайта:
                        <input class="main-input" type="text" name="siteAdress" id="siteAdress" placeholder="Введите адрес сайта">
                    </label>
                    <label for="requestMethod">
                        Метод запроса:
                        <input class="main-input" type="text" name="siteAdress" id="siteAdress" placeholder="Введите адрес сайта">
                    </label>
                    <label for="requestType">
                        Заголовки:
                        <div class="row-wrapper">
                            <div class="textarea-wrapper">
                                <textarea name="requestType" placeholder='"Content-Type": "application/json"' id=""></textarea>
                            </div>
                            <div class="textarea-wrapper">
                                <textarea name="requestToken" placeholder='<token>' id=""></textarea>
                            </div>
                        </div>
                    </label>
                    <label for="requestJSON" class="checkbox-label">
                        <input type="checkbox" name="" id="">
                        Проверка JSON
                    </label>
                    <label for="requestXML" class="checkbox-label">
                        <input type="checkbox" name="" id="">
                        Проверка XML
                    </label>
                </form>
                <main-button>Сохранить</main-button>
            </div>
        </div>
    </div>
</template>

<script>
    export default {}
</script>

<style lang="scss" scoped>
    .modal-wrapper{
        width: 100vw !important;
        height: 100vh !important;
        position: fixed;
        top: 0;
        left: 0;
        z-index: 5;
        background-color: rgba(0, 0, 0, 0.4);
        display: flex;
        align-items: center;
        justify-content: center;
    }
    .modal{
        width: 50%;
    }
    .form{
        display: flex;
        flex-direction: column;
        gap: 25px;
        label{
            display: flex;
            flex-direction: column;
            color: #969696;
            gap: 7px; 
        }
        .checkbox-label{
            flex-direction: row;
            align-items: center;
            input{
                width: 20px;
                height: 20px;
            }
        }
    }
    .row-wrapper{
        display: flex;
        flex-direction: row;
        gap: 10px;
    }
</style>