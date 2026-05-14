import { useMemo } from "react";
import { useSearchParams } from "react-router-dom";
import type { ExpenseFilterValues } from "../components/ExpenseFilters";

function getNumberParam(value: string | null, defaultValue: number) {
  const parsed = Number(value);
  return Number.isNaN(parsed) || parsed <= 0 ? defaultValue : parsed;
}

export function useExpenseFilters() {
  const [searchParams, setSearchParams] = useSearchParams();

  const pageNumber = getNumberParam(searchParams.get("pageNumber"), 1);
  const pageSize = getNumberParam(searchParams.get("pageSize"), 10);

  const appliedFilters: ExpenseFilterValues = useMemo(
    () => ({
      fromDate: searchParams.get("fromDate") ?? "",
      toDate: searchParams.get("toDate") ?? "",
      categoryId: searchParams.get("categoryId") ?? "",
    }),
    [searchParams],
  );

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

    if (filters.fromDate) nextParams.set("fromDate", filters.fromDate);
    if (filters.toDate) nextParams.set("toDate", filters.toDate);
    if (filters.categoryId) nextParams.set("categoryId", filters.categoryId);

    setSearchParams(nextParams);
  }

  function clearFilters() {
    const nextParams = new URLSearchParams();

    nextParams.set("pageNumber", "1");
    nextParams.set("pageSize", String(pageSize));

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
