import type { ExpensePeriod } from "../types/expense.types";

function formatDate(date: Date): string {
  return date.toISOString().slice(0, 10);
}

export function getExpenseDateRange(period: ExpensePeriod) {
  const today = new Date();

  if (period === "all" || period === "custom") {
    return {
      fromDate: "",
      toDate: "",
    };
  }

  if (period === "thisMonth") {
    return {
      fromDate: formatDate(new Date(today.getFullYear(), today.getMonth(), 1)),
      toDate: formatDate(today),
    };
  }

  if (period === "lastMonth") {
    return {
      fromDate: formatDate(
        new Date(today.getFullYear(), today.getMonth() - 1, 1),
      ),
      toDate: formatDate(new Date(today.getFullYear(), today.getMonth(), 0)),
    };
  }

  if (period === "last90") {
    const fromDate = new Date(today);
    fromDate.setDate(today.getDate() - 90);

    return {
      fromDate: formatDate(fromDate),
      toDate: formatDate(today),
    };
  }

  if (period === "thisYear") {
    return {
      fromDate: formatDate(new Date(today.getFullYear(), 0, 1)),
      toDate: formatDate(today),
    };
  }

  const fromDate = new Date(today);
  fromDate.setDate(today.getDate() - 30);

  return {
    fromDate: formatDate(fromDate),
    toDate: formatDate(today),
  };
}
