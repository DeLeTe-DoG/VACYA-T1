import { createRouter, createWebHistory} from "vue-router";
import BeginView from "../views/BeginView.vue"
import store from '../store'
// ниже объявляется массив путей для маршрутизатора vue-router
// поля name, path и component используем базово обязательно
// в будущем добавим еще поле beforeEnter, оно нужно для ограничения доступа к странице если человек не залогинился
const routes = [
    {
        name: 'home',
        path: '/',
        meta: {needProjectCheck: true},
        component: () => import('../views/HomeView.vue'),
        beforeEnter: (to, from, next) => {
            guard(to, from, next)
        }
    },
    {
        name: 'dashboard',
        path: '/dashboard',
        meta: {needProjectCheck: true},
        component: () => import('../views/MainView.vue'),
        beforeEnter: (to, from, next) => {
            guard(to, from, next)
        }
    },
    {
        path: '/auth',
        name: 'Auth',
        component: () => import("../views/LoginView.vue"),
    },
    {
        path: '/profile',
        name: 'Profile',
        component: () => import("../views/Profile.vue"),
        beforeEnter: (to, from, next) => {
            guard(to, from, next)
        }
    },
    {
        path: '/add-site',
        name: 'Add-site',
        component: () => import('../views/AddSiteView.vue'),
        beforeEnter: (to, from, next) => {
            guard(to, from, next)
        }
    },
    {
        path: '/tests',
        name: 'Tests',
        meta: {needProjectCheck: true},
        component: () => import('../views/Tests.vue'),
        // beforeEnter: (to, from, next) => {
        //     guard(to, from, next)
        // }
    },
    {
        path: '/tests/plan-test',
        name: 'add-plan-test',
        meta: {needProjectCheck: true},
        component: () => import('../views/AddPlanTests.vue'),
        beforeEnter: (to, from, next) => {
            guard(to, from, next)
        }
    },


]

const guard = function (to, from, next) {
    const token = localStorage.getItem('token')
    // if(!token) {
    //     return next({path: '/auth'})
    // }
    // if(to.meta.needProjectCheck) {
    //     let activeProject = store.state.sites.activeProject
    //     return next({query: { project: activeProject }})
    // setTimeout(() => {
    //     sites = store.state.sites.sites
    //     console.log(sites.length)
    //     if(sites.length == 0) {
    //         router.push({path: '/add-site'})
    //     }
    // }, 2000)
    // if(JSON.parse(localStorage.getItem('userData')).sites.length == 0) {
    //     router.push({ path: '/add-site' })
    // }
    if(token) {
        next();
    } else {
        router.push({ path: '/auth' })
    }
}

export const router = new createRouter({
    routes,
    history: createWebHistory()
})

// router.beforeEach((to, from, next) => {
//   const isAuthenticated = !!localStorage.getItem('authToken');
  
//   if (to.meta.requiresAuth && !isAuthenticated) {
//     next('/auth');
//   } else {
//     next();
//   }
// });