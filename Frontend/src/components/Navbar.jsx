import { NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext.jsx'
import Button from './ui/Button.jsx'

function Navbar() {
  const { token, user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const linkBase =
    'text-sm font-medium text-slate-600 hover:text-slate-900 transition-colors';

  return (
    <header className="sticky top-0 z-30 border-b border-[#E2E8F0] bg-white/90 backdrop-blur">
      <div className="mx-auto flex max-w-6xl items-center px-8 py-4">
        {/* Left: Logo */}
        <div className="flex flex-1 items-center gap-2">
          <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-gradient-to-br from-blue-500 to-sky-400 text-white shadow-soft">
            <span className="text-lg font-semibold">SB</span>
          </div>
          <div className="leading-tight">
            <div className="text-base font-semibold tracking-tight text-slate-900">
              SkillBridge
            </div>
            <div className="text-xs text-slate-500">AI career copilot</div>
          </div>
        </div>

        {/* Center: Menu */}
        <nav
          className="hidden flex-1 items-center justify-center gap-6 md:flex"
          aria-label="Main"
        >
          <NavLink
            to="/"
            className={({ isActive }) =>
              `${linkBase} ${isActive ? 'text-slate-900' : ''}`
            }
          >
            Home
          </NavLink>
          {token && (
            <NavLink
              to="/dashboard"
              className={({ isActive }) =>
                `${linkBase} ${isActive ? 'text-slate-900' : ''}`
              }
            >
              Dashboard
            </NavLink>
          )}
        </nav>

        {/* Right: Actions */}
        <div className="flex flex-1 items-center justify-end gap-3">
          {token ? (
            <>
              <span className="hidden text-xs text-slate-500 sm:inline">
                {user?.name || user?.email}
              </span>
              <Button variant="outline" type="button" onClick={handleLogout}>
                Logout
              </Button>
            </>
          ) : (
            <>
              <NavLink to="/login" className={linkBase}>
                Login
              </NavLink>
              <NavLink to="/register">
                <Button variant="primary" type="button">
                  Get Started
                </Button>
              </NavLink>
            </>
          )}
        </div>
      </div>
    </header>
  )
}

export default Navbar
