import type { Expense } from "../types/expense.types";
import { getExpenseSummary } from "../utils/expenseSummary";

type ExpenseSummaryCardsProps = {
  expenses: Expense[];
};

function formatAmount(amount: number) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
  }).format(amount);
}

export function ExpenseSummaryCards({ expenses }: ExpenseSummaryCardsProps) {
  const summary = getExpenseSummary(expenses);

  return (
    <div className="mb-6 grid grid-cols-1 gap-4 md:grid-cols-3">
      <div className="rounded-2xl border bg-white p-5 shadow-sm">
        <p className="text-sm text-gray-500">Total Spent</p>
        <p className="mt-2 text-2xl font-semibold text-gray-900">
          {formatAmount(summary.total)}
        </p>
        <p className="mt-1 text-xs text-gray-500">
          {summary.count} transactions
        </p>
      </div>

      <div className="rounded-2xl border bg-white p-5 shadow-sm">
        <p className="text-sm text-gray-500">Recurring</p>
        <p className="mt-2 text-2xl font-semibold text-gray-900">
          {formatAmount(summary.recurringTotal)}
        </p>
        <p className="mt-1 text-xs text-gray-500">
          Bills and repeating expenses
        </p>
      </div>

      <div className="rounded-2xl border bg-white p-5 shadow-sm">
        <p className="text-sm text-gray-500">One-time</p>
        <p className="mt-2 text-2xl font-semibold text-gray-900">
          {formatAmount(summary.nonRecurringTotal)}
        </p>
        <p className="mt-1 text-xs text-gray-500">Regular purchases</p>
      </div>
    </div>
  );
}
