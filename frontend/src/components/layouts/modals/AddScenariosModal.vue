<template>
  <div class="modal-wrapper" v-if="isActiveModal" @click.self="closeModal">
    <div class="window modal">
      <div class="window__header">
        <h2 class="window__title">Добавление теста API</h2>
      </div>
      <div class="window__body">
        <form action="" class="form">
          <label for="requestName">
            Отображаемое имя теста:
            <input
              class="main-input"
              type="text"
              name="requestName"
              id="requestName"
              placeholder="Введите имя сайта"
              v-model="scenariosBody.name"
            />
          </label>
          <label for="requestMethod">
            Метод запроса:
            <input
              class="main-input"
              type="text"
              name="requestMethod"
              id="requestMethod"
              placeholder="Введите адрес сайта"
              v-model="scenariosBody.httpMethod"
            />
          </label>
          <label for="requestType">
            Тело запроса:
            <div class="row-wrapper">
              <div class="textarea-wrapper">
                <textarea
                  name="requestType"
                  placeholder='"Content-Type": "application/json"'
                  id=""
                  v-model="scenariosBody.body"
                ></textarea>
              </div>
            </div>
          </label>
          <label for="requestMethod">
            Ожидаемыq контент:
            <input
              class="main-input"
              type="text"
              name="requestMethod"
              id="requestMethod"
              placeholder="Введите адрес сайта"
              v-model="scenariosBody.expectedContent"
            />
          </label>
          <label for="requestJSON" class="checkbox-label">
            <input type="checkbox" name="requestJSON" id="requestJSON" v-model="scenariosBody.checkJSON" />
            Проверка JSON
          </label>
          <label for="requestXML" class="checkbox-label">
            <input type="checkbox" name="requestXML" id="requestXML" v-model="scenariosBody.checkXML" />
            Проверка XML
          </label>
        </form>
        <main-button @click="addScenarios([$route.query.project, scenariosBody])">Сохранить</main-button>
      </div>
    </div>
  </div>
</template>

<script>
import { mapActions } from 'vuex'
export default {
  data() {
    return {
      isActiveModal: false,
      scenariosBody: {
        name: "test scen",
        httpMethod: "GET",
        body: "url: 'https://vacya.netlify.app'",
        checkXML: true,
        checkJSON: true,
        expectedContent: "OK",
      },
    };
  },
  methods: {
    ...mapActions({
        addScenarios: 'sites/addScenarios'
    }),
    openModule() {
      this.isActiveModal = true;
    },
    closeModal() {
      this.isActiveModal = false;
    },
  },
};
</script>

<style lang="scss" scoped>
.modal-wrapper {
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
.modal {
  width: 50%;
}
.form {
  display: flex;
  flex-direction: column;
  gap: 25px;
  label {
    display: flex;
    flex-direction: column;
    color: #969696;
    gap: 7px;
  }
  .checkbox-label {
    flex-direction: row;
    align-items: center;
    input {
      width: 20px;
      height: 20px;
    }
  }
}
.row-wrapper {
  display: flex;
  flex-direction: row;
  gap: 10px;
}
</style>
