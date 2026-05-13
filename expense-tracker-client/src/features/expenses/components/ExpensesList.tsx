import type { Expense } from "../types/expense.types";
import { groupExpenses } from "../utils/groupExpenses";
import { ExpenseGroup } from "./ExpenseGroup";

type ExpensesListProps = {
  expenses: Expense[];
  isDeleting: boolean;
  onDelete: (id: number) => void;
};

export function ExpensesList({
  expenses,
  isDeleting,
  onDelete,
}: ExpensesListProps) {
  const groupedExpenses = groupExpenses(expenses);

  return (
    <div className="space-y-6">
      {Object.entries(groupedExpenses).map(([label, group]) => (
        <ExpenseGroup
          key={label}
          label={label}
          total={group.total}
          expenses={group.items}
          isDeleting={isDeleting}
          onDelete={onDelete}
        />
      ))}
    </div>
  );
}
