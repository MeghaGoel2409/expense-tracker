import { FilterField } from "./FilterField";

type FilterDateProps = {
  label: string;
  value: string;
  onChange: (value: string) => void;
};

export function FilterDate({ label, value, onChange }: FilterDateProps) {
  return (
    <FilterField label={label}>
      <input
        type="date"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        className="w-full rounded-lg border px-3 py-2 text-sm"
      />
    </FilterField>
  );
}
