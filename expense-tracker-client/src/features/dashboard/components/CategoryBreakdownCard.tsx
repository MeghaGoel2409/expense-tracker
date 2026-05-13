import type { DashboardSummaryDto } from "../types/dashboard.types";
import { formatCurrency } from "@/common/utils/formatCurrency";

type Props = {
  data?: DashboardSummaryDto;
  isLoading: boolean;
};

export function CategoryBreakdownCard({ data, isLoading }: Props) {
  const categories = data?.categoryBreakdown ?? [];

  return (
    <div className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
      <div className="flex items-start justify-between gap-4">
        <div>
          <h2 className="text-base font-semibold text-gray-900">
            Category Breakdown
          </h2>
          <p className="mt-1 text-sm text-gray-500">
            Where your money went in this period.
          </p>
        </div>

        {!isLoading && data?.topCategory && (
          <span className="rounded-full bg-gray-100 px-3 py-1 text-xs font-medium text-gray-700">
            Top: {data.topCategory}
          </span>
        )}
      </div>

      {isLoading ? (
        <div className="mt-5 space-y-3">
          <div className="h-5 animate-pulse rounded bg-gray-200" />
          <div className="h-5 animate-pulse rounded bg-gray-200" />
          <div className="h-5 animate-pulse rounded bg-gray-200" />
        </div>
      ) : categories.length === 0 ? (
        <div className="mt-6 rounded-xl border border-dashed border-gray-300 p-6 text-center">
          <p className="text-sm font-medium text-gray-700">
            No expenses yet for this period
          </p>
          <p className="mt-1 text-sm text-gray-500">
            Add your first expense to see category insights.
          </p>
        </div>
      ) : (
        <div className="mt-5 space-y-4">
          {categories.map((category) => (
            <div key={category.categoryId}>
              <div className="mb-1 flex justify-between text-sm">
                <span className="font-medium text-gray-700">
                  {category.categoryName}
                </span>
                <span className="text-gray-500">
                  {formatCurrency(category.totalAmount)} · {category.percentage}
                  %
                </span>
              </div>

              <div className="h-2 rounded-full bg-gray-100">
                <div
                  className="h-2 rounded-full bg-gray-900"
                  style={{
                    width: `${Math.min(category.percentage, 100)}%`,
                  }}
                />
              </div>

              <p className="mt-1 text-xs text-gray-400">
                {category.transactionCount} transactions
              </p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
