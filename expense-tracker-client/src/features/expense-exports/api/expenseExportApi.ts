import { unwrapApiResult, type ApiResult } from "@/lib/api/apiResult";
import { apiClient } from "@/lib/api/axios";
import { endpoints } from "@/lib/api/endpoints";
import type {
  CreateExpenseExportRequest,
  CreateExpenseExportResponse,
  ExpenseExportStatusResponse,
} from "../types/expenseExport.types";

export const expenseExportApi = {
  async createExport(
    request: CreateExpenseExportRequest,
  ): Promise<CreateExpenseExportResponse> {
    const response = await apiClient.post<
      ApiResult<CreateExpenseExportResponse>
    >(endpoints.expenseExports.base, request);

    return unwrapApiResult(response.data, "Failed to request expense export.");
  },

  async getExportStatus(
    exportJobId: number,
  ): Promise<ExpenseExportStatusResponse> {
    const response = await apiClient.get<
      ApiResult<ExpenseExportStatusResponse>
    >(endpoints.expenseExports.byId(exportJobId));

    return unwrapApiResult(response.data, "Failed to retrieve export status.");
  },

  async downloadExport(exportJobId: number): Promise<Blob> {
    const response = await apiClient.get<Blob>(
      endpoints.expenseExports.download(exportJobId),
      {
        responseType: "blob",
      },
    );

    return response.data;
  },
};
