import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import api from '../api/axios.js'
import Card from '../components/ui/Card.jsx'
import Button from '../components/ui/Button.jsx'

function Dashboard() {
  const { user } = useAuth()

  const [profile, setProfile] = useState(null)
  const [loadingProfile, setLoadingProfile] = useState(true)
  const [profileError, setProfileError] = useState('')

  useEffect(() => {
    let isMounted = true

    const fetchProfile = async () => {
      try {
        const response = await api.get('/api/profile')
        if (!isMounted) return
        setProfile(response.data || null)
      } catch {
        if (!isMounted) return
        setProfileError('Unable to load profile details.')
      } finally {
        if (isMounted) setLoadingProfile(false)
      }
    }

    fetchProfile()

    return () => {
      isMounted = false
    }
  }, [])

  const displayName =
    profile?.name || profile?.fullName || user?.name || 'Your SkillBridge workspace'
  const email = profile?.email || user?.email

  return (
    <section className="space-y-6">
      <Card className="rounded-xl p-6 shadow-sm">
        <p className="inline-flex items-center rounded-full bg-blue-50 px-3 py-1 text-xs font-medium text-blue-700">
          Welcome back
        </p>
        <h2 className="mt-3 text-2xl font-semibold text-slate-900">
          Welcome back, {displayName}
        </h2>
        {loadingProfile ? (
          <p className="mt-1 text-xs text-slate-500">Loading your profile…</p>
        ) : profileError ? (
          <p className="mt-1 text-xs text-red-500">{profileError}</p>
        ) : (
          email && (
            <p className="mt-1 text-sm text-slate-500">Signed in as {email}</p>
          )
        )}
        <p className="mt-2 max-w-2xl text-sm text-slate-600">
          This is your starting point. Upload your resume, analyse your skills
          against target roles and follow a focused roadmap to get interview-ready.
        </p>
      </Card>

      <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
        <Card className="flex flex-col justify-between rounded-xl p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
          <div>
            <h3 className="text-base font-semibold text-slate-900">Upload Resume</h3>
            <p className="mt-1 text-sm text-slate-600">
              Start by uploading your latest resume so SkillBridge can analyse
              your skills and experience.
            </p>
          </div>
          <div className="mt-4">
            <Link to="/upload-resume">
              <Button variant="primary">Go to Upload</Button>
            </Link>
          </div>
        </Card>

        <Card className="flex flex-col justify-between rounded-xl p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
          <div>
            <h3 className="text-base font-semibold text-slate-900">Analyse Skills</h3>
            <p className="mt-1 text-sm text-slate-600">
              Review match %, missing skills and role variants, then generate a
              tailored learning roadmap.
            </p>
          </div>
          <div className="mt-4">
            <Link to="/analysis">
              <Button variant="outline">View Analysis</Button>
            </Link>
          </div>
        </Card>

        <Card className="flex flex-col justify-between rounded-xl p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
          <div>
            <h3 className="text-base font-semibold text-slate-900">View Profile</h3>
            <p className="mt-1 text-sm text-slate-600">
              Review your saved details and skill profile. Manage the
              information SkillBridge uses for analysis.
            </p>
          </div>
          <div className="mt-4">
            <Link to="/profile">
              <Button variant="outline" type="button">
                View Profile
              </Button>
            </Link>
          </div>
        </Card>
      </div>
    </section>
  )
}

export default Dashboard
