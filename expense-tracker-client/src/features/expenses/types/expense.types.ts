export type ExpenseType = "Essential" | "NonEssential";

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface Expense {
  id: number;
  amount: number;
  expenseDate: string;
  categoryId: number;
  categoryName?: string | null;
  currency: string;
  notes?: string | null;
  merchant?: string | null;
  paymentMethod?: string | null;
  expenseType: ExpenseType;
  isRecurring: boolean;
}

export interface CreateExpenseRequest {
  amount: number;
  expenseDate: string;
  categoryId: number;
  //currency?: string | null;
  notes?: string | null;
  merchant?: string | null;
  paymentMethod?: string | null;
  expenseType: ExpenseType;
  isRecurring: boolean;
}

export interface UpdateExpenseRequest {
  id: number;
  amount: number;
  expenseDate: string;
  categoryId: number;
  //currency?: string | null;
  notes?: string | null;
  merchant?: string | null;
  paymentMethod?: string | null;
  expenseType: ExpenseType;
  isRecurring: boolean;
}

export interface ExpenseQueryParams {
  categoryId?: number;
  fromDate?: string;
  toDate?: string;
  sortBy?: string;
  sortDescending?: boolean;
  pageNumber?: number;
  pageSize?: number;
}
