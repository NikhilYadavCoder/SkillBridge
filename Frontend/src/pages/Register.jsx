import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'

function Register() {
  const { register, loading } = useAuth()
  const navigate = useNavigate()

  const [form, setForm] = useState({
    name: '',
    email: '',
    dateOfBirth: '',
    gender: '',
    contact: '',
    password: '',
    confirmPassword: '',
  })
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})

  const handleChange = (e) => {
    const { name, value } = e.target
    setForm((prev) => ({ ...prev, [name]: value }))
    setFieldErrors((prev) => ({ ...prev, [name]: '' }))
  }

  const validate = () => {
    const nextErrors = {}

    if (!form.name.trim()) {
      nextErrors.name = 'Full name is required'
    } else if (form.name.trim().length < 2) {
      nextErrors.name = 'Please enter your full name'
    }

    if (!form.email.trim()) {
      nextErrors.email = 'Email is required'
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
      nextErrors.email = 'Please enter a valid email address'
    }

    if (form.contact && !/^\d{10,15}$/.test(form.contact)) {
      nextErrors.contact = 'Enter a valid contact number (10–15 digits)'
    }

    if (!form.password) {
      nextErrors.password = 'Password is required'
    } else if (form.password.length < 8) {
      nextErrors.password = 'Password must be at least 8 characters'
    }

    if (!form.confirmPassword) {
      nextErrors.confirmPassword = 'Please confirm your password'
    } else if (form.password !== form.confirmPassword) {
      nextErrors.confirmPassword = 'Password and Confirm Password must match'
    }

    return nextErrors
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setSuccess('')

    const validationErrors = validate()
    if (Object.keys(validationErrors).length > 0) {
      setFieldErrors(validationErrors)
      setError('Please fix the highlighted fields.')
      return
    }

    const payload = {
      name: form.name,
      email: form.email,
      dateOfBirth: form.dateOfBirth || null,
      gender: form.gender || null,
      contact: form.contact || null,
      password: form.password,
      confirmPassword: form.confirmPassword,
    }

    const result = await register(payload)
    if (result.success) {
      setSuccess('Registration successful. Redirecting to login…')
      setTimeout(() => navigate('/login'), 1200)
    } else {
      setError(result.message || 'Registration failed')
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
            Create your SkillBridge account
          </h1>
          <p className="text-sm text-blue-100/90">
            We&apos;ll use this to personalise your job matches, learning roadmap
            and interview prep.
          </p>
        </div>
      </div>

      {/* Right side: form card */}
      <div className="flex w-full items-center justify-center px-6 py-10 lg:w-1/2">
        <div className="w-full max-w-md rounded-xl bg-white p-8 shadow-md">
          <h2 className="text-2xl font-semibold text-slate-900">
            Create your SkillBridge account
          </h2>
          <p className="mt-1 text-sm text-slate-500">
            We&apos;ll use this to personalise your job matches and roadmap.
          </p>

          <form className="mt-6 space-y-4" onSubmit={handleSubmit}>
            <div className="space-y-1">
              <label
                className="text-sm font-medium text-slate-700"
                htmlFor="name"
              >
                Full name <span className="text-red-500">*</span>
              </label>
              <input
                id="name"
                name="name"
                type="text"
                className={`w-full rounded-lg border px-4 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400 ${
                  fieldErrors.name ? 'border-red-400' : 'border-[#E2E8F0]'
                }`}
                value={form.name}
                onChange={handleChange}
              />
              {fieldErrors.name && (
                <p className="text-xs text-red-500">{fieldErrors.name}</p>
              )}
            </div>

            <div className="space-y-1">
              <label
                className="text-sm font-medium text-slate-700"
                htmlFor="email"
              >
                Email <span className="text-red-500">*</span>
              </label>
              <input
                id="email"
                name="email"
                type="email"
                className={`w-full rounded-lg border px-4 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400 ${
                  fieldErrors.email ? 'border-red-400' : 'border-[#E2E8F0]'
                }`}
                autoComplete="email"
                value={form.email}
                onChange={handleChange}
              />
              {fieldErrors.email && (
                <p className="text-xs text-red-500">{fieldErrors.email}</p>
              )}
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-1">
                <label
                  className="text-sm font-medium text-slate-700"
                  htmlFor="dateOfBirth"
                >
                  Date of birth
                </label>
                <input
                  id="dateOfBirth"
                  name="dateOfBirth"
                  type="date"
                  className="w-full rounded-lg border border-[#E2E8F0] px-4 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
                  value={form.dateOfBirth}
                  onChange={handleChange}
                />
              </div>

              <div className="space-y-1">
                <label
                  className="text-sm font-medium text-slate-700"
                  htmlFor="gender"
                >
                  Gender
                </label>
                <select
                  id="gender"
                  name="gender"
                  className="w-full rounded-lg border border-[#E2E8F0] bg-white px-4 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
                  value={form.gender}
                  onChange={handleChange}
                >
                  <option value="">Select</option>
                  <option value="Male">Male</option>
                  <option value="Female">Female</option>
                  <option value="Other">Other</option>
                  <option value="Prefer not to say">Prefer not to say</option>
                </select>
              </div>
            </div>

            <div className="space-y-1">
              <label
                className="text-sm font-medium text-slate-700"
                htmlFor="contact"
              >
                Contact number
              </label>
              <input
                id="contact"
                name="contact"
                type="tel"
                className={`w-full rounded-lg border px-4 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400 ${
                  fieldErrors.contact ? 'border-red-400' : 'border-[#E2E8F0]'
                }`}
                value={form.contact}
                onChange={handleChange}
              />
              <p className="text-xs text-slate-500">
                Optional – used only for your profile.
              </p>
              {fieldErrors.contact && (
                <p className="text-xs text-red-500">{fieldErrors.contact}</p>
              )}
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div className="space-y-1">
                <label
                  className="text-sm font-medium text-slate-700"
                  htmlFor="password"
                >
                  Password <span className="text-red-500">*</span>
                </label>
                <input
                  id="password"
                  name="password"
                  type="password"
                  className={`w-full rounded-lg border px-4 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400 ${
                    fieldErrors.password ? 'border-red-400' : 'border-[#E2E8F0]'
                  }`}
                  autoComplete="new-password"
                  placeholder="Password"
                  value={form.password}
                  onChange={handleChange}
                />
                {fieldErrors.password && (
                  <p className="text-xs text-red-500">{fieldErrors.password}</p>
                )}
              </div>

              <div className="space-y-1">
                <label
                  className="text-sm font-medium text-slate-700"
                  htmlFor="confirmPassword"
                >
                  Confirm password <span className="text-red-500">*</span>
                </label>
                <input
                  id="confirmPassword"
                  name="confirmPassword"
                  type="password"
                  className={`w-full rounded-lg border px-4 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400 ${
                    fieldErrors.confirmPassword ? 'border-red-400' : 'border-[#E2E8F0]'
                  }`}
                  autoComplete="new-password"
                  placeholder="Confirm password"
                  value={form.confirmPassword}
                  onChange={handleChange}
                />
                {fieldErrors.confirmPassword && (
                  <p className="text-xs text-red-500">{fieldErrors.confirmPassword}</p>
                )}
              </div>
            </div>

            {error && <p className="text-xs text-red-500">{error}</p>}
            {success && <p className="text-xs text-emerald-600">{success}</p>}

            <button
              type="submit"
              disabled={loading}
              className="mt-2 w-full rounded-lg bg-[#3B82F6] px-4 py-2 text-sm font-medium text-white shadow-sm transition-colors duration-200 hover:bg-blue-600 disabled:cursor-not-allowed disabled:opacity-60"
            >
              {loading ? 'Creating account…' : 'Create account'}
            </button>
          </form>

          <p className="mt-4 text-xs text-slate-500">
            Already have an account?{' '}
            <Link to="/login" className="font-medium text-blue-600">
              Login
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}

export default Register
