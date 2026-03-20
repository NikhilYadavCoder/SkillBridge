import { useEffect, useState } from 'react'
import { useLocation } from 'react-router-dom'
import api from '../api/axios.js'
import Card from '../components/ui/Card.jsx'
import SkillTag from '../components/ui/SkillTag.jsx'
import ProgressBar from '../components/ui/ProgressBar.jsx'

function Analysis() {
  const location = useLocation()
  const [roles, setRoles] = useState([])
  const [selectedRole, setSelectedRole] = useState('')
  const [analysis, setAnalysis] = useState(null)
  const [loadingRoles, setLoadingRoles] = useState(true)
  const [loadingAnalysis, setLoadingAnalysis] = useState(false)
  const [error, setError] = useState('')

  useEffect(() => {
    let isMounted = true

    const fetchRolesAndInitialAnalysis = async () => {
      setLoadingRoles(true)
      setError('')
      try {
        const res = await api.get('/api/job/roles')
        if (!isMounted) return

        const list = Array.isArray(res.data) ? res.data : []
        setRoles(list)

        if (list.length > 0) {
          const initialRole = list[0]
          setSelectedRole(initialRole)
          await runAnalysis(initialRole, isMounted)
        }
      } catch {
        if (!isMounted) return
        setError('Unable to load roles.')
      } finally {
        if (isMounted) setLoadingRoles(false)
      }
    }

    const runAnalysis = async (role, mountedFlag = true) => {
      setLoadingAnalysis(true)
      setError('')
      try {
        const res = await api.post('/api/job/analyze', {
          role,
          pageNumber: 1,
          pageSize: 5,
        })
        if (!mountedFlag) return
        setAnalysis(res.data)
      } catch {
        if (!mountedFlag) return
        setError('Unable to run analysis for this role.')
      } finally {
        if (mountedFlag) setLoadingAnalysis(false)
      }
    }

    fetchRolesAndInitialAnalysis()

    return () => {
      isMounted = false
    }
  }, [])

  const handleRoleChange = async (event) => {
    const value = event.target.value
    setSelectedRole(value)
    if (value) {
      setError('')
      setAnalysis(null)

      setLoadingAnalysis(true)
      try {
        const res = await api.post('/api/job/analyze', {
          role: value,
          pageNumber: 1,
          pageSize: 5,
        })
        setAnalysis(res.data)
      } catch {
        setError('Unable to run analysis for this role.')
      } finally {
        setLoadingAnalysis(false)
      }
    }
  }

  let match = 0
  const variants = analysis && Array.isArray(analysis.variants) ? analysis.variants : []
  if (variants.length > 0) {
    const best = variants.reduce(
      (max, item) => (item.matchPercentage > max.matchPercentage ? item : max),
      variants[0]
    )
    match = Math.round(best.matchPercentage || 0)
  }

  const aggregatedMissingSkills = (() => {
    const set = new Set()
    for (const v of variants) {
      if (Array.isArray(v.missingSkills)) {
        for (const skill of v.missingSkills) {
          if (skill) set.add(skill)
        }
      }
    }
    return Array.from(set)
  })()

  return (
    <section className="space-y-6">
      {location.state?.fromResumeUpload && (
        <div className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-xs text-emerald-700">
          Resume uploaded successfully. Choose a target role to see your match.
        </div>
      )}

      {/* Match score & role selector */}
      <Card className="rounded-xl p-6 shadow-sm">
        <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
          <div>
            <p className="text-xs font-semibold uppercase tracking-[0.22em] text-blue-600">
              Match score
            </p>
            <p className="mt-2 text-4xl font-semibold text-slate-900">
              {loadingAnalysis && !analysis ? '…' : `${match}%`}
            </p>
            <p className="text-xs text-slate-500">
              Target role:{' '}
              {selectedRole || (loadingRoles ? 'Loading roles…' : 'No roles available')}
            </p>
          </div>
          <div className="w-full max-w-sm space-y-3">
            <div>
              <label className="block text-xs font-medium text-slate-500">
                Select a role
              </label>
              <select
                value={selectedRole}
                onChange={handleRoleChange}
                disabled={loadingRoles || roles.length === 0 || loadingAnalysis}
                className="mt-1 w-full rounded-lg border border-[#E2E8F0] bg-white px-3 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400 disabled:cursor-not-allowed disabled:bg-slate-50"
              >
                {roles.length === 0 ? (
                  <option value="">No roles available</option>
                ) : (
                  roles.map((role) => (
                    <option key={role} value={role}>
                      {role}
                    </option>
                  ))
                )}
              </select>
            </div>
            <div>
              <ProgressBar value={match} />
              <p className="mt-2 text-xs text-slate-500">
                Based on your current profile skills and the selected role.
              </p>
            </div>
          </div>
        </div>
        {error && (
          <p className="mt-3 text-xs text-red-500">{error}</p>
        )}
      </Card>

      {/* Variants & missing skills */}
      <div className="grid gap-6 md:grid-cols-2">
        <Card className="space-y-3 rounded-xl p-6 shadow-sm">
          <h2 className="text-base font-semibold text-slate-900">Role variants</h2>
          {loadingAnalysis && !analysis ? (
            <p className="text-xs text-slate-500">Running analysis…</p>
          ) : variants.length === 0 ? (
            <p className="text-xs text-slate-500">No analysis results yet.</p>
          ) : (
            <ul className="space-y-2 text-sm text-slate-600">
              {variants.map((result) => (
                <li key={result.variant}>
                  • {result.variant} ({Math.round(result.matchPercentage || 0)}%)
                </li>
              ))}
            </ul>
          )}
        </Card>

        <Card className="space-y-3 rounded-xl p-6 shadow-sm">
          <h2 className="text-base font-semibold text-slate-900">Missing skills</h2>
          {loadingAnalysis && !analysis ? (
            <p className="text-xs text-slate-500">Running analysis…</p>
          ) : aggregatedMissingSkills.length === 0 ? (
            <p className="text-xs text-slate-500">
              No missing skills identified for the selected role.
            </p>
          ) : (
            <div className="flex flex-wrap gap-2">
              {aggregatedMissingSkills.map((skill) => (
                <SkillTag key={skill}>{skill}</SkillTag>
              ))}
            </div>
          )}
        </Card>
      </div>

      {/* Roadmap */}
      <Card className="space-y-4 rounded-xl p-6 shadow-sm">
        <h2 className="text-base font-semibold text-slate-900">Roadmap</h2>
        <div className="grid gap-3 md:grid-cols-3">
          <div className="rounded-xl bg-slate-50 p-4">
            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Week 1–2
            </p>
            <p className="mt-1 text-sm font-semibold text-slate-900">
              Strengthen fundamentals
            </p>
            <p className="mt-1 text-xs text-slate-600">
              Focus on core concepts and solidify your base.
            </p>
          </div>
          <div className="rounded-xl bg-slate-50 p-4">
            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Week 3–4
            </p>
            <p className="mt-1 text-sm font-semibold text-slate-900">
              Ecosystem & tooling
            </p>
            <p className="mt-1 text-xs text-slate-600">
              Learn the key tools and platforms used with this role.
            </p>
          </div>
          <div className="rounded-xl bg-slate-50 p-4">
            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">
              Week 5
            </p>
            <p className="mt-1 text-sm font-semibold text-slate-900">
              Interview prep
            </p>
            <p className="mt-1 text-xs text-slate-600">
              Practise problem-solving and behavioural questions.
            </p>
          </div>
        </div>
      </Card>

      {/* Interview questions */}
      <Card className="space-y-3 rounded-xl p-6 shadow-sm">
        <h2 className="text-base font-semibold text-slate-900">Interview questions</h2>
        <div className="space-y-2 text-sm text-slate-700">
          <details className="group rounded-xl border border-slate-200 bg-slate-50 px-4 py-3">
            <summary className="flex cursor-pointer list-none items-center justify-between text-sm font-medium text-slate-900">
              How would you design a scalable API for job search?
              <span className="text-xs text-slate-500 group-open:hidden">Show</span>
              <span className="hidden text-xs text-slate-500 group-open:inline">Hide</span>
            </summary>
            <p className="mt-2 text-xs text-slate-600">
              Think about endpoints, data model, pagination, caching and failure
              handling.
            </p>
          </details>
          <details className="group rounded-xl border border-slate-200 bg-slate-50 px-4 py-3">
            <summary className="flex cursor-pointer list-none items-center justify-between text-sm font-medium text-slate-900">
              How do you secure an ASP.NET Core Web API?
              <span className="text-xs text-slate-500 group-open:hidden">Show</span>
              <span className="hidden text-xs text-slate-500 group-open:inline">Hide</span>
            </summary>
            <p className="mt-2 text-xs text-slate-600">
              Consider authentication, authorisation, input validation and
              protecting sensitive data.
            </p>
          </details>
        </div>
      </Card>
    </section>
  )
}

export default Analysis
