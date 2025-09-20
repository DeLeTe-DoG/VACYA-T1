import { createStore } from "vuex";
import { mapState, mapActions } from 'vuex';
import axios from "axios";
import { api } from "../api";

import { authModule } from "./authModule";
import { sitesModule } from "./sitesModule";

export default createStore({
    modules: {
        auth: authModule,
        sites: sitesModule,
    },
    state: {
        visibleMenu: true,
        userData: null,
    },
    mutations: {
        toggleMenu(state) {
            state.visibleMenu = !state.visibleMenu
        },
        setUserData(state, data) {
            state.userData = data
        }
    },
    actions: {
        getUserData({state, commit}, data) {
            axios
                .get(`${api}/api/user/me`, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`,
                        'Content-Type': 'application/json'
                    }
                })
                .then(response => {
                    console.log(response.data)
                    commit('setUserData', response.data)
                    if(!response.data) {
                        localStorage.removeItem('token')
                    }
                })
        }
    },
})




    
    

// export default {
//     data() {
//         return{

//         }
//     }
// }
// }