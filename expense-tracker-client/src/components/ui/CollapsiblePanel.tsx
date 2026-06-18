import type { ReactNode } from "react";

type CollapsiblePanelProps = {
  title: string;
  summary?: string;
  isOpen: boolean;
  onToggle: () => void;
  children: ReactNode;
  actionLabel?: string;
};

export function CollapsiblePanel({
  title,
  summary,
  isOpen,
  onToggle,
  children,
  actionLabel,
}: CollapsiblePanelProps) {
  return (
    <section className="mb-4 rounded-xl border border-gray-200 bg-white">
      <button
        type="button"
        onClick={onToggle}
        className="flex w-full items-center justify-between gap-4 px-4 py-3 text-left"
        aria-expanded={isOpen}
      >
        <div>
          <h2 className="text-sm font-semibold text-gray-900">{title}</h2>

          {summary && <p className="mt-0.5 text-sm text-gray-500">{summary}</p>}
        </div>

        <span className="rounded-lg border border-gray-300 px-3 py-1.5 text-sm font-medium text-gray-700 hover:bg-gray-50">
          {actionLabel ?? (isOpen ? "-" : "+")}
        </span>
      </button>

      {isOpen && (
        <div className="border-t border-gray-200 px-4 py-4">{children}</div>
      )}
    </section>
  );
}
