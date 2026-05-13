export type CategoryExpenseSummaryDto = {
  categoryId: number;
  categoryName: string;
  totalAmount: number;
  percentage: number;
  transactionCount: number;
};

export type DashboardSummaryDto = {
  totalExpenses: number;
  expenseCount: number;
  essentialTotal: number;
  nonEssentialTotal: number;
  recurringTotal: number;
  topCategory: string | null;
  categoryBreakdown: CategoryExpenseSummaryDto[];
};

export type DashboardSummaryFilter = {
  fromDate?: string;
  toDate?: string;
};
