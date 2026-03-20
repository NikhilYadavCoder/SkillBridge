// Reusable button component
export function Button({ variant = 'primary', className = '', children, ...props }) {
  const base =
    'inline-flex items-center justify-center rounded-full px-4 py-2 text-sm font-medium transition-colors duration-150 disabled:cursor-not-allowed disabled:opacity-60 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500 focus-visible:ring-offset-2 focus-visible:ring-offset-slate-50';

  const variants = {
    primary: 'bg-blue-600 text-white hover:bg-blue-700 shadow-sm',
    outline:
      'border border-slate-300 bg-white text-slate-900 hover:bg-slate-50 shadow-sm',
    ghost: 'text-slate-700 hover:bg-slate-50',
  };

  return (
    <button className={`${base} ${variants[variant]} ${className}`} {...props}>
      {children}
    </button>
  );
}

export default Button;
