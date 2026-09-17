import { useMutation } from "@tanstack/react-query";
import { notify } from "@/components/ui/toast/notify";
import { getApiErrorMessage } from "@/lib/api/api-error";
import { expenseExportApi } from "../api/expenseExportApi";

export function useDownloadExpenseExport() {
  return useMutation({
    mutationFn: async (exportJobId: number) => {
      try {
        return await expenseExportApi.downloadExport(exportJobId);
      } catch (error) {
        throw new Error(
          await getApiErrorMessage(error, "Failed to download expense export."),
        );
      }
    },

    onSuccess: (blob) => {
      const url = URL.createObjectURL(blob);
      const anchor = document.createElement("a");

      anchor.href = url;
      anchor.download = `expenses-${new Date()
        .toISOString()
        .replace(/[:.]/g, "-")}.csv`;

      document.body.appendChild(anchor);
      anchor.click();
      anchor.remove();

      URL.revokeObjectURL(url);

      notify.success(
        "Export downloaded",
        "Your expense export was downloaded successfully.",
      );
    },

    onError: (error: Error) => {
      notify.error("Could not download export", error.message);
    },
  });
}
