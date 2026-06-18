import type { ReactNode } from "react";

type FilterPanelProps = {
  children: ReactNode;
  actions?: ReactNode;
  onSearch: () => void;
};

export function FilterPanel({ children, actions, onSearch }: FilterPanelProps) {
  return (
    <form
      onSubmit={(event) => {
        event.preventDefault();
        onSearch();
      }}
      className="mb-4 rounded-xl border border-gray-200 bg-white px-4 py-3 shadow-sm"
    >
      <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
        <div className="min-w-0 flex-1">{children}</div>

        {actions && (
          <div className="flex shrink-0 flex-wrap items-center gap-2">
            {actions}
          </div>
        )}
      </div>
    </form>
  );
}
