import { useQuery } from "@tanstack/react-query";
import { expenseApi } from "../api/expenseApi";
import type { ExpenseQueryParams } from "../types/expense.types";
import { getApiErrorMessage } from "@/lib/api/api-error";

export function useExpenses(params?: ExpenseQueryParams) {
  return useQuery({
    queryKey: ["expenses", params],
    queryFn: async () => {
      try {
        return await expenseApi.getExpenses(params);
      } catch (error) {
        throw new Error(getApiErrorMessage(error, "Failed to load expenses."));
      }
    },
  });
}
