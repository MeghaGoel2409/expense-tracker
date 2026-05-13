import type { Expense } from "../types/expense.types";

export function getExpenseSummary(expenses: Expense[]) {
  const total = expenses.reduce((sum, expense) => sum + expense.amount, 0);

  const recurringTotal = expenses
    .filter((expense) => expense.isRecurring)
    .reduce((sum, expense) => sum + expense.amount, 0);

  const nonRecurringTotal = total - recurringTotal;

  return {
    total,
    recurringTotal,
    nonRecurringTotal,
    count: expenses.length,
  };
}
