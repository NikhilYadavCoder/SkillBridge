import { useEffect, useState } from 'react'
import { useLocation } from 'react-router-dom'
import api from '../api/axios.js'
import Card from '../components/ui/Card.jsx'

function Analysis() {
  const location = useLocation()
  const [roles, setRoles] = useState([])
  const [selectedRole, setSelectedRole] = useState('')
  const [analysis, setAnalysis] = useState(null)
  const [loadingRoles, setLoadingRoles] = useState(true)
  const [loadingAnalysis, setLoadingAnalysis] = useState(false)
  const [error, setError] = useState('')
  const [page, setPage] = useState(1)
  const pageSize = 2
  const [openVariant, setOpenVariant] = useState(null)

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
      setPage(1)

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

  const variants = analysis && Array.isArray(analysis.variants) ? analysis.variants : []
  const paginatedVariants = variants.slice((page - 1) * pageSize, page * pageSize)

  return (
    <section className="space-y-6">
      {location.state?.fromResumeUpload && (
        <div className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-xs text-emerald-700">
          Resume uploaded successfully. Choose a target role to see your match.
        </div>
      )}

      {/* Role selector */}
      <Card className="rounded-xl p-6 shadow-sm">
        <div className="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
          <div>
            <p className="text-xs font-semibold uppercase tracking-[0.22em] text-blue-600">
              Role analysis
            </p>
            <p className="mt-2 text-lg font-semibold text-slate-900">
              {selectedRole || (loadingRoles ? 'Loading roles…' : 'Select a role')}
            </p>
            <p className="mt-1 text-xs text-slate-500">
              Pick a target role to see variant-wise results.
            </p>
          </div>
          <div className="w-full max-w-sm">
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
            {error && (
              <p className="mt-2 text-xs text-red-500">{error}</p>
            )}
          </div>
        </div>
      </Card>

      {/* Variant-wise results */}
      {loadingAnalysis && !analysis ? (
        <Card className="rounded-xl p-6 shadow-sm">
          <p className="text-xs text-slate-500">Running analysis…</p>
        </Card>
      ) : variants.length === 0 ? (
        <Card className="rounded-xl p-6 shadow-sm">
          <p className="text-xs text-slate-500">No analysis results yet.</p>
        </Card>
      ) : (
        <>
          <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
            {paginatedVariants.map((item) => {
            const matchedSkills = Array.isArray(item.matchedSkills) ? item.matchedSkills : []
            const missingSkills = Array.isArray(item.missingSkills) ? item.missingSkills : []
            const roadmapItems =
              item.roadmap && Array.isArray(item.roadmap.roadmap)
                ? item.roadmap.roadmap
                : []
            const interviewItems =
              item.interviewQuestions &&
              Array.isArray(item.interviewQuestions.questions)
                ? item.interviewQuestions.questions
                : []

            return (
              <Card
                key={item.variant}
                className="flex h-full flex-col space-y-4 rounded-xl p-6 shadow-sm transition-shadow duration-200 hover:shadow-md"
              >
                {/* Header */}
                <div className="flex items-baseline justify-between gap-2">
                  <div>
                    <h2 className="text-sm font-semibold text-slate-900">
                      {item.variant || 'Variant'}
                    </h2>
                    <p className="mt-0.5 text-xs text-slate-500">{selectedRole}</p>
                  </div>
                  <p className="text-xl font-semibold text-slate-900">
                    {Math.round(item.matchPercentage || 0)}%
                    <span className="ml-1 text-xs font-normal text-slate-500">match</span>
                  </p>
                </div>

                {/* Matched skills */}
                <div className="space-y-1">
                  <p className="text-xs font-medium text-slate-500">Matched skills</p>
                  {matchedSkills.length === 0 ? (
                    <p className="text-xs text-slate-500">No matched skills.</p>
                  ) : (
                    <div className="flex flex-wrap gap-2">
                      {matchedSkills.map((skill) => (
                        <span
                          key={skill}
                          className="rounded-full bg-green-100 px-3 py-1 text-xs font-medium text-green-600"
                        >
                          {skill}
                        </span>
                      ))}
                    </div>
                  )}
                </div>

                {/* Missing skills */}
                <div className="space-y-1">
                  <p className="text-xs font-medium text-slate-500">Missing skills</p>
                  {missingSkills.length === 0 ? (
                    <p className="text-xs text-slate-500">No missing skills.</p>
                  ) : (
                    <div className="flex flex-wrap gap-2">
                      {missingSkills.map((skill) => (
                        <span
                          key={skill}
                          className="rounded-full bg-blue-100 px-3 py-1 text-xs font-medium text-blue-600"
                        >
                          {skill}
                        </span>
                      ))}
                    </div>
                  )}
                </div>

                {/* Roadmap */}
                <div className="space-y-1">
                  <p className="text-xs font-medium text-slate-500">Roadmap</p>
                  {roadmapItems.length === 0 ? (
                    <p className="text-xs text-slate-500">No roadmap generated for this variant.</p>
                  ) : (
                    <ul className="space-y-1 text-xs text-slate-600">
                      {roadmapItems.slice(0, 3).map((item) => {
                        const steps = Array.isArray(item.steps) ? item.steps : []
                        const resources = Array.isArray(item.resources) ? item.resources : []

                        return (
                          <li key={item.skill} className="rounded-lg bg-slate-50 px-3 py-2">
                            <p className="text-[11px] font-semibold text-slate-900">
                              {item.skill}
                              {item.estimatedTime && (
                                <span className="ml-1 text-[10px] font-normal text-slate-500">
                                  • {item.estimatedTime}
                                </span>
                              )}
                            </p>

                            {steps.length > 0 && (
                              <ol className="mt-1 list-decimal space-y-0.5 pl-4 text-[11px] text-slate-600">
                                {steps.slice(0, 3).map((s, index) => (
                                  <li key={index}>{s}</li>
                                ))}
                              </ol>
                            )}

                            {resources.length > 0 && (
                              <div className="mt-1">
                                <p className="text-[10px] font-medium text-slate-500">Resources</p>
                                <ul className="mt-0.5 list-disc space-y-0.5 pl-4 text-[11px] text-slate-600">
                                  {resources.slice(0, 2).map((r, index) => (
                                    <li key={index}>{r}</li>
                                  ))}
                                </ul>
                              </div>
                            )}
                          </li>
                        )
                      })}
                    </ul>
                  )}
                </div>

                {/* Interview questions */}
                <div className="space-y-1">
                  <p className="text-xs font-medium text-slate-500">Interview questions</p>
                  {interviewItems.length === 0 ? (
                    <p className="text-xs text-slate-500">No interview questions generated.</p>
                  ) : (
                    <div className="space-y-2">
                      <button
                        type="button"
                        onClick={() =>
                          setOpenVariant((current) =>
                            current === item.variant ? null : item.variant
                          )
                        }
                        className="rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-xs font-medium text-slate-900 hover:bg-slate-100"
                      >
                        {openVariant === item.variant ? 'Hide questions' : 'Show questions'}
                      </button>

                      {openVariant === item.variant && (
                        <div className="space-y-1 rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-xs text-slate-700">
                          {interviewItems.map((q, index) => (
                            <div
                              key={`${q.skill}-${index}`}
                              className="rounded-md bg-white px-2 py-1"
                            >
                              <p className="text-[11px] font-semibold text-slate-900">
                                {q.skill}
                                {q.difficulty && (
                                  <span className="ml-1 text-[10px] font-normal text-slate-500">
                                    • {q.difficulty}
                                  </span>
                                )}
                              </p>
                              <p className="mt-0.5 text-[11px] text-slate-700">{q.question}</p>
                            </div>
                          ))}
                        </div>
                      )}
                    </div>
                  )}
                </div>
              </Card>
            )
          })}
          </div>

          <div className="flex items-center justify-end gap-3 text-xs text-slate-600">
            <button
              type="button"
              onClick={() => setPage((p) => Math.max(1, p - 1))}
              disabled={page === 1}
              className="rounded-lg border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-700 disabled:cursor-not-allowed disabled:opacity-50"
            >
              Prev
            </button>
            <span>
              Page {page} of {Math.max(1, Math.ceil(variants.length / pageSize))}
            </span>
            <button
              type="button"
              onClick={() =>
                setPage((p) =>
                  p * pageSize >= variants.length ? p : p + 1
                )
              }
              disabled={page * pageSize >= variants.length}
              className="rounded-lg border border-slate-200 bg-white px-3 py-1 text-xs font-medium text-slate-700 disabled:cursor-not-allowed disabled:opacity-50"
            >
              Next
            </button>
          </div>
        </>
      )}
    </section>
  )
}

export default Analysis
