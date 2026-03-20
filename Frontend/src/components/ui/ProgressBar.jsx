// Simple progress bar; value is 0-100
export function ProgressBar({ value }) {
  let widthClass = 'w-1/4';
  if (value >= 90) widthClass = 'w-[90%]';
  else if (value >= 75) widthClass = 'w-3/4';
  else if (value >= 50) widthClass = 'w-1/2';
  else if (value >= 30) widthClass = 'w-1/3';

  return (
    <div className="h-2 w-full overflow-hidden rounded-full bg-slate-100">
      <div
        className={`h-full rounded-full bg-gradient-to-r from-blue-500 to-sky-400 ${widthClass}`}
      />
    </div>
  );
}

export default ProgressBar;
