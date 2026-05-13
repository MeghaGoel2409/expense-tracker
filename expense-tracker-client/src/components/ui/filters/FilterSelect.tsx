import { FilterField } from "./FilterField";

type FilterSelectOption = {
  label: string;
  value: string;
};

type FilterSelectProps = {
  label: string;
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
  placeholder = "All",
  disabled = false,
  onChange,
}: FilterSelectProps) {
  return (
    <FilterField label={label}>
      <select
        value={value}
        disabled={disabled}
        onChange={(e) => onChange(e.target.value)}
        className="w-full rounded-lg border px-3 py-2 text-sm disabled:bg-gray-100"
      >
        <option value="">{placeholder}</option>

        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </FilterField>
  );
}
