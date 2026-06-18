import type { ComponentType } from "react";

type ActionButtonTone = "default" | "danger";

type ActionButtonProps = {
  label: string;
  icon: ComponentType<{ className?: string }>;
  tone?: ActionButtonTone;
  disabled?: boolean;
  onClick?: () => void;
};

export function ActionButton({
  label,
  icon: Icon,
  tone = "default",
  disabled = false,
  onClick,
}: ActionButtonProps) {
  const toneClass =
    tone === "danger"
      ? "text-red-700 hover:bg-red-50"
      : "text-blue-700 hover:bg-blue-50";

  return (
    <button
      type="button"
      onClick={onClick}
      disabled={disabled}
      aria-label={label}
      title={label}
      className={`inline-flex items-center gap-1 rounded-md px-2 py-1 text-xs font-medium transition disabled:cursor-not-allowed disabled:opacity-50 ${toneClass}`}
    >
      <Icon className="h-3.5 w-3.5" />

      <span className="hidden sm:inline">{label}</span>
    </button>
  );
}
