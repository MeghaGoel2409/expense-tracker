import type { InputHTMLAttributes } from "react";

type AuthTextFieldProps = {
  label: string;
  error?: string;
} & InputHTMLAttributes<HTMLInputElement>;

export function AuthTextField({
  id,
  label,
  error,
  className = "",
  ...inputProps
}: AuthTextFieldProps) {
  return (
    <div>
      <label
        htmlFor={id}
        className="mb-1 block text-sm font-medium text-gray-700"
      >
        {label}
      </label>

      <input
        id={id}
        {...inputProps}
        className={`w-full rounded-lg border px-3 py-2 outline-none focus:border-black ${
          error ? "border-red-400" : "border-gray-300"
        } ${className}`}
      />

      {error && <p className="mt-1 text-sm text-red-600">{error}</p>}
    </div>
  );
}
