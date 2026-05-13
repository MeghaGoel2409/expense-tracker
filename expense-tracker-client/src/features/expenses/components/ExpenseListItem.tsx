import { Link } from "react-router-dom";
import type { Expense } from "../types/expense.types";
import { getCategoryStyle } from "../utils/categoryStyles";
import { formatCurrency } from "@/lib/utils/formatCurrency";

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
    <div className="rounded-2xl border bg-white p-4 shadow-sm transition hover:shadow-md">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex min-w-0 items-start gap-3">
          <div
            className={`flex h-11 w-11 shrink-0 items-center justify-center rounded-full text-lg ${categoryStyle.className}`}
          >
            {categoryStyle.icon}
          </div>

          <div className="min-w-0">
            <div className="flex flex-wrap items-center gap-2">
              <h3 className="truncate text-sm font-semibold text-gray-900">
                {expense.merchant || "Unnamed Expense"}
              </h3>

              <span
                className={`rounded-full px-2 py-0.5 text-xs font-medium ${categoryStyle.className}`}
              >
                {expense.categoryName || "Uncategorized"}
              </span>

              {expense.isRecurring && (
                <span className="rounded-full bg-blue-50 px-2 py-0.5 text-xs font-medium text-blue-700">
                  Recurring
                </span>
              )}
            </div>

            <p className="mt-1 text-xs text-gray-500">
              {expense.paymentMethod || "No payment method"}
            </p>

            {expense.notes && (
              <p className="mt-2 line-clamp-2 text-sm text-gray-600">
                {expense.notes}
              </p>
            )}
          </div>
        </div>

        <div className="flex shrink-0 items-center justify-between gap-4 sm:justify-end">
          <div className="text-right">
            <p className="text-base font-semibold text-gray-900">
              {formatCurrency(expense.amount, expense.currency)}
            </p>

            <p className="text-xs text-gray-500">{expense.currency}</p>
          </div>

          <div className="flex gap-2">
            <Link
              to={`/expenses/${expense.id}/edit`}
              className="rounded-lg border px-3 py-1.5 text-xs font-medium text-blue-700 hover:bg-blue-50"
            >
              Edit
            </Link>

            <button
              type="button"
              onClick={() => onDelete(expense.id)}
              disabled={isDeleting}
              className="rounded-lg border border-red-200 px-3 py-1.5 text-xs font-medium text-red-700 hover:bg-red-50 disabled:opacity-60"
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
