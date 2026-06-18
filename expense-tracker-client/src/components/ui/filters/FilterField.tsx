import type { ReactNode } from "react";

type FilterFieldProps = {
  label?: string;
  children: ReactNode;
};

export function FilterField({ label, children }: FilterFieldProps) {
  return (
    <div>
      {label && (
        <label className="mb-1 block text-sm font-medium text-gray-700">
          {label}
        </label>
      )}

      {children}
    </div>
  );
}
