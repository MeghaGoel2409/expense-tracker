import { useQuery } from "@tanstack/react-query";
import { expenseApi } from "../api/expenseApi";
import { getApiErrorMessage } from "@/lib/api/api-error";

export function useExpense(id: number) {
  return useQuery({
    queryKey: ["expense", id],
    queryFn: async () => {
      try {
        return await expenseApi.getExpenseById(id);
      } catch (error) {
        throw new Error(getApiErrorMessage(error, "Failed to load expenses."));
      }
    },
    enabled: !!id,
  });
}
