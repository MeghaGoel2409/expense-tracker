import { useState } from "react";
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

  const fromDate = searchParams.get("fromDate") ?? "";
  const toDate = searchParams.get("toDate") ?? "";
  const categoryId = searchParams.get("categoryId") ?? "";

  const [filterValues, setFilterValues] = useState<ExpenseFilterValues>({
    fromDate,
    toDate,
    categoryId,
  });

  const queryParams = {
    pageNumber,
    pageSize,
    sortBy: "date",
    sortDescending: true,
    fromDate: fromDate || undefined,
    toDate: toDate || undefined,
    categoryId: categoryId ? Number(categoryId) : undefined,
  };

  const applyFilters = () => {
    const nextParams = new URLSearchParams();

    nextParams.set("pageNumber", "1");
    nextParams.set("pageSize", String(pageSize));

    if (filterValues.fromDate) {
      nextParams.set("fromDate", filterValues.fromDate);
    }

    if (filterValues.toDate) {
      nextParams.set("toDate", filterValues.toDate);
    }

    if (filterValues.categoryId) {
      nextParams.set("categoryId", filterValues.categoryId);
    }

    setSearchParams(nextParams);
  };

  const clearFilters = () => {
    setFilterValues({
      fromDate: "",
      toDate: "",
      categoryId: "",
    });

    setSearchParams({
      pageNumber: "1",
      pageSize: String(pageSize),
    });
  };

  const changePage = (newPageNumber: number) => {
    const nextParams = new URLSearchParams(searchParams);

    nextParams.set("pageNumber", String(newPageNumber));
    nextParams.set("pageSize", String(pageSize));

    setSearchParams(nextParams);
  };

  return {
    pageNumber,
    pageSize,
    filterValues,
    setFilterValues,
    queryParams,
    applyFilters,
    clearFilters,
    changePage,
  };
}
