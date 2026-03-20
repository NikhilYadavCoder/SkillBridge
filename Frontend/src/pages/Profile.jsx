import { useEffect, useState } from 'react'
import { useLocation } from 'react-router-dom'
import axios from 'axios'
import api from '../api/axios.js'
import Card from '../components/ui/Card.jsx'
import Button from '../components/ui/Button.jsx'
import SkillTag from '../components/ui/SkillTag.jsx'

function Profile() {
  const location = useLocation()
  const [profile, setProfile] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [editing, setEditing] = useState(false)
  const [saving, setSaving] = useState(false)

  const [draft, setDraft] = useState({
    name: '',
    email: '',
    skills: [],
  })
  const [newSkill, setNewSkill] = useState('')

  useEffect(() => {
    let isMounted = true

    const fetchProfile = async () => {
      try {
        const token = localStorage.getItem('token')

        const res = await axios.get('https://localhost:7275/api/profile', {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        })

        console.log('PROFILE DATA:', res.data)

        if (!isMounted) return

        const data = res.data || {}

        const normalised = {
          name: data.name || data.fullName || '',
          email: data.email || '',
          skills: Array.isArray(data.skills) ? data.skills : [],
        }

        setProfile(normalised)
        setDraft(normalised)
      } catch (err) {
        console.error('Error fetching profile:', err)
        if (!isMounted) return
        setError('Unable to load profile.')
      } finally {
        if (isMounted) setLoading(false)
      }
    }

    fetchProfile()

    return () => {
      isMounted = false
    }
  }, [])

  const handleFieldChange = (field, value) => {
    setDraft((prev) => ({ ...prev, [field]: value }))
  }

  const handleListChange = (field, index, value) => {
    setDraft((prev) => {
      const list = Array.isArray(prev[field]) ? [...prev[field]] : []
      list[index] = value
      return { ...prev, [field]: list }
    })
  }

  const handleAddListItem = (field) => {
    setDraft((prev) => ({
      ...prev,
      [field]: [...(Array.isArray(prev[field]) ? prev[field] : []), ''],
    }))
  }

  const handleRemoveListItem = (field, index) => {
    setDraft((prev) => {
      const list = Array.isArray(prev[field]) ? [...prev[field]] : []
      list.splice(index, 1)
      return { ...prev, [field]: list }
    })
  }

  const handleAddSkill = () => {
    const trimmed = newSkill.trim()
    if (!trimmed) return
    setDraft((prev) => ({
      ...prev,
      skills: [...(prev.skills || []), trimmed],
    }))
    setNewSkill('')
  }

  const handleRemoveSkill = (skill) => {
    setDraft((prev) => ({
      ...prev,
      skills: (prev.skills || []).filter((s) => s !== skill),
    }))
  }

  const handleCancel = () => {
    if (profile) setDraft(profile)
    setEditing(false)
    setError('')
  }

  const handleSave = async () => {
    setSaving(true)
    setError('')
    try {
      const payload = {
        skills: draft.skills || [],
      }

      await api.put('/api/profile/update', payload)
      setProfile(payload)
      setEditing(false)
    } catch {
      setError('Unable to save profile changes.')
    } finally {
      setSaving(false)
    }
  }

  if (loading) {
    return (
      <div className="max-w-4xl mx-auto px-6 py-8">
        <p className="text-sm text-slate-600">Loading profile…</p>
      </div>
    )
  }

  return (
    <div className="max-w-4xl mx-auto px-6 py-8 space-y-6">
      {location.state?.fromResumeUpload && (
        <div className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-xs text-emerald-700">
          Profile updated from resume
        </div>
      )}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-slate-900">Profile</h1>
          <p className="mt-1 text-sm text-slate-600">
            View and edit the information SkillBridge uses to personalise your
            analysis.
          </p>
        </div>
        <div className="flex items-center gap-3">
          {error && <p className="text-xs text-red-500">{error}</p>}
          {editing ? (
            <>
              <Button
                variant="outline"
                type="button"
                onClick={handleCancel}
                disabled={saving}
              >
                Cancel
              </Button>
              <Button
                variant="primary"
                type="button"
                onClick={handleSave}
                disabled={saving}
              >
                {saving ? 'Saving…' : 'Save'}
              </Button>
            </>
          ) : (
            <Button
              variant="outline"
              type="button"
              onClick={() => setEditing(true)}
            >
              Edit
            </Button>
          )}
        </div>
      </div>

      {/* Basic Info */}
      <Card className="rounded-xl p-6 shadow-sm">
        <h2 className="text-base font-semibold text-slate-900">Basic Info</h2>
        <div className="mt-4 space-y-3 text-sm">
          <div>
            <p className="text-xs font-medium text-slate-500">Name</p>
            {editing ? (
              <input
                type="text"
                value={draft.name}
                onChange={(e) => handleFieldChange('name', e.target.value)}
                className="mt-1 w-full rounded-lg border border-[#E2E8F0] px-3 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
              />
            ) : (
              <p className="mt-1 text-slate-700">{profile?.name || '—'}</p>
            )}
          </div>

          <div>
            <p className="text-xs font-medium text-slate-500">Email</p>
            <p className="mt-1 text-slate-700">{profile?.email || '—'}</p>
          </div>
        </div>
      </Card>

      {/* Skills */}
      <Card className="rounded-xl p-6 shadow-sm">
        <h2 className="text-base font-semibold text-slate-900">Skills</h2>
        <p className="mt-1 text-xs text-slate-500">
          These are skills extracted from your resume. You can adjust them if
          something is missing.
        </p>
        <div className="mt-4 flex flex-wrap gap-2">
          {(draft.skills || []).length === 0 && (
            <p className="text-xs text-slate-500">No skills added yet.</p>
          )}
          {(draft.skills || []).map((skill) => (
            <button
              key={skill}
              type="button"
              onClick={editing ? () => handleRemoveSkill(skill) : undefined}
              className="group"
            >
              <SkillTag>
                <span>{skill}</span>
                {editing && (
                  <span className="ml-1 text-[10px] text-blue-500 group-hover:text-blue-700">
                    ×
                  </span>
                )}
              </SkillTag>
            </button>
          ))}
        </div>
        {editing && (
          <div className="mt-4 flex flex-wrap items-center gap-2 text-sm">
            <input
              type="text"
              value={newSkill}
              onChange={(e) => setNewSkill(e.target.value)}
              placeholder="Add a skill"
              className="w-full max-w-xs rounded-lg border border-[#E2E8F0] px-3 py-2 text-sm text-slate-900 shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
            />
            <Button
              variant="outline"
              type="button"
              onClick={handleAddSkill}
              disabled={!newSkill.trim()}
            >
              Add
            </Button>
          </div>
        )}
      </Card>

      {/* Debug: show raw profile data in development */}
      {import.meta.env.DEV && profile && (
        <pre className="mt-4 rounded-lg bg-slate-50 p-3 text-[10px] text-slate-500 overflow-x-auto">
          {JSON.stringify(profile, null, 2)}
        </pre>
      )}
    </div>
  )
}

export default Profile
