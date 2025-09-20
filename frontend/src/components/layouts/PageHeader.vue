<template>
  <header class="header">
    <div class="side">
      <!-- <a class="btn_menu" @click="openMenu"></a> 
        <a class="btn_search" ></a> -->
      <img
        class="btn_logo"
        :style="element2Style"
        src="/src/assets/images/icons/VACYA.svg"
        @click="$router.push({path: '/'})"
      />
      <img
        @click.stop="toggleMenu"
        class="btn_search"
        src="/src/assets/images/icons/menu-icon.svg"
        onmouseover="this.src='/src/assets/images/icons/active-menu-icon.svg';"
        onmouseout="this.src='/src/assets/images/icons/menu-icon.svg';"
      />
      <img
        class="btn_search"
        src="/src/assets/images/icons/search-icon.svg"
        onmouseover="this.src='/src/assets/images/icons/active-search-icon.svg';"
        onmouseout="this.src='/src/assets/images/icons/search-icon.svg';"
      />
    </div>
    <div class="side">
      <img
        class="btn_notifications"
        src="/src/assets/images/icons/notifications-icon.svg"
        onmouseover="this.src='/src/assets/images/icons/active-notifications-icon.svg';"
        onmouseout="this.src='/src/assets/images/icons/notifications-icon.svg';"
      />
      <img
        class="btn_settings"
        src="/src/assets/images/icons/settings-icon.svg"
        onmouseover="this.src='/src/assets/images/icons/active-settings-icon.svg';"
        onmouseout="this.src='/src/assets/images/icons/settings-icon.svg';"
      />
      <!-- <a class="btn_notifications"></a> 
        <a class="btn_settings"></a> -->
      <button @click="$router.push({ path: '/profile' })" class="btn_avatar">А</button>
    </div>
  </header>
</template>

<script>
import { openBlock } from "vue";
import { mapState } from "vuex";
import { mapActions, mapMutations } from "vuex/dist/vuex.cjs.js";

// import openMenu from "/src/store/index.js";

export default {
  computed: {
    ...mapState({
      visibleMenu: (state) => state.visibleMenu,
    }),
    element2Style() {
      return {
        marginRight: this.visibleMenu ? '75px' : '0px',
      }}
  },
  methods: {
    ...mapMutations({
        toggleMenu: 'toggleMenu',
    }),
  }
}
</script>


<style lang="scss" scoped>
.header {
  height: 70px;
  background-color: rgb(255, 255, 255);
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  filter: drop-shadow(0px 4px 4px rgba(0, 0, 0, 0.25));
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
}

.side {
  display: flex;
  align-items: center;
  gap: 25px;
  margin-inline: 25px;
}

.btn_avatar {
  width: 40px;
  height: 40px;
  cursor: pointer;
  background-color: rgba(65, 140, 255, 0.5);
  border-radius: 100px;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 17px;
}
//я честно не бубу как по-другому эти ховеры мутить, сори. попробую сделать через js как-нибудь...
.btn_search {
  width: 28px;
  height: 28px;
  cursor: pointer;
  // background-image: url(/src/assets/images/icons/search-icon.svg);
}
// :hover.btn_search{
//         background-image: url(/src/assets/images/icons/active-search-icon.svg);
//         transition:cubic-bezier(1, 0, 0, 1), 200ms;
//     }

.btn_menu {
  width: 28px;
  height: 28px;
  cursor: pointer;
  // background-image: url(/src/assets/images/icons/menu-icon.svg)
}
// :hover.btn_menu{
//     background-image: url(/src/assets/images/icons/active-menu-icon.svg);
//     transition:cubic-bezier(1, 0, 0, 1), 200ms;
// }

.btn_notifications {
  width: 28px;
  height: 28px;
  cursor: pointer;
  // background-image: url(/src/assets/images/icons/notifications-icon.svg)
}
// :hover.btn_notifications{
//     background-image: url(/src/assets/images/icons/active-notifications-icon.svg);
//     transition:cubic-bezier(1, 0, 0, 1), 200ms;
// }

.btn_settings {
  width: 28px;
  height: 28px;
  cursor: pointer;
  // background-image: url(/src/assets/images/icons/settings-icon.svg)
}
// :hover.btn_settings{
//     background-image: url(/src/assets/images/icons/active-settings-icon.svg);
//     transition:cubic-bezier(1, 0, 0, 1), 200ms;
// }

.btn_logo{
  height: 38px;
  width: 150px;
  cursor: pointer;
}
</style>
