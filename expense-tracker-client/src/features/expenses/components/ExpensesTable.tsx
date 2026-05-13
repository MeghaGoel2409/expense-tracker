import { Link } from "react-router-dom";
import type { Expense } from "../types/expense.types";

type ExpensesTableProps = {
  expenses: Expense[];
  isDeleting: boolean;
  onDelete: (id: number) => void;
};

function formatDate(value: string) {
  return new Date(value).toLocaleDateString();
}

function formatAmount(amount: number, currency?: string | null) {
  return new Intl.NumberFormat("en-US", {
    style: "currency",
    currency: currency || "USD",
  }).format(amount);
}

export function ExpensesTable({
  expenses,
  isDeleting,
  onDelete,
}: ExpensesTableProps) {
  return (
    <div className="overflow-hidden rounded-2xl border bg-white shadow-sm">
      <table className="w-full text-left text-sm">
        <thead className="border-b bg-gray-50 text-gray-600">
          <tr>
            <th className="px-4 py-3 font-medium">Date</th>
            <th className="px-4 py-3 font-medium">Merchant</th>
            <th className="px-4 py-3 font-medium">Category</th>
            <th className="px-4 py-3 font-medium">Payment</th>
            <th className="px-4 py-3 font-medium">Recurring</th>
            <th className="px-4 py-3 text-right font-medium">Amount</th>
            <th className="px-4 py-3 text-right font-medium">Actions</th>
          </tr>
        </thead>

        <tbody className="divide-y">
          {expenses.map((expense) => (
            <tr key={expense.id} className="hover:bg-gray-50">
              <td className="px-4 py-3">{formatDate(expense.expenseDate)}</td>

              <td className="px-4 py-3">{expense.merchant || "-"}</td>

              <td className="px-4 py-3">
                {expense.categoryName || expense.categoryId}
              </td>

              <td className="px-4 py-3">{expense.paymentMethod || "-"}</td>

              <td className="px-4 py-3">
                {expense.isRecurring ? (
                  <span className="rounded-full bg-gray-100 px-2 py-1 text-xs font-medium text-gray-700">
                    Yes
                  </span>
                ) : (
                  <span className="text-gray-500">No</span>
                )}
              </td>

              <td className="px-4 py-3 text-right font-medium">
                {formatAmount(expense.amount, expense.currency)}
              </td>

              <td className="space-x-2 px-4 py-3 text-right">
                <Link
                  to={`/expenses/${expense.id}/edit`}
                  className="rounded-lg border px-3 py-1 text-xs font-medium text-blue-700 hover:bg-blue-50"
                >
                  Edit
                </Link>

                <button
                  type="button"
                  onClick={() => onDelete(expense.id)}
                  disabled={isDeleting}
                  className="rounded-lg border border-red-200 px-3 py-1 text-xs font-medium text-red-700 hover:bg-red-50 disabled:opacity-60"
                >
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
