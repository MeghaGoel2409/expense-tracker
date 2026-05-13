import { useState } from "react";
import { CategoryBreakdownCard } from "../components/CategoryBreakdownCard";
import { DashboardSummaryCards } from "../components/DashboardSummaryCards";
import { SpendingSplitCard } from "../components/SpendingSplitCard";
import { useDashboardSummary } from "../hooks/useDashboardSummary";
import {
  getDashboardDateRange,
  type DashboardRange,
} from "../utils/dashboardDateRanges";

export function DashboardPage() {
  const [range, setRange] = useState<DashboardRange>("thisMonth");

  const filters = getDashboardDateRange(range);
  const { data, isLoading, isError } = useDashboardSummary(filters);

  if (isError) {
    return (
      <div className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700">
        Unable to load dashboard. Please try again.
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <section className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-gray-900">Dashboard</h1>
          <p className="text-sm text-gray-500">
            Track your spending for the selected period.
          </p>
        </div>

        <select
          value={range}
          onChange={(e) => setRange(e.target.value as DashboardRange)}
          className="w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm shadow-sm sm:w-48"
        >
          <option value="thisMonth">This Month</option>
          <option value="lastMonth">Last Month</option>
          <option value="last3Months">Last 3 Months</option>
        </select>
      </section>

      <DashboardSummaryCards data={data} isLoading={isLoading} />

      <div className="grid gap-6 lg:grid-cols-2">
        <SpendingSplitCard data={data} isLoading={isLoading} />
        <CategoryBreakdownCard data={data} isLoading={isLoading} />
      </div>
    </div>
  );
}
