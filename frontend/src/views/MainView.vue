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
          <div
            class="metric"
            v-if="
              sites && sites.find((site) => site.id == activeSite).dns
            "
          >
            <div class="metric-wrapper">
              <p class="metric__title">DNS</p>
              <p class="metric__description">
                Проверка наличия DNS
              </p>
            </div>
            <h2 class="metric__value">
              {{ sites.find((site) => site.id == activeSite).dns }}
            </h2>
          </div>
          <div
            class="metric"
            v-if="
              sites && sites.find((site) => site.id == activeSite).ssl
            "
          >
            <div class="metric-wrapper">
              <p class="metric__title">SSL</p>
              <p class="metric__description">
                Проверка наличия SSL
              </p>
            </div>
            <h2 class="metric__value">
              {{ sites.find((site) => site.id == activeSite).ssl }}
            </h2>
          </div>
        </div>
        <MainChart
          v-if="sites && activeSite"
          :chartData="[
            sites.find((site) => site.id == activeSite).webSiteData,
            sites.find((site) => site.id == activeSite).testsData,
          ]"
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
            <div
              class="choice-row__header"
              :style="{
                backgroundColor: site.isAvailable
                  ? ''
                  : 'rgba(255, 0, 0, 0.29)',
              }"
              @click="handleActiveSite(site.id)"
            >
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
              <button class="delete-btn" @click="deleteSite(activeSite)">
                <svg
                  width="24"
                  height="24"
                  viewBox="0 0 24 24"
                  fill="none"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    d="M18 5V19C18 19.5 17.5 20 17 20H12H7C6.5 20 6 19.5 6 19V5"
                    stroke="#969696"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                  <path
                    d="M4 5H20"
                    stroke="#969696"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                  <path
                    d="M10 4H14M10 9V16M14 9V16"
                    stroke="#969696"
                    stroke-width="2"
                    stroke-linecap="round"
                    stroke-linejoin="round"
                  />
                </svg>
              </button>
            </div>
            <div class="choice-row__body" v-if="site.id == activeSite">
              <div class="choice-row__header">
                <h5>Тесты API</h5>
                <main-button
                  class="small-btn"
                  @click="$refs.addTestModal.openModule()"
                  >Добавить</main-button
                >
              </div>
              <ul
                v-if="sites.find((site) => site.id == activeSite).testScenarios"
                class="api-test-list"
              >
                <li
                  v-for="scenarios in sites.find(
                    (site) => site.id == activeSite
                  ).testScenarios"
                >
                  <!-- {{ scenarios }} -->
                  <div class="api-test__row">
                    <h4 class="api-test__title">{{ scenarios.name }}</h4>
                    <button class="delete-btn" @click="deleteScenarios([activeSite, scenarios.name])">
                      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                      <path d="M18 5V19C18 19.5 17.5 20 17 20H12H7C6.5 20 6 19.5 6 19V5" stroke="#969696" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                      <path d="M4 5H20" stroke="#969696" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                      <path d="M10 4H14M10 9V16M14 9V16" stroke="#969696" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
                      </svg>
                    </button>
                  </div>
                  
                  <div class="api-test__row">
                    <p class="api-test__method">{{ scenarios.httpMethod }}</p>
                    <p v-if="scenarios.apiPath" class="api-test__adress">{{ scenarios.apiPath }}</p>
                  </div>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="window">
      <div class="window__header">
        <h2 class="window__title">История Проверок</h2>
      </div>
      <div class="window__body" :style="{ marginBottom: '50px' }">
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
              <tr
                v-if="sites && activeSite"
                v-for="error in sites.find((site) => site.id == activeSite)
                  .webSiteData"
              >
                <td>Работаспособность сайта</td>
                <td
                  class="status"
                  :class="error.errorMessage ? 'error' : 'success'"
                >
                  {{ error.statusCode }}
                </td>
                <td>{{ error.errorMessage ? error.errorMessage : "-" }}</td>
                <td>
                  {{ error.lastChecked.split("T")[0] }}
                  {{ error.lastChecked.split("T")[1].split(".")[0] }}
                </td>
                <td>{{ sites.find((site) => site.id == activeSite).url }}</td>
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
      <div class="window__header">
        <h2 class="window__title">Ошибки тестов API</h2>
      </div>
      <div
        class="window__body"
        v-if="
          sites &&
          activeSite &&
          sites.find((site) => site.id == activeSite).testsData[0]
        "
      >
        <div class="scroll-wrapper">
          <table v-if="sites && activeSite">
            <thead>
              <tr>
                <th>Имя проверки</th>
                <th>Статус</th>
                <th>Имя</th>
                <th>Дата и время</th>
                <th>URL-адрес</th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-if="sites.find((site) => site.id == activeSite).testsData[0]"
                v-for="error in sites.find((site) => site.id == activeSite)
                  .testsData"
              >
                <!-- {{ error }} -->
                <td>{{ error.name }}</td>
                <td
                  class="status"
                  :class="error.errorMessage ? 'error' : 'success'"
                >
                  {{ error.statusCode }}
                </td>
                <td>{{ error.errorMessage }}</td>
                <td>
                  {{ error.lastChecked.split("T")[0] }}
                  {{ error.lastChecked.split("T")[1].split(".")[0] }}
                </td>
                <td v-if="sites && sites.find((site) => site.id == activeSite).testScenarios.find((scen) => scen.name == error.name)">
                  {{
                    sites
                      .find((site) => site.id == activeSite)
                      .testScenarios.find((scen) => scen.name == error.name).apiPath
                  }}
                </td>
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
import { errorMessages } from "vue/compiler-sfc";

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
      deleteScenarios: 'sites/deleteScenarios',
      deleteSite: 'sites/deleteSite',
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
    this.getSites().then(() => {
      if(this.sites.length == 0) {
        this.$router.push('/add-site')
      }
    });
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
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 10px;
  border-radius: 16px;
  padding: 10px;
  background-color: #f5f5f5;
  .metric {
    width: 100%;
    background-color: #fff;
    padding: 20px 15px;
    border-radius: 12px;
    height: 180px;
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
    padding: 5px;
    border-radius: 4px;
    &.error-site {
      background-color: rgba(255, 0, 0, 0.29);
    }
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
    .api-test-list {
      padding: 0;
      list-style-type: none;
      display: flex;
      flex-direction: column;
      // gap: 20px;
      margin-top: 20px;
      li {
        padding: 10px 0;
        border-top: 1px solid #969696;
      }
      .api-test {
        &__title {
          margin-bottom: 10px;
        }
        &__row {
          display: flex;
          flex-direction: row;
          justify-content: space-between;
        }
        &__adress {
          color: #969696;
        }
      }
    }
  }
}
</style>
