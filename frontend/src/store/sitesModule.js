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
            axios
                .get(`${api}/api/user/me`, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`,
                    }
                })
                .then(response => {
                    console.log(response.data.sites)
                    commit('setSites', response.data.sites)
                    if(!response.data) {
                        localStorage.removeItem('token')
                    }
                })
        },
        addSite({state, commit}, data) {
            axios
                .post(`${api}/api/user/me/`, data,
                    {
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