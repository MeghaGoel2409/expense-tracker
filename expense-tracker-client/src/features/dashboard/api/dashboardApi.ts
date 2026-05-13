import { apiClient } from "@/lib/api/axios";
import { unwrapApiResult } from "@/lib/api/apiResult";
import type {
  DashboardSummaryDto,
  DashboardSummaryFilter,
} from "../types/dashboard.types";

export async function getDashboardSummary(
  filters: DashboardSummaryFilter,
): Promise<DashboardSummaryDto> {
  const response = await apiClient.get("/Dashboard/summary", {
    params: filters,
  });

  return unwrapApiResult<DashboardSummaryDto>(
    response.data,
    "Could not load data",
  );
}
