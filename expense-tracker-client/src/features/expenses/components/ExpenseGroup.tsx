import type { Expense } from "../types/expense.types";
import { ExpenseListItem } from "./ExpenseListItem";

type ExpenseGroupProps = {
  label: string;
  total: number;
  expenses: Expense[];
  isDeleting: boolean;
  onDelete: (id: number) => void;
};

function formatAmount(amount: number) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: "USD",
  }).format(amount);
}

export function ExpenseGroup({
  label,
  total,
  expenses,
  isDeleting,
  onDelete,
}: ExpenseGroupProps) {
  return (
    <section>
      <div className="sticky top-0 z-10 flex items-center justify-between bg-gray-50 px-1 py-2">
        <h2 className="text-xs font-semibold uppercase tracking-wide text-gray-500">
          {label}
        </h2>

        <span className="text-sm font-semibold text-gray-900">
          {formatAmount(total)}
        </span>
      </div>

      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white divide-y divide-gray-100">
        {expenses.map((expense) => (
          <ExpenseListItem
            key={expense.id}
            expense={expense}
            isDeleting={isDeleting}
            onDelete={onDelete}
          />
        ))}
      </div>
    </section>
  );
}
