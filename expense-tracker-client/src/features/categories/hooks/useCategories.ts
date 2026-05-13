import { useQuery } from "@tanstack/react-query";
import { categoryApi } from "../api/categoryApi";
import { getApiErrorMessage } from "@/lib/api/api-error";

export function useCategories() {
  return useQuery({
    queryKey: ["categories"],
    queryFn: async () => {
      try {
        return await categoryApi.getCategories();
      } catch (error) {
        throw new Error(getApiErrorMessage(error, "Failed to load expenses."));
      }
    },
  });
}
