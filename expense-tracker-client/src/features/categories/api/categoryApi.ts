import { apiClient } from "@/lib/api/axios";
import { endpoints } from "@/lib/api/endpoints";
import type { Category } from "../types/category.types";
import { unwrapApiResult, type ApiResult } from "@/lib/api/apiResult";

export const categoryApi = {
  async getCategories(): Promise<Category[]> {
    const response = await apiClient.get<ApiResult<Category[]>>(
      endpoints.categories.base,
    );

    return unwrapApiResult(response.data, "Failed to load categories");
  },
};
