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
    },
    mutations: {
        toggleMenu(state) {
            state.visibleMenu = !state.visibleMenu
        },
    },
    actions: {
        getUserData() {
            axios
                .get(`${api}/api/user/me`, {
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem('token')}`,
                        'Content-Type': 'application/json'
                    }
                })
                .then(response => {
                    console.log(response.data)
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