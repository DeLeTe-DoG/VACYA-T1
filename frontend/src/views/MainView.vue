<template>
  <div class="grid-structure">
    <div class="window">
      <div class="window__header">
        <h2 class="window__title">Общие данные</h2>
      </div>
      <div class="window__body">
        <div class="metrics-cards" v-if="activeSite">
          <div
            class="metric"
            v-if="
              sites && sites.find((site) => site.id == activeSite).totalErrors
            "
          >
            <div class="metric-wrapper">
              <p class="metric__title">Общее кол-во ошибок</p>
              <p class="metric__description">За последние 24 часа</p>
            </div>
            <h2 class="metric__value">
              {{ sites.find((site) => site.id == activeSite).totalErrors }}
            </h2>
          </div>
          <div
            class="metric"
            v-if="
              sites && sites.find((site) => site.id == activeSite).responseTime
            "
          >
            <div class="metric-wrapper">
              <p class="metric__title">Время отклика</p>
              <p class="metric__description">
                Время преобразования доменного имени в IP-адрес через DNS
              </p>
            </div>
            <h2 class="metric__value">
              {{ sites.find((site) => site.id == activeSite).responseTime }}
            </h2>
          </div>
        </div>
        <MainChart
          v-if="sites && activeSite"
          :chartData="sites.find((site) => site.id == activeSite).webSiteData"
        />
      </div>
    </div>
    <div class="column-wrapper">
      <div class="window" v-if="sites">
        <div class="window__header">
          <span>
            <h2 class="window__title">Отображаемые сайты</h2>
            <p class="window__subtitle">
              Выберите сайт, данные которого хотите увидеть
            </p>
          </span>

          <main-button
            class="add-site-btn"
            @click="$router.push({ path: '/add-site' })"
          >
            <svg
              width="16"
              height="16"
              viewBox="0 0 16 16"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M8 1V15M1 8H15"
                stroke="white"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
              />
            </svg>
          </main-button>
        </div>
        <div class="window__body">
          <div
            class="choice-row"
            v-for="site in sites"
            :class="site.id == activeSite ? 'active' : ''"
          >
            <div class="choice-row__header" @click="handleActiveSite(site.id)">
              <div class="choice-row__wrapper">
                <button
                  class="choice-row__open-btn"
                  :class="site.id == activeSite ? 'active' : ''"
                >
                  <svg
                    width="14"
                    height="8"
                    viewBox="0 0 14 8"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <path
                      fill-rule="evenodd"
                      clip-rule="evenodd"
                      d="M0.817185 0.31743C1.02034 0.114336 1.29584 0.000244141 1.5831 0.000244141C1.87036 0.000244141 2.14586 0.114336 2.34902 0.31743L6.99977 4.96818L11.6505 0.31743C11.8548 0.120092 12.1285 0.0108979 12.4125 0.0133662C12.6966 0.0158345 12.9683 0.129767 13.1692 0.330626C13.37 0.531485 13.4839 0.803198 13.4864 1.08724C13.4889 1.37129 13.3797 1.64494 13.1824 1.84926L7.76569 7.26593C7.56253 7.46902 7.28703 7.58311 6.99977 7.58311C6.71251 7.58311 6.43701 7.46902 6.23385 7.26593L0.817185 1.84926C0.614092 1.64611 0.5 1.37061 0.5 1.08335C0.5 0.796085 0.614092 0.520585 0.817185 0.31743Z"
                      fill="#969696"
                    />
                  </svg>
                </button>
                <div
                  class="choice-row__indicator"
                  :style="{
                    backgroundColor: site.isAvailable ? '#41C84A' : '#F62E2E',
                    width: '10px',
                    height: '10px',
                    borderRadius: '50%',
                  }"
                ></div>
                <p class="choice-row__name">{{ site.name }}</p>
              </div>
              <button class="edit-btn">
                <svg
                  width="20"
                  height="20"
                  viewBox="0 0 20 20"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M17.2266 2.30078L17.6992 2.77344C18.0664 3.14062 18.0664 3.73438 17.6992 4.09766L16.5625 5.23828L14.7617 3.4375L15.8984 2.30078C16.2656 1.93359 16.8594 1.93359 17.2227 2.30078H17.2266ZM8.19531 10.0078L13.4375 4.76172L15.2383 6.5625L9.99219 11.8047C9.87891 11.918 9.73828 12 9.58594 12.043L7.30078 12.6953L7.95312 10.4102C7.99609 10.2578 8.07812 10.1172 8.19141 10.0039L8.19531 10.0078ZM14.5742 0.976562L6.86719 8.67969C6.52734 9.01953 6.28125 9.4375 6.15234 9.89453L5.03516 13.8008C4.94141 14.1289 5.03125 14.4805 5.27344 14.7227C5.51562 14.9648 5.86719 15.0547 6.19531 14.9609L10.1016 13.8438C10.5625 13.7109 10.9805 13.4648 11.3164 13.1289L19.0234 5.42578C20.1211 4.32812 20.1211 2.54688 19.0234 1.44922L18.5508 0.976562C17.4531 -0.121094 15.6719 -0.121094 14.5742 0.976562ZM3.4375 2.5C1.53906 2.5 0 4.03906 0 5.9375V16.5625C0 18.4609 1.53906 20 3.4375 20H14.0625C15.9609 20 17.5 18.4609 17.5 16.5625V12.1875C17.5 11.668 17.082 11.25 16.5625 11.25C16.043 11.25 15.625 11.668 15.625 12.1875V16.5625C15.625 17.4258 14.9258 18.125 14.0625 18.125H3.4375C2.57422 18.125 1.875 17.4258 1.875 16.5625V5.9375C1.875 5.07422 2.57422 4.375 3.4375 4.375H7.8125C8.33203 4.375 8.75 3.95703 8.75 3.4375C8.75 2.91797 8.33203 2.5 7.8125 2.5H3.4375Z"
                    fill="#969696"
                  />
                </svg>
              </button>
            </div>
            <div class="choice-row__body" v-if="site.id == activeSite">
              <div class="choice-row__header">
                <h5>Тесты API</h5>
                <main-button class="small-btn" @click="$refs.addTestModal.openModule()">Добавить</main-button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="window">
      <div class="window__header">
        <h2 class="window__title">История ошибок</h2>
      </div>
      <div class="window__body">
        <div class="scroll-wrapper">
          <table v-if="sites && activeSite">
            <thead>
              <tr>
                <th>Тип проверки</th>
                <th>Статус</th>
                <th>Имя</th>
                <th>Дата и время</th>
                <th>URL-адрес</th>
              </tr>
            </thead>
            <tbody>
              <tr v-if="sites" v-for="error in sites.find(site => site.id == activeSite).webSiteData">
                <td>Работаспособность сайта</td>
                <td class="status success">{{ error.statusCode }}</td>
                <td>{{ error.errorMessage }}</td>
                <td>{{ error.lastChecked.split('T')[0] }} {{ error.lastChecked.split('T')[1].split('.')[0] }}</td>
                <td>{{ sites.find(site => site.id == activeSite).url }}</td>
              </tr>
              <!-- <tr>
                <td>Post-Get-Anal-Manal-Zaebal</td>
                <td class="status error">Ошибка 304</td>
                <td>Азат Сагдетдинов</td>
                <td>20.09.2025 13:20:15</td>
                <td>/IAmNotGood</td>
              </tr>
              <tr>
                <td>Get</td>
                <td class="status mega-error">Ошибка 404</td>
                <td>Владислав Григорьев</td>
                <td>20.09.2025 13:20:15</td>
                <td>/BackSideMaster</td>
              </tr> -->
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
  <AddScenariosModal ref="addTestModal" />
</template>

<script>
import { mapActions, mapMutations, mapState } from "vuex/dist/vuex.cjs.js";
import MainChart from "../components/layouts/main-page/MainChart.vue";
import AddScenariosModal from "../components/layouts/modals/AddScenariosModal.vue";

export default {
  components: { MainChart, AddScenariosModal },
  data() {
    return {
      token: null,
      user_data: null,
      userData: null,
      addTestModal: false,
      activeSite: null,
      // chartData: null,
      metrics: [
        // {
        //   id: 1,
        //   indicator: "#418DDF",
        //   title: "Total time",
        //   value: "3000",
        //   measure: "ms",
        //   description: "Итоговое время загрузки страницы",
        // },
        // {
        //   id: 1,
        //   title: "Общее кол-во ошибок",
        //   value: null,
        //   description: "За последние 24 часа",
        // },
        // {
        //   id: 2,
        //   title: "Время отклика",
        //   value: "1000",
        //   description:
        //     "Время преобразования доменного имени в IP-адрес через DNS",
        // },
        // {
        //   id: 3,
        //   title: "Работаспособность API",
        //   children: [
        //     {
        //       id: 1,
        //       date: "10-09-2025",
        //       code: 404,
        //       message: "server not found",
        //     },
        //     {
        //       id: 2,
        //       date: "11-09-2025",
        //       code: 200,
        //       message: "alive",
        //     },
        //   ],
        // },
      ],
    };
  },
  computed: {
    ...mapState({
      sites: (state) => state.sites.sites,
      // activeSite: (state) => state.sites.activeSite,
    }),
    // chartData() {
    //   if(this.sites) {
    //     return this.sites.find(site => site.id == this.$route.query.project).webSiteData
    //   }
    // }
  },
  methods: {
    ...mapActions({
      getSites: "sites/getSites",
    }),
    ...mapMutations({
      setActiveSite: "sites/setActiveSite",
    }),
    handleActiveSite(site_id) {
      const currentPath = this.$route.path;
      this.$router.push({ path: currentPath, query: { project: site_id } });
    },
  },
  mounted() {
    this.token = localStorage.getItem("token");
    this.getSites();
    this.activeSite = this.$route.query.project;
  },
  watch: {
    "$route.query.project": {
      immadiate: true,
      handler(newVal) {
        this.activeSite = newVal;
        // this.getSites()
        // this.chartData = this.sites.find(site => site.id == newVal).webSiteData
      },
    },
  },
};
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
  max-width: max-content;
  border-collapse: collapse;
  margin-bottom: 20px;
  position: relative;
}

th,
td {
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
.mega-error {
  background-color: #f8d7da;
  color: #721c24;
}

@media (max-width: 768px) {
  th,
  td {
    padding: 8px;
    font-size: 14px;
  }
}

.metrics-cards {
  display: flex;
  flex-direction: row;
  gap: 10px;
  border-radius: 16px;
  padding: 10px;
  background-color: #d9d9d9;
  height: 200px;
  .metric {
    width: 100%;
    background-color: #fff;
    padding: 20px 15px;
    border-radius: 12px;
    height: 100%;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    &__marker {
      width: 70px;
      height: 70px;
      border-radius: 50%;
    }
    &__title {
      font-size: 24px;
      margin-bottom: 10px;
    }
    &__value {
      font-size: 42px;
    }
    &__description {
      font-size: 15px;
      color: #969696;
    }
    &__list {
      overflow-y: scroll;
      height: max-content;
      max-height: 100%;
      ul {
        padding: 0;
        list-style-type: none;
        li {
          display: flex;
          flex-direction: row;
          align-items: center;
          gap: 18px;
          p {
            font-size: 20px;
          }
        }
      }
    }
  }
}
.add-site-btn {
  padding: unset;
  aspect-ratio: 1/1;
}

.choice-row {
  padding: 20px;
  padding-top: 20px;
  border-top: 1px solid #f5f5f5;
  border-radius: 8px;
  &.active {
    background-color: #f5f5f5;
  }
  &__header {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
    font-size: 20px;
  }
  &__wrapper {
    display: flex;
    flex-direction: row;
    align-items: center;
    gap: 10px;
  }
  &__open-btn {
    border: none;
    background: transparent;
    outline: none;
    svg {
      transition: all 0.2s ease-in;
    }
    &.active svg {
      transform: rotate(180deg);
    }
  }
  &__body {
    padding-top: 20px;
  }
}
</style>
