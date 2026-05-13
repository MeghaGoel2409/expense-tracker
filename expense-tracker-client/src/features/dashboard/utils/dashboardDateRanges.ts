export type DashboardRange = "thisMonth" | "lastMonth" | "last3Months";

function toDateOnlyString(date: Date): string {
  return date.toISOString().split("T")[0];
}

export function getDashboardDateRange(range: DashboardRange) {
  const today = new Date();

  if (range === "thisMonth") {
    const fromDate = new Date(today.getFullYear(), today.getMonth(), 1);

    return {
      fromDate: toDateOnlyString(fromDate),
      toDate: toDateOnlyString(today),
    };
  }

  if (range === "lastMonth") {
    const fromDate = new Date(today.getFullYear(), today.getMonth() - 1, 1);
    const toDate = new Date(today.getFullYear(), today.getMonth(), 0);

    return {
      fromDate: toDateOnlyString(fromDate),
      toDate: toDateOnlyString(toDate),
    };
  }

  const fromDate = new Date(today.getFullYear(), today.getMonth() - 2, 1);

  return {
    fromDate: toDateOnlyString(fromDate),
    toDate: toDateOnlyString(today),
  };
}
