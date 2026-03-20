import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../api/axios.js'
import Card from '../components/ui/Card.jsx'
import Button from '../components/ui/Button.jsx'

function UploadResume() {
  const navigate = useNavigate()
  const [file, setFile] = useState(null)
  const [fileName, setFileName] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  const handleChange = (event) => {
    const file = event.target.files?.[0]
    if (file) {
      setFile(file)
      setFileName(file.name)
    }
  }

  const handleDrop = (event) => {
    event.preventDefault()
    const file = event.dataTransfer.files?.[0]
    if (file) {
      setFile(file)
      setFileName(file.name)
    }
  }

  const handleDragOver = (event) => {
    event.preventDefault()
  }

  const handleUpload = async () => {
    if (!file) return
    setLoading(true)
    setError('')
    try {
      const formData = new FormData()
      formData.append('file', file)

      await api.post('/api/resume/upload', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      })

      // After successful upload, navigate to analysis where
      // you can pick a role and see your match.
      navigate('/analysis', { state: { fromResumeUpload: true } })
    } catch {
      setError('Unable to upload resume. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <section className="flex justify-center">
      <Card className="w-full max-w-xl rounded-xl border-2 border-dashed border-[#E2E8F0] bg-white p-8 text-center shadow-sm">
        <h1 className="text-xl font-semibold text-slate-900">Upload your resume</h1>
        <p className="mt-2 text-sm text-slate-600">
          Drag and drop a PDF file, or browse from your computer. We&apos;ll use
          this to analyse your skills and generate a roadmap.
        </p>

        <div
          className="mt-6 flex cursor-pointer flex-col items-center justify-center rounded-2xl border border-dashed border-slate-300 bg-slate-50/80 px-6 py-10 text-center transition-all duration-200 hover:border-blue-400 hover:bg-blue-50/60"
          onDrop={handleDrop}
          onDragOver={handleDragOver}
        >
          <p className="text-sm font-medium text-slate-900">Drag &amp; drop your resume here</p>
          <p className="mt-1 text-xs text-slate-500">PDF files are recommended</p>
          <label className="mt-4 inline-flex cursor-pointer items-center rounded-full bg-white px-4 py-2 text-xs font-medium text-blue-600 shadow-sm ring-1 ring-slate-200 hover:bg-slate-50">
            <span>Browse files</span>
            <input type="file" accept="application/pdf" className="hidden" onChange={handleChange} />
          </label>
          {fileName && (
            <p className="mt-3 text-xs text-slate-600">Selected file: {fileName}</p>
          )}
        </div>

        {error && (
          <p className="mt-3 text-xs text-red-500">{error}</p>
        )}

        <div className="mt-6 flex justify-center">
          <Button
            variant="primary"
            type="button"
            disabled={!file || loading}
            className="w-full max-w-xs"
            onClick={handleUpload}
          >
            {loading ? 'Uploading…' : 'Upload resume'}
          </Button>
        </div>
      </Card>
    </section>
  )
}

export default UploadResume
