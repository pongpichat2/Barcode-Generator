import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { LoginRequest } from '../types'
import api from '../services/api'

export const useAuthStore = defineStore('auth', () => {
  const token = ref<string | null>(localStorage.getItem('token'))
  const username = ref<string | null>(localStorage.getItem('username'))
  const isLoggedIn = ref(!!token.value)

  async function login(req: LoginRequest) {
    const res = await api.post('/auth/login', req)
    token.value = res.data.token
    username.value = res.data.username
    isLoggedIn.value = true
    localStorage.setItem('token', res.data.token)
    localStorage.setItem('username', res.data.username)
  }

  function logout() {
    token.value = null
    username.value = null
    isLoggedIn.value = false
    localStorage.removeItem('token')
    localStorage.removeItem('username')
  }

  return { token, username, isLoggedIn, login, logout }
})
