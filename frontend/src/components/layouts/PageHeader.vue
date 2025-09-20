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
      <button class="exit-btn" @click="handleLogOut">
        <svg width="26" height="26" viewBox="0 0 26 26" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M13.0007 3.25C13.2768 3.25031 13.5424 3.35604 13.7431 3.54559C13.9439 3.73514 14.0647 3.9942 14.0809 4.26984C14.0971 4.54549 14.0074 4.81691 13.8302 5.02865C13.653 5.24039 13.4016 5.37647 13.1274 5.40908L13.0007 5.41667H7.58398C7.31864 5.4167 7.06254 5.51412 6.86425 5.69044C6.66596 5.86676 6.53928 6.10973 6.50823 6.37325L6.50065 6.5V19.5C6.50069 19.7653 6.5981 20.0214 6.77443 20.2197C6.95075 20.418 7.19371 20.5447 7.45723 20.5758L7.58398 20.5833H12.459C12.7351 20.5836 13.0007 20.6894 13.2015 20.8789C13.4022 21.0685 13.5231 21.3275 13.5393 21.6032C13.5554 21.8788 13.4658 22.1502 13.2885 22.362C13.1113 22.5737 12.8599 22.7098 12.5857 22.7424L12.459 22.75H7.58398C6.75501 22.75 5.95734 22.4333 5.3542 21.8646C4.75106 21.2959 4.38803 20.5182 4.3394 19.6907L4.33398 19.5V6.5C4.33394 5.67102 4.65067 4.87336 5.21937 4.27022C5.78808 3.66708 6.56577 3.30405 7.39332 3.25542L7.58398 3.25H13.0007ZM19.1832 9.16933L22.2469 12.2341C22.45 12.4372 22.5641 12.7127 22.5641 13C22.5641 13.2873 22.45 13.5628 22.2469 13.7659L19.1832 16.8307C18.98 17.0338 18.7043 17.1479 18.4169 17.1478C18.1296 17.1477 17.854 17.0334 17.6509 16.8301C17.4477 16.6268 17.3337 16.3512 17.3338 16.0638C17.3339 15.7764 17.4481 15.5009 17.6514 15.2978L18.8658 14.0833H13.0007C12.7133 14.0833 12.4378 13.9692 12.2346 13.766C12.0315 13.5629 11.9173 13.2873 11.9173 13C11.9173 12.7127 12.0315 12.4371 12.2346 12.234C12.4378 12.0308 12.7133 11.9167 13.0007 11.9167H18.8658L17.6514 10.7023C17.4481 10.4991 17.3339 10.2236 17.3338 9.93618C17.3337 9.6488 17.4477 9.37315 17.6509 9.16988C17.854 8.9666 18.1296 8.85234 18.4169 8.85224C18.7043 8.85214 18.98 8.9662 19.1832 9.16933Z" fill="black"/>
        </svg>
      </button>
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
    handleLogOut() {
      localStorage.removeItem('token')
      this.$router.push('/auth')
    }
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
.exit-btn{
  border: none;
  outline: none;
  background: transparent;
  cursor: pointer;
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
