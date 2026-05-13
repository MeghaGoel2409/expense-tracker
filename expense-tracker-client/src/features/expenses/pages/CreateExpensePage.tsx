// src/features/expenses/pages/CreateExpensePage.tsx

import { useNavigate } from "react-router-dom";
import type {
  ExpenseFormInput,
  ExpenseFormValues,
} from "../schemas/expenseSchemas";
import { ExpenseForm } from "../components/ExpenseForm";
import { mapExpenseToCreateRequest } from "../mappers/expenseMappers";
import { useCreateExpense } from "../hooks/useCreateExpense";

const defaultValues: ExpenseFormInput = {
  amount: 0,
  expenseDate: new Date().toISOString().slice(0, 10),
  categoryId: 0,
  currency: "USD",
  notes: "",
  merchant: "",
  paymentMethod: "",
  expenseType: "Essential",
  isRecurring: false,
};

export function CreateExpensePage() {
  const navigate = useNavigate();
  const createExpenseMutation = useCreateExpense();

  const onSubmit = async (values: ExpenseFormValues) => {
    await createExpenseMutation.mutateAsync(mapExpenseToCreateRequest(values));
    navigate("/expenses");
  };

  return (
    <div className="mx-auto max-w-2xl">
      <div className="mb-6">
        <h1 className="text-2xl font-semibold text-gray-900">Add Expense</h1>
        <p className="mt-1 text-sm text-gray-600">
          Track a new expense in your account.
        </p>
      </div>

      <ExpenseForm
        defaultValues={defaultValues}
        onSubmit={onSubmit}
        submitText="Save Expense"
        isSubmitting={createExpenseMutation.isPending}
        serverError={
          createExpenseMutation.isError
            ? createExpenseMutation.error.message
            : undefined
        }
        onCancel={() => navigate("/expenses")}
      />
    </div>
  );
}
