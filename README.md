# VACYA-T1
View And Check Your Apps

## BACKEND
Представляет собой стандартный сервер с собсвтенными эндпоинтами, связью с базой данных, сервисами и контроллерами. Так же он:
- Написан на `ASP.NET` Core с поддержкой регистрации, логина. Имеет хэширование пароля пользователя при помощи `BCrypt`.

- Пользователи могут добавлять сайты, прописывая их `URl`, в этом случае проверка будет происходить только по ожидаемому содержимому и по StatusCode параметру.

- Кроме этого, пользователь может создать отдельные `Сценарии`, котоыре будут проверять эндпоинты указанного `API`, с указанными заголовками, телом запроса и методом запроса

- Реализован фильтр по дате, зависящий от выбранного диапазона в днях.

- В качестве базы данных имеет PostgreSQL базу данных, хранящуюся на ресурсе [Render](https://render.com)

## Документация

### 🔹 *Регистрация пользователя*
**Метод:** `POST` \
**API:** `/api/user/register`\
**Авторизация:** ❌ не требуется

### *Тело запроса*
```json
{
  "name": "string",
  "email": "string",
  "password": "string"
}
```
### *Успешный ответ:* <span style="color: green;">200 OK</span>

```json
{
  "token": "jwt-token",
  "user": {
    "name": "string",
    "email": "string"
  }
}
```

### *Возможные ошибки*
- `400 Bad Request` — некорректные данные или пользователь уже существует

##

### 🔹 *Логин пользователя*
**Метод:** `POST` \
**API:** `/api/user/login`\
**Авторизация:** ❌ не требуется

### *Тело запроса*
```json
{
  "name": "string",
  "password": "string"
}
```
### *Успешный ответ:* <span style="color: green;">200 OK</span>

```json
{
  "token": "jwt-token",
  "user": {
    "name": "string",
    "email": "string"
  }
}
```
### *Возможные ошибки*
- `400 Bad Request` — некорректные данные или пользователь уже существует
- `401 Unauthorized` — неверный логин или пароль

##

### 🔹 *Получить сайты пользователя*
**Метод:** `GET` \
**API:** `/api/user/{userName}/sites`\
**Авторизация:** ✅ требуется JWT

### *Заголовки запроса*
```json
{
  "Autorization": "Bearer {token}",
  "Content-Type": "application/json"
}
```

### *Тело запроса*
```json
[
  {
    "id": 1,
    "name": "My Site",
    "url": "https://example.com",
    "expectedContent": "OK",
    "totalErrors": 2,
    "webSiteData": []
  }
]
```
### *Успешный ответ:* <span style="color: green;">200 OK</span>

```json
[
  {
    "id": 1,
    "name": "My Site",
    "url": "https://example.com",
    "expectedContent": "OK",
    "totalErrors": 2,
    "webSiteData": []
  }
]
```
### *Возможные ошибки*: 
- `401 Unauthorized` — **причина:** отсутствие токена в заголовках

##

### 🔹 *Добавить сайт*
**Метод:** `POST` \
**API:** `/api/user/{userName}/sites`\
**Авторизация:** ✅ требуется JWT

### *Заголовки запроса*
```json
{
  "Autorization": "Bearer {token}",
  "Content-Type": "application/json"
}
```

### *Тело запроса*
```json
{
  "name": "My Site",
  "url": "https://example.com",
  "expectedContent": "OK"
}
```
### *Успешный ответ:* <span style="color: green;">200 OK</span>

```json
{
    "id": 5,
    "url": "http://localhost:5046/api/user/register",
    "name": "VacyaREGLOCAL",
    "expectedContent": "Vacya",
    "responseTime": "",
    "isAvailable": false, // только true/false
    "dns": "",
    "ssl": "",
    "totalErrors": 0,
    "webSiteData": [],
    "testScenarios": [],
    "testsData": []
}
```
### *Возможные ошибки*: 
- `401 Unauthorized` — **причина:** отсутствие токена в заголовках
- `400 Bad Request` — сайт с таким URL уже существует
- `400 Bad Request` — сайт с таким именем `name` уже существует

##

### 🔹 *Добавить новый сценарий*
**Метод:** `POST` \
**API:** `/api/user/{userName}/sites/{siteName}/scenarios/add`\
**Авторизация:** ✅ требуется JWT

### *Заголовки запроса*
```json
{
  "Autorization": "Bearer {token}",
  "Content-Type": "application/json"
}
```

### *Тело запроса*
```json
{
  "name": "Check HomePage",
  "httpMethod": "GET",
  "body": null,
  "checkXml": false, // только true/false
  "checkJson": true, // только true/false
  "expectedContent": "Welcome"
}

```
### *Успешный ответ:* <span style="color: green;">200 OK</span>

```json
{
    "siteId": "5",
    "url": "",
    "name": "Проверка регистрации",
    "httpMethod": "POST",
    "headers": {
        "Content-Type": "application/json"
    },
    "body": "{ \"Name\": \"b\", \"Email\": \"b\", \"Password\": \"b\" }",
    "expectedContent": "token",
    "checkJson": true, // только true/false
    "checkXml": true // только true/false
}
```
### *Возможные ошибки*: 
- `401 Unauthorized` — **причина:** отсутствие токена в заголовках
- `400 Bad Request` — ошибка добавления сценария (отсутствие нужного поля)

##

### 🔹 *Фильтрация данных по дате*
**Метод:** `POST` \
**API:** `/api/user/{userId}/filter`\
**Авторизация:** ✅ требуется JWT

### *Заголовки запроса*
```json
{
  "Autorization": "Bearer {token}",
  "Content-Type": "application/json"
}
```

### *Тело запроса*
```json
{
  "dateFrom": "2025-09-01T00:00:00Z",
  "dateTo": "2025-09-20T23:59:59Z"
}

```
### *Успешный ответ:* <span style="color: green;">200 OK</span>

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "My Site",
      "url": "https://example.com",
      "totalErrors": 3,
      "webSiteData": [
        {
          "id": 101,
          "statusCode": 200,
          "errorMessage": null,
          "lastChecked": "2025-09-19T12:00:00Z"
        },
        {
          "id": 102,
          "statusCode": 200,
          "errorMessage": null,
          "lastChecked": "2025-09-19T12:00:00Z"
        }
      ]
    }
  ]
}
```
### *Возможные ошибки*: 
- `400 Bad Request` — дата начала больше даты конца
- `404 Not Found` — нет данных за указанный период

##

### 🔹 *Удалить пользователя*
**Метод:** `DELETE` \
**API:** `/api/user/{userName}`\
**Авторизация:** ✅ требуется JWT

### *Заголовки запроса*
```json
{
  "Autorization": "Bearer {token}",
  "Content-Type": "application/json"
}
```

### *Тело запроса*
```json
{
  // Данные форируют запрос и отправляют POST без тела 
}

```
### *Успешный ответ:* <span style="color: green;">200 OK</span>

```json
{
  "success": true,
  "message": "Пользователь удалён"
}
```
### *Возможные ошибки*: 
- `401 Unauthorized` — **причина:** отсутствие токена в заголовках

##

### 🔹 *Удалить сайт*
**Метод:** `DELETE` \
**API:** `/api/user/{userName}/sites/{siteId}`\
**Авторизация:** ✅ требуется JWT

### *Заголовки запроса*
```json
{
  "Autorization": "Bearer {token}",
  "Content-Type": "application/json"
}
```

### *Тело запроса*
```json
{
  // Данные форируют запрос и отправляют POST без тела 
}

```
### *Успешный ответ:* <span style="color: green;">200 OK</span>

```json
{
  "success": true,
  "message": "Сайт удалён"
}
```
### *Возможные ошибки*: 
- `401 Unauthorized` — **причина:** отсутствие токена в заголовках
- `400 Bad Request` — сайт не найден

##

### 🔹 *Удалить сценарий*
**Метод:** `DELETE` \
**API:** `/api/user/{userName}/sites/{siteId}`\
**Авторизация:** ✅ требуется JWT

### *Заголовки запроса*
```json
{
  "Autorization": "Bearer {token}",
  "Content-Type": "application/json"
}
```

### *Тело запроса*
```json
{
  // Данные форируют запрос и отправляют POST без тела 
}

```
### *Успешный ответ:* <span style="color: green;">200 OK</span>

```json
{
  "success": true,
  "message": "Сценарий удалён"
}
```
### *Возможные ошибки*: 
- `401 Unauthorized` — **причина:** отсутствие токена в заголовках
- `400 Bad Request` — сценарий не найден

##

