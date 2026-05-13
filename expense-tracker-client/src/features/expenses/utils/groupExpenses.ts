import type { Expense } from "../types/expense.types";

function getDateLabel(date: Date) {
  const today = new Date();
  const yesterday = new Date();
  yesterday.setDate(today.getDate() - 1);

  const isSameDay = (d1: Date, d2: Date) =>
    d1.toDateString() === d2.toDateString();

  if (isSameDay(date, today)) return "Today";
  if (isSameDay(date, yesterday)) return "Yesterday";

  return date.toLocaleDateString();
}

export function groupExpenses(expenses: Expense[]) {
  const groups: Record<string, { items: Expense[]; total: number }> = {};

  expenses.forEach((expense) => {
    const date = new Date(expense.expenseDate);
    const label = getDateLabel(date);

    if (!groups[label]) {
      groups[label] = { items: [], total: 0 };
    }

    groups[label].items.push(expense);
    groups[label].total += expense.amount;
  });

  return groups;
}
