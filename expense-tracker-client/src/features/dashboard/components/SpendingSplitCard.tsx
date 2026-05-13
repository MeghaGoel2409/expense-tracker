import type { DashboardSummaryDto } from "../types/dashboard.types";
import { formatCurrency } from "@/lib/utils/formatCurrency";

type Props = {
  data?: DashboardSummaryDto;
  isLoading: boolean;
};

export function SpendingSplitCard({ data, isLoading }: Props) {
  return (
    <div className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm">
      <h2 className="text-base font-semibold text-gray-900">Spending Split</h2>
      <p className="mt-1 text-sm text-gray-500">
        Essential vs non-essential spending.
      </p>

      {isLoading ? (
        <div className="mt-5 space-y-3">
          <div className="h-5 animate-pulse rounded bg-gray-200" />
          <div className="h-5 animate-pulse rounded bg-gray-200" />
        </div>
      ) : (
        <div className="mt-5 space-y-4">
          <SplitRow label="Essential" value={data?.essentialTotal ?? 0} />
          <SplitRow
            label="Non-Essential"
            value={data?.nonEssentialTotal ?? 0}
          />
        </div>
      )}
    </div>
  );
}

function SplitRow({ label, value }: { label: string; value: number }) {
  return (
    <div className="flex items-center justify-between rounded-xl bg-gray-50 px-4 py-3">
      <span className="text-sm font-medium text-gray-700">{label}</span>
      <span className="text-sm font-semibold text-gray-900">
        {formatCurrency(value)}
      </span>
    </div>
  );
}
