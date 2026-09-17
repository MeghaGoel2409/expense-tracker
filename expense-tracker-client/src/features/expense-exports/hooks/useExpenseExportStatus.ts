import { useQuery } from "@tanstack/react-query";
import { getApiErrorMessage } from "@/lib/api/api-error";
import { expenseExportApi } from "../api/expenseExportApi";

export function useExpenseExportStatus(exportJobId: number | null) {
  return useQuery({
    queryKey: ["expense-export", exportJobId],

    queryFn: async () => {
      if (exportJobId === null) {
        throw new Error("Export job ID is required.");
      }

      try {
        return await expenseExportApi.getExportStatus(exportJobId);
      } catch (error) {
        throw new Error(
          await getApiErrorMessage(error, "Failed to retrieve export status."),
        );
      }
    },

    enabled: exportJobId !== null,

    refetchInterval: (query) => {
      const status = query.state.data?.status;

      return status === "Completed" || status === "Failed" ? false : 2_000;
    },

    refetchIntervalInBackground: false,
    retry: 2,
  });
}
