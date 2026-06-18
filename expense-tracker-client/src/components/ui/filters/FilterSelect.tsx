import { ChevronDown, Tag } from "lucide-react";
import { FilterField } from "./FilterField";

type FilterSelectOption = {
  label: string;
  value: string;
};

type FilterSelectProps = {
  label?: string;
  value: string;
  options: FilterSelectOption[];
  placeholder?: string;
  disabled?: boolean;
  onChange: (value: string) => void;
};

export function FilterSelect({
  label,
  value,
  options,
  placeholder = "All Categories",
  disabled = false,
  onChange,
}: FilterSelectProps) {
  return (
    <FilterField label={label}>
      <div className="relative">
        <Tag className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-500" />

        <select
          value={value}
          disabled={disabled}
          onChange={(event) => onChange(event.target.value)}
          className="h-9 w-full appearance-none rounded-full border border-gray-300 bg-white pl-9 pr-24 text-sm font-medium text-gray-700 transition hover:bg-gray-50 focus:border-gray-900 focus:outline-none focus:ring-1 focus:ring-gray-900 disabled:cursor-not-allowed disabled:bg-gray-100 disabled:text-gray-400"
        >
          <option value="">Category: {placeholder}</option>

          {options.map((option) => (
            <option key={option.value} value={option.value}>
              Category: {option.label}
            </option>
          ))}
        </select>

        <ChevronDown className="pointer-events-none absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-500" />
      </div>
    </FilterField>
  );
}
