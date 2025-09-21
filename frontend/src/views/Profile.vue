<template>
  <div class="profile">
    <h2>Мой профиль</h2>
    {{ userData }}    
    <div v-if="userData" class="user-info">
      <div v-if="userData.name" class="info-item">
        <span class="label">Имя:</span>
        <span class="value">{{ userData.name }}</span>
      </div>
      
      <div v-if="userData.email" class="info-item">
        <span class="label">Email:</span>
        <span class="value">{{ userData.email }}</span>
      </div>
      
      <!-- <div v-if="user.loginTime" class="info-item">
        <span class="label">Вход выполнен:</span>
        <span class="value">{{ formatDate(user.loginTime) }}</span>
      </div>
      
      <div v-if="user.registrationTime" class="info-item">
        <span class="label">Зарегистрирован:</span>
        <span class="value">{{ formatDate(user.registrationTime) }}</span>
      </div> -->
    </div>
    
    <div v-else class="no-data">
      <p>Данные пользователя не найдены</p>
    </div>

    <div class="actions">
      <button @click="refreshData" class="refresh-btn">Обновить</button>
      <button @click="handleLogout" class="logout-btn">Выйти</button>
    </div>
  </div>
</template>

<script>
import { mapActions, mapState } from 'vuex'
export default {
  data() {
    return {
      user: null,
      loading: false
    };
  },
  computed: {
    ...mapState({
      userData: state => state.auth.userData,
    }),
  },
  mounted() {
    // this.getUserData();
  },
  methods: {
    // ...mapActions({
    //   getUserData: 'getUserData',
    // }),
    // loadUserData() {
    //   try {
    //     const userData = localStorage.getItem('user');
    //     if (userData) {
    //       this.user = JSON.parse(userData);
    //       console.log('Данные пользователя:', this.user);
    //     } else {
    //       console.log('Данные не найдены в localStorage');
    //     }
    //   } catch (error) {
    //     console.error('Ошибка загрузки данных:', error);
    //   }
    // },
    
    refreshData() {
      this.getUserData();
    },
    
    formatDate(dateString) {
      return new Date(dateString).toLocaleString('ru-RU');
    },
    
    handleLogout() {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      this.$router.push('/auth');
    }
  }
};
</script>

<style scoped>
.profile {
  max-width: 500px;
  margin: 50px auto;
  padding: 30px;
  border: 1px solid #ddd;
  border-radius: 10px;
  background: white;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
}

h2 {
  text-align: center;
  margin-bottom: 30px;
  color: #333;
}

.loading {
  text-align: center;
  padding: 40px;
  color: #666;
}

.user-info {
  margin: 20px 0;
}

.info-item {
  display: flex;
  justify-content: space-between;
  padding: 12px 0;
  border-bottom: 1px solid #eee;
}

.info-item:last-child {
  border-bottom: none;
}

.label {
  font-weight: bold;
  color: #555;
}

.value {
  color: #333;
}

.no-data {
  text-align: center;
  padding: 40px;
  color: #666;
}

.actions {
  display: flex;
  gap: 15px;
  margin-top: 30px;
  justify-content: center;
}

button {
  padding: 12px 24px;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 14px;
}

.refresh-btn {
  background: #2196F3;
  color: white;
}

.refresh-btn:hover {
  background: #1976D2;
}

.logout-btn {
  background: #f44336;
  color: white;
}

.logout-btn:hover {
  background: #d32f2f;
}
</style>