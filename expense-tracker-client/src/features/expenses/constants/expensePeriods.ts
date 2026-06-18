import type { ExpensePeriod } from "../types/expense.types";

export const expensePeriodOptions = [
  { label: "Last 30 days", value: "last30" },
  { label: "This month", value: "thisMonth" },
  { label: "Last month", value: "lastMonth" },
  { label: "Last 90 days", value: "last90" },
  { label: "This year", value: "thisYear" },
  { label: "All time", value: "all" },
  { label: "Custom", value: "custom" },
] satisfies ReadonlyArray<{
  label: string;
  value: ExpensePeriod;
}>;

export function getExpensePeriodLabel(period: ExpensePeriod): string {
  const selectedOption = expensePeriodOptions.find(
    (option) => option.value === period,
  );

  return selectedOption?.label ?? "Last 30 days";
}

export const DEFAULT_EXPENSE_PERIOD: ExpensePeriod = "last30";
