import { apiClient } from "@/lib/api/axios";
import { endpoints } from "@/lib/api/endpoints";
import type { FeatureSettings } from "../types/featureSettings.types";
import { unwrapApiResult, type ApiResult } from "@/lib/api/apiResult";

export const featureSettingsApi = {
  async getFeatureSettings(): Promise<FeatureSettings> {
    const response = await apiClient.get<ApiResult<FeatureSettings>>(
      endpoints.featureSettings.base,
    );
    return unwrapApiResult(response.data, "Failed to load feature settings.");
  },
};
