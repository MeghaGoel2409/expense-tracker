import { useQuery } from "@tanstack/react-query";
import { getApiErrorMessage } from "@/lib/api/api-error";
import { featureSettingsApi } from "../api/featureSettingsApi";

export function useFeatureSettings(enabled: boolean) {
  return useQuery({
    queryKey: ["feature-settings"],

    queryFn: async () => {
      try {
        return await featureSettingsApi.getFeatureSettings();
      } catch (error) {
        throw new Error(
          await getApiErrorMessage(error, "Failed to load feature settings."),
        );
      }
    },

    enabled,
    staleTime: 5 * 60 * 1000,
    gcTime: 30 * 60 * 1000,
    refetchOnWindowFocus: false,
    retry: 1,
  });
}
