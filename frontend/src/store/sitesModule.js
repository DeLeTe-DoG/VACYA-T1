import axios from "axios";
import { api } from "../api";

export const sitesModule = {
    namespaced: true,

    state: {
        sites: [],
        activeProject: 1,
        testScenarios: [],
    },
    mutations: {
        setSites(state, data) {
            state.sites = data
        },
        setTestScenarios(state, data) {
            state.testScenarios = data
        },
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