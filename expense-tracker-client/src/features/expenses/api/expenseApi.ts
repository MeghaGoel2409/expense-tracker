import { apiClient } from "@/lib/api/axios";
import { endpoints } from "@/lib/api/endpoints";
import { unwrapApiResult, type ApiResult } from "@/lib/api/apiResult";
import type {
  CreateExpenseRequest,
  Expense,
  ExpenseQueryParams,
  PagedResult,
  UpdateExpenseRequest,
} from "../types/expense.types";

export const expenseApi = {
  async getExpenses(
    params?: ExpenseQueryParams,
  ): Promise<PagedResult<Expense>> {
    const response = await apiClient.get<ApiResult<PagedResult<Expense>>>(
      endpoints.expenses.base,
      {
        params: {
          CategoryId: params?.categoryId,
          FromDate: params?.fromDate,
          ToDate: params?.toDate,
          SortBy: params?.sortBy,
          SortDescending: params?.sortDescending,
          PageNumber: params?.pageNumber,
          PageSize: params?.pageSize,
        },
      },
    );

    return unwrapApiResult(response.data, "Failed to load expenses.");
  },

  async getExpenseById(id: number): Promise<Expense> {
    const response = await apiClient.get<ApiResult<Expense>>(
      endpoints.expenses.byId(id),
    );
    return unwrapApiResult(response.data, "Failed to load expense.");
  },

  async createExpense(request: CreateExpenseRequest): Promise<Expense> {
    const response = await apiClient.post(endpoints.expenses.base, request);
    return unwrapApiResult(response.data, "Failed to create expense.");
  },

  async updateExpense(
    id: number,
    request: UpdateExpenseRequest,
  ): Promise<Expense> {
    const response = await apiClient.put(endpoints.expenses.byId(id), request);
    return unwrapApiResult(response.data, "Failed to update expense.");
  },

  async deleteExpense(id: number) {
    const response = await apiClient.delete(endpoints.expenses.byId(id));
    return unwrapApiResult(response.data, "Failed to delete expense.");
  },
};
