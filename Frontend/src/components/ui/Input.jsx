// Labeled input with optional error text
export function Input({ label, id, error, required, helper, className = '', ...props }) {
  return (
    <div className="space-y-1.5">
      {label && (
        <label htmlFor={id} className="block text-sm font-medium text-slate-700">
          {label}
          {required && <span className="ml-0.5 text-red-500">*</span>}
        </label>
      )}
      <input
        id={id}
        className={`w-full rounded-xl border border-slate-300 bg-white px-3 py-2 text-sm text-slate-900 shadow-sm placeholder:text-slate-400 focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/70 ${
          error ? 'border-red-500 ring-1 ring-red-200' : ''
        } ${className}`}
        {...props}
      />
      {helper && !error && <p className="text-xs text-slate-500">{helper}</p>}
      {error && <p className="text-xs text-red-600">{error}</p>}
    </div>
  );
}

export default Input;
