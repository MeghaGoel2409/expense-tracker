import { useQuery } from "@tanstack/react-query";
import { getDashboardSummary } from "../api/dashboardApi";
import type { DashboardSummaryFilter } from "../types/dashboard.types";

export function useDashboardSummary(filters: DashboardSummaryFilter) {
  return useQuery({
    queryKey: ["dashboard-summary", filters],
    queryFn: () => getDashboardSummary(filters),
  });
}
