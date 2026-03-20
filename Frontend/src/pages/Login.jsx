import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'

function Login() {
  const { login, loading } = useAuth()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})

  const validate = () => {
    const nextErrors = {}

    if (!email.trim()) {
      nextErrors.email = 'Email is required'
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      nextErrors.email = 'Please enter a valid email address'
    }

    if (!password) {
      nextErrors.password = 'Password is required'
    } else if (password.length < 8) {
      nextErrors.password = 'Password must be at least 8 characters'
    }

    return nextErrors
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setFieldErrors({})

    const validationErrors = validate()
    if (Object.keys(validationErrors).length > 0) {
      setFieldErrors(validationErrors)
      return
    }
    const result = await login(email, password)
    if (result.success) {
      navigate('/dashboard')
    } else {
      setError(result.message || 'Login failed')
    }
  }

  return (
    <div className="flex min-h-[calc(100vh-4rem)] bg-[#F8FAFC]">
      {/* Left side: gradient intro */}
      <div className="hidden w-1/2 items-center justify-center bg-gradient-to-br from-blue-500 to-blue-300 p-10 text-white lg:flex">
        <div className="max-w-md space-y-4">
          <p className="text-sm font-semibold uppercase tracking-[0.22em] text-blue-100/90">
            SkillBridge
          </p>
          <h1 className="text-3xl font-bold leading-tight">
            Welcome to SkillBridge
          </h1>
          <p className="text-sm text-blue-100/90">
            A focused workspace where your resume, skills, roadmap and
            interview prep live together. Log in to continue your journey.
          </p>
        </div>
      </div>

      {/* Right side: form card */}
      <div className="flex w-full items-center justify-center px-6 py-10 lg:w-1/2">
        <div className="w-full max-w-md rounded-xl bg-white p-8 shadow-md">
          <h2 className="text-2xl font-semibold text-slate-900">Welcome back</h2>
          <p className="mt-1 text-sm text-slate-500">
            Log in to access your personalised matches, roadmap and interview prep.
          </p>

          <form className="mt-6 space-y-4" onSubmit={handleSubmit}>
            <div className="space-y-1">
              <label
                className="text-sm font-medium text-slate-700"
                htmlFor="email"
              >
                Email <span className="text-red-500">*</span>
              </label>
              <input
                id="email"
                type="email"
                className={`w-full rounded-lg border px-4 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400 ${
                  fieldErrors.email ? 'border-red-400' : 'border-[#E2E8F0]'
                }`}
                autoComplete="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
              {fieldErrors.email && (
                <p className="text-xs text-red-500">{fieldErrors.email}</p>
              )}
            </div>

            <div className="space-y-1">
              <label
                className="text-sm font-medium text-slate-700"
                htmlFor="password"
              >
                Password <span className="text-red-500">*</span>
              </label>
              <input
                id="password"
                type="password"
                className={`w-full rounded-lg border px-4 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400 ${
                  fieldErrors.password ? 'border-red-400' : 'border-[#E2E8F0]'
                }`}
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              {fieldErrors.password && (
                <p className="text-xs text-red-500">{fieldErrors.password}</p>
              )}
            </div>

            {error && <p className="text-xs text-red-500">{error}</p>}

            <button
              type="submit"
              disabled={loading}
              className="mt-2 w-full rounded-lg bg-[#3B82F6] px-4 py-2 text-sm font-medium text-white shadow-sm transition-colors duration-200 hover:bg-blue-600 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {loading ? 'Signing you in…' : 'Login'}
            </button>
          </form>

          <p className="mt-4 text-xs text-slate-500">
            New to SkillBridge?{' '}
            <Link to="/register" className="font-medium text-blue-600">
              Create an account
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}

export default Login
