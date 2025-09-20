import axios from "axios";
import { api } from "../api";
import { router } from "../router";

export const sitesModule = {
    namespaced: true,

    state: {
        sites: null,
        testScenarios: [],
        activeSite: null,
    },
    mutations: {
        setSites(state, data) {
            state.sites = data
        },
        setTestScenarios(state, data) {
            state.testScenarios = data
        },
        setActiveSite(state, site_id) {
            console.log(site_id)
            if(site_id) {
                state.activeSite = site_id
            } else {
                state.activeSite = $route.query.project
            }
        }
    },
    actions: {
        getSites({state, commit}, data) {
            console.log(JSON.parse(localStorage.getItem('userData')))
            return axios
                .get(`${api}/api/user/${JSON.parse(localStorage.getItem('userData')).name}/sites`, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`,
                    }
                })
                .then(response => {
                    console.log(response.data)
                    commit('setSites', response.data)
                    if(!response.data) {
                        localStorage.removeItem('token')
                    }
                })
        },
        addSite({state, commit}, data) {
            axios
                .post(`${api}/api/user/${JSON.parse(localStorage.getItem('userData')).name}/sites/add`, data,
                    {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                })
                .then(response => {
                    console.log(response)
                })
        },
        addScenarios({state, commit}, data) {
            const site_name = state.sites.find(site => site.id == data[0]).name
            axios
                .post(`${api}/api/user/${JSON.parse(localStorage.getItem('userData')).name}/sites/${site_name}/scenarios/add`, 
                data[1], {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`
                    }
                })
                .then(response => {
                    console.log(response)
                })
        }
    }
}