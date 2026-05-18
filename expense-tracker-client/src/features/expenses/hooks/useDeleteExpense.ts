import { useMutation, useQueryClient } from "@tanstack/react-query";
import { expenseApi } from "../api/expenseApi";
import { getApiErrorMessage } from "@/lib/api/api-error";
import { notify } from "@/components/ui/toast/notify";

export function useDeleteExpense() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: number) => {
      try {
        return await expenseApi.deleteExpense(id);
      } catch (error) {
        throw new Error(getApiErrorMessage(error, "Failed to delete expense."));
      }
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["expenses"] });
      notify.success("Expense deleted", "Expense was removed successfully.");
    },
    onError: (error) => {
      notify.error("Could not delete expense", error.message);
    },
  });
}
