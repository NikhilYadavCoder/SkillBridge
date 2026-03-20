import { createContext, useContext, useState } from 'react'
import api from '../api/axios.js'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [token, setToken] = useState(() => localStorage.getItem('token'))
  const [user, setUser] = useState(() => {
    const raw = localStorage.getItem('user')
    return raw ? JSON.parse(raw) : null
  })
  const [loading, setLoading] = useState(false)

  const login = async (email, password) => {
    setLoading(true)
    try {
      const response = await api.post('/api/auth/login', { email, password })
      const data = response.data || {}

      const jwtToken =
        data.token || data.jwtToken || data.accessToken || data.access_token

      if (!jwtToken) {
        throw new Error('Token not found in response')
      }

      const userInfo = {
        email: data.email || email,
        name: data.name || data.fullName || 'User',
      }

      setToken(jwtToken)
      setUser(userInfo)
      localStorage.setItem('token', jwtToken)
      localStorage.setItem('user', JSON.stringify(userInfo))

      return { success: true }
    } catch (error) {
      const message =
        error.response?.data?.message || error.message || 'Login failed'
      return { success: false, message }
    } finally {
      setLoading(false)
    }
  }

  const register = async (payload) => {
    setLoading(true)
    try {
      await api.post('/api/auth/register', payload)
      return { success: true }
    } catch (error) {
      const message =
        error.response?.data?.message || error.message || 'Registration failed'
      return { success: false, message }
    } finally {
      setLoading(false)
    }
  }

  const logout = () => {
    setToken(null)
    setUser(null)
    localStorage.removeItem('token')
    localStorage.removeItem('user')
  }

  const value = { token, user, loading, login, register, logout }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
