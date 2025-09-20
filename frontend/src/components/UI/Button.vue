<template>
    <button class="main-btn"><slot></slot></button>
</template>

<script>
    export default {
        name: 'main-button', //объявляем имя компонента, это будет тег при его использовании.
    }
</script>

<style lang="scss">
    .main-btn{
        cursor: pointer;
        height: 45px;
        font-size: 16px;
        color: #fff;
        padding: 0 30px;
        background-color: #418CFF;
        border: none;
        outline: none;
        border-radius: 8px;
        display: flex;
        align-items: center;
        justify-content: center;
        &:disabled{
            background-color: #a3c3f5;
        }
    }
    .small-btn{
        height: 30px;
        padding: 0 15px;
    }
</style>