type FilterChipOption<TValue extends string> = {
  label: string;
  value: TValue;
};

type FilterChipGroupProps<TValue extends string> = {
  value: TValue;
  options: readonly FilterChipOption<TValue>[];
  onChange: (value: TValue) => void;
};

export function FilterChipGroup<TValue extends string>({
  value,
  options,
  onChange,
}: FilterChipGroupProps<TValue>) {
  return (
    <div className="flex flex-wrap items-center gap-2">
      {options.map((option) => {
        const isSelected = option.value === value;

        return (
          <button
            key={option.value}
            type="button"
            onClick={() => onChange(option.value)}
            className={
              isSelected
                ? "inline-flex h-9 items-center rounded-full bg-gray-900 px-3.5 text-sm font-medium text-white shadow-sm"
                : "inline-flex h-9 items-center rounded-full border border-gray-300 bg-white px-3.5 text-sm font-medium text-gray-700 transition hover:bg-gray-50"
            }
          >
            {option.label}
          </button>
        );
      })}
    </div>
  );
}
