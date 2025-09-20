<script>
import PageHeader from '../components/layouts/PageHeader.vue';
import Sidebar from '../components/layouts/Sidebar.vue';

// export default {}
import { mapState } from "vuex";

export default {
  computed: {
    ...mapState({
      visibleMenu: state => state.visibleMenu,
    }),
  },
};
</script>

<template class="Begin_body">
    <PageHeader />
    <Sidebar v-if="visibleMenu" />

</template>

<style lang="scss" scoped>


</style>
