import { useMutation, useQueryClient } from "@tanstack/react-query";
import { expenseApi } from "../api/expenseApi";
import type { CreateExpenseRequest } from "../types/expense.types";
import { getApiErrorMessage } from "@/lib/api/api-error";
import { notify } from "@/components/ui/toast/notify";

export function useCreateExpense() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: CreateExpenseRequest) => {
      try {
        return await expenseApi.createExpense(request);
      } catch (error) {
        throw new Error(getApiErrorMessage(error, "Failed to create expense."));
      }
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["expenses"] });
      notify.success("Expense created", "Your expense was saved successfully.");
    },
    onError: (error) => {
      notify.error("Could not create expense", error.message);
    },
  });
}
