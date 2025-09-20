import { api } from "../api";
import axios from "axios";
import { router } from "../router";

export const authModule = {
  namespaced: true,
  // state - состояния, тут храним динамические переменные (статичные какие-нибудь будем хранить просто в data компонента)
  state: {
    loading: false,
    errorMessage: "",
    userData: null,
  },
  // mutations - прост мини функции для изменения стейта
  // можно конечно менять просто через state.loading например, но принято юзать мутации
  mutations: {
    setLoading(state, bool) {
      state.loading = bool
    },
    setErrorMessage(state, data) {
      state.errorMessage = data
    },
    setUserData(state, data) {
      state.userData = data
    }
  },
  // actions - функции для взаимодействия с состояниями, в основном тут всякие запросы.
  // простенькие функции с обработкой статики пишем просто в методах компонентов
  actions: {
    async handleRegister({ state, commit }, data) {
      try {
        commit('setLoading', true)        //коммиты нужны для обращения к мутациям, первый параментр - имя мутации, второй - данные, которые отправляем в мутацию
        commit('setErrorMessage', '')

        console.log("Регистрирую:", data);

        const response = await axios.post(`${api}/api/user/register`, data);
        console.log("Регистрация успешна:", response.data);
        console.log(response.data)

        const token = response.data.token;

        if (!token) {
          throw new Error("Токен не получен");
        }

        localStorage.setItem("token", token);     // localStorage - типо кеширование данных, они хранятся ни в проекте, а в браузере ваще вроде, удобно если нужно запомнить какие-то данные чтоб затем обратиться к ним с любой точки проекта через localStorage.getItem('token)

        commit('setUserData', response.data.user)
        localStorage.setItem('userData', JSON.stringify(response.data.user))

        router.replace({ path: "/" });      //этот метод заменяет адрес в урле на "/".    в .vue компонентах пишется по другому: this.$router.push('/')
      } catch (error) {
        console.error("Ошибка регистрации:", error);

        if (error.response?.status === 409) {
          commit('setErrorMessage', "Пользователь уже существует")
        } else if (error.response?.status === 400) {
          commit('setErrorMessage', "Неверные данные")
        } else {
          commit('setErrorMessage', error.response?.data || error.message || "Ошибка регистрации")
        }
      } finally {
        commit('setLoading', false)
      }
    },
    async handleLogin({ state, commit }, data) {
      console.log(data)
      try {
        commit('setLoading', true)
        commit('setErrorMessage', '')

        console.log("Пытаюсь войти с:", data);

        const response = await axios.post(
          `${api}/api/user/login`,
          data
        );
        console.log("Ответ сервера:", response.data);

        const token = response.data.token;
        localStorage.setItem('userData', JSON.stringify(response.data.user))
        if (!token) {
          throw new Error("Токен не получен от сервера");
        }

        // Сохраняем токен
        localStorage.setItem("token", token);
        console.log("Токен сохранен");

        // Сохраняем основные данные пользователя
        commit('setUserData', response.data.user)

        // Перенаправляем в профиль
        router.replace({path: "/"});
      } catch (error) {
        console.error("Ошибка входа:", error);

        if (error.response?.status === 401) {
          commit('setErrorMessage', "Неверное имя пользователя или пароль")
        } else if (error.response?.status === 404) {
          commit('setErrorMessage', "Сервер не доступен. Попробуйте позже.")
        } else if (error.code === "NETWORK_ERROR") {
          commit('setErrorMessage', "Нет соединения с сервером")
        } else {
          commit('setErrorMessage', "error.response?.data || error.message || 'Ошибка входа'")            
        }
      } finally {
        commit('setLoading', false)
      }
    },
  },
};
