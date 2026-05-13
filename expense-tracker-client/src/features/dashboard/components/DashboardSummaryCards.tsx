import type { DashboardSummaryDto } from "../types/dashboard.types";
import { formatCurrency } from "@/common/utils/formatCurrency";

type Props = {
  data?: DashboardSummaryDto;
  isLoading: boolean;
};

export function DashboardSummaryCards({ data, isLoading }: Props) {
  const cards = [
    {
      title: "Total Spent",
      value: formatCurrency(data?.totalExpenses ?? 0),
    },
    {
      title: "Transactions",
      value: data?.expenseCount?.toString() ?? "0",
    },
    {
      title: "Recurring Total",
      value: formatCurrency(data?.recurringTotal ?? 0),
    },
  ];

  return (
    <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
      {cards.map((card) => (
        <div
          key={card.title}
          className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm"
        >
          <p className="text-sm text-gray-500">{card.title}</p>

          {isLoading ? (
            <div className="mt-3 h-7 w-24 animate-pulse rounded bg-gray-200" />
          ) : (
            <p className="mt-2 text-2xl font-semibold text-gray-900">
              {card.value}
            </p>
          )}
        </div>
      ))}
    </div>
  );
}
