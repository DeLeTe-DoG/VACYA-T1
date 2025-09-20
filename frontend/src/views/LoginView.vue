<template>
  <div class="auth-wrapper">
    <div class="register-form">
      <h2>{{ switchToRegister ? "Регистрация" : "Вход в систему" }}</h2>
      <form v-if="switchToRegister">
        <input v-model="userData.Name" type="text" placeholder="Имя" required />
        <input
          v-model="userData.Email"
          type="email"
          placeholder="Email"
          required
        />
        <input
          v-model="userData.Password"
          type="password"
          placeholder="Пароль"
          required
        />
        <button type="button" :disabled="loading" @click="sendRegister()">
          {{ loading ? "Регистрация..." : "Зарегистрироваться" }}
        </button>
        <div>
          <p style="margin-top: 8px;">Я соглашаюсь с <span @click="">политикой конфидециальности</span></p>
        </div>
      </form>
      <form @submit.prevent="handleLogin" v-else>
        <input
          v-model="userData.Name"
          type="text"
          placeholder="Имя пользователя"
          required
        />
        <input
          v-model="userData.Email"
          type="email"
          placeholder="Email"
          required
        />
        <input
          v-model="userData.Password"
          type="password"
          placeholder="Пароль"
          required
        />
        <button type="submit" @click="sendLogin()" :disabled="loading">
          {{ loading ? "Вход..." : "Войти" }}
        </button>
      </form>

      <div v-if="errorMessage" class="error-message">
        {{ errorMessage }}
      </div>

      <p class="login-link" v-if="switchToRegister">
        Есть аккаунт? <span @click="switchToRegister = false">Войти</span>
      </p>
      <p class="login-link" v-else>
        Нет аккаунта? <span @click="switchToRegister = true">Зарегистрироваться</span>
      </p>

    </div>

    <img class="picture" src="/src/assets/images/cool-image-from-behind.png" />
  </div>
</template>

<script>
// import axios from "axios";
import { mapActions } from "vuex";

export default {
  data() {
    return {
      userData: {
        Name: "",
        Email: "",
        Password: "",
      },
    //   errorMessage: "",
    //   loading: false,
      switchToRegister: false,
    };
  },
  methods: {
    ...mapActions({
        handleRegister: 'auth/handleRegister',
        handleLogin: 'auth/handleLogin',
    }),
    sendRegister() {
        const data = this.userData
        this.handleRegister(data)
    },
    sendLogin() {
        const data = this.userData
        this.handleLogin(data)
    },
  },
};
</script>

<style lang="scss" scoped>
.auth-wrapper {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  gap: 340px;
  margin-left: 340px;
}

.picture {
  width: 27vw;
  height: 96vh;
  border-radius: 30px;
  margin-block: 2vh;
  margin-right: 2vh;
  // display: flex;
  // align-items: center;
  // justify-items: end;
}

.register-form {
  max-width: 400px;
  margin: 50px auto;
  padding: 30px;
  border: 1px solid #ddd;
  border-radius: 10px;
  background: white;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  height: max-content;
}
h2 {
  margin-bottom: 30px;
  color: #333;
}

input {
  width: 100%;
  padding: 12px;
  margin: 10px 0;
  border: 1px solid #ddd;
  border-radius: 5px;
  font-size: 16px;
  box-sizing: border-box;
}

input:focus {
  outline: none;
  border-color: #2196f3;
}

button {
  width: 100%;
  padding: 12px;
  background: #2196f3;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 16px;
  cursor: pointer;
  margin-top: 15px;
}

button:hover:not(:disabled) {
  background: #1976d2;
}

button:disabled {
  background: #ccc;
  cursor: not-allowed;
}

.error-message {
  color: #d32f2f;
  background: #ffebee;
  padding: 12px;
  border-radius: 5px;
  margin: 15px 0;
  border: 1px solid #ffcdd2;
}

.login-link {
  text-align: center;
  margin-top: 20px;
  color: #666;
  span {
    color: #2196f3;
    cursor: pointer;
    // text-decoration: none;
    &:hover {
      text-decoration: underline;
    }
  }
}
</style>
