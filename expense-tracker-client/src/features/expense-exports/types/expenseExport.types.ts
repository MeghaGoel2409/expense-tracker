export type ExpenseExportFormat = "Csv";
export type ExpenseExportStatus =
  | "Pending"
  | "Processing"
  | "Completed"
  | "Failed";

export interface ExpenseExportFilter {
  fromDate?: string;
  toDate?: string;
  categoryId?: number;
}

export interface CreateExpenseExportRequest {
  format: ExpenseExportFormat;
  filter: ExpenseExportFilter;
}

export interface CreateExpenseExportResponse {
  exportJobId: number;
}

export interface ExpenseExportStatusResponse {
  exportJobId: number;
  status: ExpenseExportStatus;
  errorMessage?: string | null;
  createdOnUtc: string;
  startedOnUtc?: string | null;
  completedOnUtc?: string | null;
}
