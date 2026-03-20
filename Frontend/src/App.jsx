import { Navigate, Route, Routes } from 'react-router-dom'
import { useAuth } from './context/AuthContext.jsx'
import Navbar from './components/Navbar.jsx'
import Landing from './pages/Landing.jsx'
import Login from './pages/Login.jsx'
import Register from './pages/Register.jsx'
import Dashboard from './pages/Dashboard.jsx'
import UploadResume from './pages/UploadResume.jsx'
import Analysis from './pages/Analysis.jsx'
import Profile from './pages/Profile.jsx'

function App() {
  const { token } = useAuth()

  return (
    <div className="min-h-screen bg-[#F8FAFC] text-slate-900">
      <Navbar />
      <main className="mx-auto max-w-6xl px-6 py-10 space-y-10">
        <Routes>
          <Route path="/" element={<Landing />} />
          <Route
            path="/login"
            element={token ? <Navigate to="/dashboard" replace /> : <Login />}
          />
          <Route
            path="/register"
            element={
              token ? <Navigate to="/dashboard" replace /> : <Register />
            }
          />
          <Route
            path="/dashboard"
            element={token ? <Dashboard /> : <Navigate to="/login" replace />}
          />
          <Route
            path="/upload-resume"
            element={token ? <UploadResume /> : <Navigate to="/login" replace />}
          />
          <Route
            path="/analysis"
            element={token ? <Analysis /> : <Navigate to="/login" replace />}
          />
          <Route
            path="/profile"
            element={token ? <Profile /> : <Navigate to="/login" replace />}
          />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </main>
    </div>
  )
}

export default App
