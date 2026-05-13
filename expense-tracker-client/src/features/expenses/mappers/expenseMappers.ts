import type { ExpenseFormValues } from "../schemas/expenseSchemas";
import type {
  CreateExpenseRequest,
  UpdateExpenseRequest,
} from "../types/expense.types";

function emptyToNull(value?: string): string | null {
  const trimmed = value?.trim();

  return trimmed ? trimmed : null;
}

export function mapExpenseToCreateRequest(
  values: ExpenseFormValues,
): CreateExpenseRequest {
  return {
    amount: values.amount,
    expenseDate: values.expenseDate,
    categoryId: values.categoryId,
    //currency: emptyToNull(values.currency),
    notes: emptyToNull(values.notes),
    merchant: emptyToNull(values.merchant),
    paymentMethod: emptyToNull(values.paymentMethod),
    expenseType: values.expenseType,
    isRecurring: values.isRecurring,
  };
}

export function mapExpenseToUpdateRequest(
  id: number,
  values: ExpenseFormValues,
): UpdateExpenseRequest {
  return {
    id,
    amount: values.amount,
    expenseDate: values.expenseDate,
    categoryId: values.categoryId,
    //currency: emptyToNull(values.currency),
    notes: emptyToNull(values.notes),
    merchant: emptyToNull(values.merchant),
    paymentMethod: emptyToNull(values.paymentMethod),
    expenseType: values.expenseType,
    isRecurring: values.isRecurring,
  };
}
