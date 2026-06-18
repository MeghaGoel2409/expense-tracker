import type { ComponentType } from "react";
import { Link } from "react-router-dom";

type ActionLinkProps = {
  to: string;
  label: string;
  icon: ComponentType<{ className?: string }>;
};

export function ActionLink({ to, label, icon: Icon }: ActionLinkProps) {
  return (
    <Link
      to={to}
      aria-label={label}
      title={label}
      className="inline-flex items-center gap-1 rounded-md px-2 py-1 text-xs font-medium text-blue-700 transition hover:bg-blue-50"
    >
      <Icon className="h-3.5 w-3.5" />

      <span className="hidden sm:inline">{label}</span>
    </Link>
  );
}
