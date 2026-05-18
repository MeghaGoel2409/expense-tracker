import { useMutation, useQueryClient } from "@tanstack/react-query";
import { expenseApi } from "../api/expenseApi";
import type { UpdateExpenseRequest } from "../types/expense.types";
import { getApiErrorMessage } from "@/lib/api/api-error";
import { notify } from "@/components/ui/toast/notify";

export function useUpdateExpense() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (request: UpdateExpenseRequest) => {
      try {
        return await expenseApi.updateExpense(request.id, request);
      } catch (error) {
        throw new Error(getApiErrorMessage(error, "Failed to update expense."));
      }
    },
    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["expenses"] });
      await queryClient.invalidateQueries({
        queryKey: ["expense", variables.id],
      });
      notify.success(
        "Expense updated",
        "Your changes were saved successfully.",
      );
    },
    onError: (error) => {
      notify.error("Could not update expense", error.message);
    },
  });
}
