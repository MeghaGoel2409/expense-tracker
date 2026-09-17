import { useMutation } from "@tanstack/react-query";
import { notify } from "@/components/ui/toast/notify";
import { getApiErrorMessage } from "@/lib/api/api-error";
import { expenseExportApi } from "../api/expenseExportApi";
import type { CreateExpenseExportRequest } from "../types/expenseExport.types";

export function useCreateExpenseExport() {
  return useMutation({
    mutationFn: async (request: CreateExpenseExportRequest) => {
      try {
        return await expenseExportApi.createExport(request);
      } catch (error) {
        throw new Error(
          await getApiErrorMessage(error, "Failed to request expense export."),
        );
      }
    },

    onSuccess: () => {
      notify.success(
        "Export requested",
        "Your expense export is being prepared.",
      );
    },

    onError: (error: Error) => {
      notify.error("Could not request export", error.message);
    },
  });
}
