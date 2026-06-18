import { useMemo } from "react";
import { useSearchParams } from "react-router-dom";
import type { ExpenseFilterValues } from "../components/ExpenseFilters";
import { getExpenseDateRange } from "../utils/expenseDateRanges";
import type { ExpensePeriod } from "../types/expense.types";

function getNumberParam(value: string | null, defaultValue: number) {
  const parsed = Number(value);
  return Number.isNaN(parsed) || parsed <= 0 ? defaultValue : parsed;
}

export function useExpenseFilters() {
  const [searchParams, setSearchParams] = useSearchParams();

  const pageNumber = getNumberParam(searchParams.get("pageNumber"), 1);
  const pageSize = getNumberParam(searchParams.get("pageSize"), 10);

  const appliedFilters: ExpenseFilterValues = useMemo(() => {
    const period =
      (searchParams.get("period") as ExpensePeriod | null) ?? "last30";

    const dateRange = getExpenseDateRange(period);

    return {
      period,
      fromDate: searchParams.get("fromDate") ?? dateRange.fromDate,
      toDate: searchParams.get("toDate") ?? dateRange.toDate,
      categoryId: searchParams.get("categoryId") ?? "",
    };
  }, [searchParams]);

  const queryParams = useMemo(
    () => ({
      pageNumber,
      pageSize,
      sortBy: "date",
      sortDescending: true,
      fromDate: appliedFilters.fromDate || undefined,
      toDate: appliedFilters.toDate || undefined,
      categoryId: appliedFilters.categoryId
        ? Number(appliedFilters.categoryId)
        : undefined,
    }),
    [pageNumber, pageSize, appliedFilters],
  );

  function applyFilters(filters: ExpenseFilterValues) {
    const nextParams = new URLSearchParams();

    nextParams.set("pageNumber", "1");
    nextParams.set("pageSize", String(pageSize));
    nextParams.set("period", filters.period);

    if (filters.period === "custom") {
      if (filters.fromDate) nextParams.set("fromDate", filters.fromDate);
      if (filters.toDate) nextParams.set("toDate", filters.toDate);
    }

    if (filters.categoryId) nextParams.set("categoryId", filters.categoryId);

    setSearchParams(nextParams);
  }

  function clearFilters() {
    const nextParams = new URLSearchParams();

    nextParams.set("pageNumber", "1");
    nextParams.set("pageSize", String(pageSize));
    nextParams.set("period", "last30");

    setSearchParams(nextParams);
  }

  function changePage(newPageNumber: number) {
    const nextParams = new URLSearchParams(searchParams);

    nextParams.set("pageNumber", String(newPageNumber));
    nextParams.set("pageSize", String(pageSize));

    setSearchParams(nextParams);
  }

  return {
    pageNumber,
    pageSize,
    appliedFilters,
    queryParams,
    applyFilters,
    clearFilters,
    changePage,
  };
}
