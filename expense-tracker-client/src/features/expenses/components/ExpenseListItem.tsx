import type { Expense } from "../types/expense.types";
import { getCategoryStyle } from "../utils/categoryStyles";
import { formatCurrency } from "@/lib/utils/formatCurrency";
import { ActionButton } from "@/components/ui/ActionButton";
import { Pencil, Trash2 } from "lucide-react";
import { ActionLink } from "@/components/ui/ActionLink";

type ExpenseListItemProps = {
  expense: Expense;
  isDeleting: boolean;
  onDelete: (id: number) => void;
};

export function ExpenseListItem({
  expense,
  isDeleting,
  onDelete,
}: ExpenseListItemProps) {
  const categoryStyle = getCategoryStyle(expense.categoryName);

  return (
    <div className="group grid gap-3 px-4 py-3 transition hover:bg-gray-50 sm:grid-cols-[1fr_auto] sm:items-center">
      <div className="flex min-w-0 items-center gap-3">
        <div
          className={`flex h-8 w-8 shrink-0 items-center justify-center rounded-full text-sm ${categoryStyle.className}`}
        >
          {categoryStyle.icon}
        </div>

        <div className="min-w-0">
          <div className="flex min-w-0 items-center gap-2">
            <h3 className="truncate text-sm font-medium text-gray-900">
              {expense.merchant || "Unnamed Expense"}
            </h3>

            {expense.isRecurring && (
              <span className="shrink-0 rounded-full bg-blue-50 px-2 py-0.5 text-[11px] font-medium text-blue-700">
                Recurring
              </span>
            )}
          </div>

          <div className="mt-0.5 flex flex-wrap items-center gap-x-2 gap-y-1 text-xs text-gray-500">
            <span>{expense.categoryName || "Uncategorized"}</span>

            {expense.paymentMethod && (
              <>
                <span>•</span>
                <span>{expense.paymentMethod}</span>
              </>
            )}

            {expense.notes && (
              <>
                <span>•</span>
                <span className="max-w-md truncate">{expense.notes}</span>
              </>
            )}
          </div>
        </div>
      </div>

      <div className="flex items-center justify-between gap-3 sm:justify-end">
        <div className="text-right">
          <p className="text-sm font-semibold text-gray-900">
            {formatCurrency(expense.amount, expense.currency)}
          </p>
        </div>

        <div className="flex shrink-0 items-center gap-1">
          <ActionLink
            to={`/expense/${expense.id}/edit`}
            label="Edit"
            icon={Pencil}
          />

          <ActionButton
            label="Delete"
            icon={Trash2}
            tone="danger"
            disabled={isDeleting}
            onClick={() => onDelete(expense.id)}
          />
        </div>
      </div>
    </div>
  );
}
