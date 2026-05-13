import type { ReactNode } from "react";

type FilterPanelProps = {
  title?: string;
  description?: string;
  children: ReactNode;
  onSearch: () => void;
  onClear: () => void;
};

export function FilterPanel({
  title = "Filters",
  description,
  children,
  onSearch,
  onClear,
}: FilterPanelProps) {
  return (
    <form
      onSubmit={(e) => {
        e.preventDefault();
        onSearch();
      }}
      className="mb-6 rounded-2xl border bg-white p-4 shadow-sm"
    >
      <div className="mb-4">
        <h2 className="text-base font-semibold text-gray-900">{title}</h2>

        {description && (
          <p className="mt-1 text-sm text-gray-600">{description}</p>
        )}
      </div>

      <div className="grid grid-cols-1 gap-4 md:grid-cols-4">
        {children}

        <div className="flex items-end gap-2">
          <button
            type="submit"
            className="rounded-lg bg-black px-4 py-2 text-sm font-medium text-white"
          >
            Search
          </button>

          <button
            type="button"
            onClick={onClear}
            className="rounded-lg border px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            Clear
          </button>
        </div>
      </div>
    </form>
  );
}
