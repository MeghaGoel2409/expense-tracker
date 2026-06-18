import type { ComponentType, ReactNode } from "react";

type FilterActionButtonTone = "primary" | "secondary";

type FilterActionButtonProps = {
  type?: "button" | "submit";
  tone?: FilterActionButtonTone;
  icon?: ComponentType<{ className?: string }>;
  children: ReactNode;
  onClick?: () => void;
};

export function FilterActionButton({
  type = "button",
  tone = "secondary",
  icon: Icon,
  children,
  onClick,
}: FilterActionButtonProps) {
  const toneClass =
    tone === "primary"
      ? "bg-gray-900 text-white hover:bg-gray-800"
      : "border border-gray-300 bg-white text-gray-700 hover:bg-gray-50";

  return (
    <button
      type={type}
      onClick={onClick}
      className={`inline-flex h-9 items-center gap-2 rounded-full px-3.5 text-sm font-medium transition ${toneClass}`}
    >
      {Icon && <Icon className="h-4 w-4" />}
      {children}
    </button>
  );
}
