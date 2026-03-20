// Simple card container
export function Card({ className = '', children }) {
  return (
    <div className={`rounded-2xl border border-slate-200 bg-white shadow-soft ${className}`}>
      {children}
    </div>
  );
}

export default Card;
