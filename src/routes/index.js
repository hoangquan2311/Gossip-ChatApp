import { createRouter, createWebHistory } from 'vue-router'

// import Chat from '../components/Chat.vue'
// import Home from '../components/Home.vue'
// import Auth from '../components/Auth.vue'

const Home = () => import('../components/pages/Home.vue')
const Chat = () => import('../components/pages/Chat.vue')
const Auth = () => import('../components/pages/Auth.vue')    

const routes = [    
  { 
    path: '/',
    component: Home 
  },
  { 
    path: '/chat/',
    component: Chat 
  },
  { 
    path: '/auth/',
    component: Auth
  }
]

const router = createRouter({
  history: createWebHistory(),  
  routes
})

export default router