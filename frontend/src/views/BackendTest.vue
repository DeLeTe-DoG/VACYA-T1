<template>
  <div class="api-tester">
    <div class="controls">
      <button 
        @click="fetchData" 
        :disabled="loading"
        class="fetch-button"
      >
        {{ loading ? 'Загрузка...' : 'Получить данные с бэка' }}
      </button>
      
      <button 
        @click="clearData" 
        class="clear-button"
      >
        Очистить
      </button>
    </div>

    <div v-if="error" class="error">
      Ошибка: {{ error }}
    </div>

    <div v-if="data" class="results">
      <h3>Результаты с бэка:</h3>
      <pre>{{ formattedData }}</pre>
    </div>

    <div v-else-if="!loading" class="placeholder">
      Нажмите кнопку выше чтобы получить данные с бэка
    </div>
  </div>
</template>

<script>
import { computed } from 'vue';

  export default {
    name: 'ApiTester',
    data() {
      return {
        loading: false,
        data: null,
        error: null
      }
    },
    computed: {
      formattedData() {
        return JSON.stringify(this.data, null, 2)
      }
    },
   methods: {
      async fetchData() {
        this.loading = true
        this.error = null

        try {
          const response = await fetch('https://vacya.onrender.com/api/weather', {
            method: 'GET',
            headers: {
              'Accept': 'application/json',
            }
          })

          if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`)
          }

          this.data = await response.json()
        } catch (err) {
          this.error = err.message
          console.error('Ошибка при запросе:', err)
        } finally {
          this.loading = false
        }
      },
      clearData() {
        this.data = null
        this.error = null
      }
    }
}
</script>

<style scoped>
.api-tester {
  background: #f8f9fa;
  padding: 20px;
  border-radius: 8px;
  border: 1px solid #e9ecef;
}

.controls {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
  flex-wrap: wrap;
}

.fetch-button, .clear-button {
  padding: 10px 20px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.2s;
}

.fetch-button {
  background-color: #007bff;
  color: white;
}

.fetch-button:hover:not(:disabled) {
  background-color: #0056b3;
}

.fetch-button:disabled {
  background-color: #6c757d;
  cursor: not-allowed;
}

.clear-button {
  background-color: #6c757d;
  color: white;
}

.clear-button:hover {
  background-color: #545b62;
}

.error {
  color: #dc3545;
  background-color: #f8d7da;
  border: 1px solid #f5c6cb;
  padding: 10px;
  border-radius: 4px;
  margin-bottom: 15px;
}

.results {
  background: white;
  padding: 15px;
  border-radius: 4px;
  border: 1px solid #dee2e6;
  overflow-x: auto;
}

.results pre {
  margin: 0;
  font-family: 'Courier New', monospace;
  font-size: 12px;
}

.placeholder {
  text-align: center;
  color: #6c757d;
  font-style: italic;
  padding: 40px 0;
}
</style>