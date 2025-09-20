<script>
import PageHeader from "./components/layouts/PageHeader.vue";
import Sidebar from "./components/layouts/Sidebar.vue";

// export default {}
import { mapState } from "vuex";

export default {
  computed: {
    ...mapState({
      visibleMenu: (state) => state.visibleMenu,
    }),
  },
  components: {
    PageHeader,
    Sidebar,
  },
};
//
</script>

<template class>
  <!-- <h1>Check this labubu!</h1> -->
  <PageHeader v-if="$route.path != '/auth'" />

  <div class="main_with_sidebar">
    <div v-if="$route.path != '/auth' && $route.path != '/'">
      <Sidebar v-if="visibleMenu" />
    </div>
    <div class="app-main">
      <RouterView></RouterView>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.main_with_sidebar {
  display: flex;
}
.app-main{
  width: 100% !important;
  height: calc(100vh - 70px);
  overflow-y: scroll;
  padding: 20px;
}
</style>
