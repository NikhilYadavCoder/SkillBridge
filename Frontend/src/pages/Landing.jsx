import { Link } from 'react-router-dom'
import Card from '../components/ui/Card.jsx'
import Button from '../components/ui/Button.jsx'
import SkillTag from '../components/ui/SkillTag.jsx'
import ProgressBar from '../components/ui/ProgressBar.jsx'

function Landing() {
  return (
    <div className="space-y-16 pb-10 pt-6">
      {/* Hero */}
      <section className="grid items-center gap-10 rounded-3xl bg-gradient-to-br from-blue-50 to-white px-8 py-10 md:grid-cols-2">
        <div className="space-y-6">
          <p className="text-sm font-semibold uppercase tracking-[0.22em] text-blue-600">
            AI-powered career assistant
          </p>
          <h1 className="text-3xl font-semibold tracking-tight text-slate-900 sm:text-4xl lg:text-5xl">
            Turn your resume into a{' '}
            <span className="text-blue-600">job-ready roadmap</span>
          </h1>
          <p className="max-w-xl text-base text-slate-600">
            Upload your resume, discover missing skills, and get a personalised
            learning path with AI-powered insights. SkillBridge keeps everything
            in one workspace so you&apos;re always ready for the next opportunity.
          </p>

          <div className="flex flex-wrap items-center gap-3 pt-2">
            <Link to="/register">
              <Button variant="primary">Get Started</Button>
            </Link>
            <Link to="/login">
              <Button variant="outline">Login</Button>
            </Link>
          </div>

          <div className="mt-4 flex flex-wrap gap-2 text-xs text-slate-600">
            <span className="rounded-full bg-slate-100 px-3 py-1">
              Upload PDF resume
            </span>
            <span className="rounded-full bg-slate-100 px-3 py-1">
              AI skill analysis
            </span>
            <span className="rounded-full bg-slate-100 px-3 py-1">
              Roadmap & interviews
            </span>
          </div>
        </div>

        <div className="relative">
          <div className="absolute inset-x-6 bottom-0 top-8 rounded-3xl bg-gradient-to-br from-blue-500/10 via-sky-400/10 to-blue-600/10" />
          <div className="relative grid gap-4 p-4 sm:p-5">
            <Card className="col-span-2 transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
              <div className="flex items-center justify-between gap-4">
                <div>
                  <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
                    Match score
                  </p>
                  <p className="mt-1 text-3xl font-semibold text-slate-900">82%</p>
                  <p className="text-xs text-slate-500">Senior .NET Developer</p>
                </div>
                <div className="w-32">
                  <ProgressBar value={82} />
                </div>
              </div>
            </Card>

            <Card className="space-y-3 transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
              <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
                Missing skills
              </p>
              <div className="flex flex-wrap gap-2">
                <SkillTag>Docker</SkillTag>
                <SkillTag>Azure</SkillTag>
                <SkillTag>System design</SkillTag>
              </div>
            </Card>

            <Card className="space-y-3 transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
              <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
                Next steps
              </p>
              <ul className="space-y-1 text-xs text-slate-600">
                <li>• Complete Docker fundamentals track</li>
                <li>• Practise REST API design questions</li>
                <li>• Review Azure deployment patterns</li>
              </ul>
            </Card>
          </div>
        </div>
      </section>

      {/* Features */}
      <section className="space-y-6">
        <div className="space-y-2 text-center">
          <p className="text-xs font-semibold uppercase tracking-[0.22em] text-blue-600">
            Features
          </p>
          <h2 className="text-2xl font-semibold text-slate-900">
            Everything you need to get interview-ready
          </h2>
          <p className="text-sm text-slate-600">
            SkillBridge turns your existing resume into a living career
            workspace.
          </p>
        </div>
        <div className="grid gap-6 md:grid-cols-3">
          <Card className="h-full rounded-xl p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
            <h3 className="text-base font-semibold text-slate-900">
              Resume Analysis
            </h3>
            <p className="mt-2 text-sm text-slate-600">
              Upload a PDF resume and let SkillBridge extract skills,
              experience, projects and certifications automatically.
            </p>
          </Card>
          <Card className="h-full rounded-xl p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
            <h3 className="text-base font-semibold text-slate-900">
              Skill Gap Detection
            </h3>
            <p className="mt-2 text-sm text-slate-600">
              Compare your profile to target roles and see exactly which skills
              to focus on next.
            </p>
          </Card>
          <Card className="h-full rounded-xl p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
            <h3 className="text-base font-semibold text-slate-900">
              Roadmap & Interview Prep
            </h3>
            <p className="mt-2 text-sm text-slate-600">
              Generate a personalised learning path and role-specific interview
              questions to practise with.
            </p>
          </Card>
        </div>
      </section>

      {/* How it works */}
      <section className="space-y-6">
        <div className="space-y-2 text-center">
          <p className="text-xs font-semibold uppercase tracking-[0.22em] text-blue-600">
            How it works
          </p>
          <h2 className="text-2xl font-semibold text-slate-900">
            From resume to roadmap in four steps
          </h2>
        </div>
        <div className="grid gap-6 md:grid-cols-4">
          {[
            {
              step: '1',
              title: 'Upload Resume',
              body: 'Drop in your existing resume in PDF format.',
            },
            {
              step: '2',
              title: 'Extract Skills',
              body: 'We identify skills, experience and projects automatically.',
            },
            {
              step: '3',
              title: 'Analyse Role Fit',
              body: 'See match %, variants and missing skills for target roles.',
            },
            {
              step: '4',
              title: 'Get Roadmap & Interviews',
              body: 'Follow a concrete learning path and practise interview questions.',
            },
          ].map((item) => (
            <Card
              key={item.step}
              className="h-full rounded-xl p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md"
            >
              <div className="mb-2 inline-flex h-7 w-7 items-center justify-center rounded-full bg-blue-50 text-xs font-semibold text-blue-700">
                {item.step}
              </div>
              <h3 className="text-sm font-semibold text-slate-900">{item.title}</h3>
              <p className="mt-1 text-xs text-slate-600">{item.body}</p>
            </Card>
          ))}
        </div>
      </section>

      {/* Example dashboard preview */}
      <section className="space-y-4">
        <div className="flex flex-col justify-between gap-2 sm:flex-row sm:items-end">
          <div>
            <p className="text-xs font-semibold uppercase tracking-[0.22em] text-blue-600">
              Preview
            </p>
            <h2 className="text-2xl font-semibold text-slate-900">
              A focused, simple candidate dashboard
            </h2>
          </div>
          <Link to="/dashboard" className="text-sm font-medium text-blue-600">
            View your workspace →
          </Link>
        </div>

        <div className="grid gap-6 md:grid-cols-3">
          <Card className="rounded-xl p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
            <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
              Match %
            </p>
            <p className="mt-2 text-3xl font-semibold text-slate-900">82%</p>
            <p className="text-xs text-slate-500">Senior .NET Developer</p>
          </Card>
          <Card className="space-y-3 rounded-xl p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
            <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
              Missing skills
            </p>
            <div className="flex flex-wrap gap-2">
              <SkillTag>Distributed systems</SkillTag>
              <SkillTag>Kubernetes</SkillTag>
              <SkillTag>Terraform</SkillTag>
            </div>
          </Card>
          <Card className="space-y-3 rounded-xl p-6 shadow-sm transition-all duration-200 hover:-translate-y-0.5 hover:shadow-md">
            <p className="text-xs font-medium uppercase tracking-wide text-slate-500">
              Roadmap preview
            </p>
            <ul className="space-y-1 text-xs text-slate-600">
              <li>• Week 1–2: Refresh C# and .NET APIs</li>
              <li>• Week 3–4: Cloud + deployment fundamentals</li>
              <li>• Week 5: System design practice</li>
            </ul>
          </Card>
        </div>
      </section>
    </div>
  )
}

export default Landing
