// src/features/expenses/pages/EditExpensePage.tsx

import { useNavigate, useParams } from "react-router-dom";
import { ExpenseForm } from "../components/ExpenseForm";
import { useExpense } from "../hooks/useExpense";
import { useUpdateExpense } from "../hooks/useUpdateExpense";
import type {
  ExpenseFormInput,
  ExpenseFormValues,
} from "../schemas/expenseSchemas";

export function EditExpensePage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const expenseId = Number(id);

  const expenseQuery = useExpense(expenseId);
  const updateExpenseMutation = useUpdateExpense();

  if (!Number.isFinite(expenseId) || expenseId <= 0) {
    return <p className="text-red-600">Invalid expense id.</p>;
  }

  if (expenseQuery.isLoading) {
    return <p className="text-sm text-gray-600">Loading expense...</p>;
  }

  if (expenseQuery.isError) {
    return (
      <p className="text-sm text-red-600">
        {expenseQuery.error?.message || "failed to load expense"}
      </p>
    );
  }

  if (!expenseQuery.data) {
    return <p className="text-sm text-red-600">Expense not found.</p>;
  }

  const expense = expenseQuery.data;

  console.log(expense.expenseType);

  const defaultValues: ExpenseFormInput = {
    amount: expense.amount,
    expenseDate: expense.expenseDate.slice(0, 10),
    categoryId: expense.categoryId,
    currency: expense.currency ?? "USD",
    notes: expense.notes ?? "",
    merchant: expense.merchant ?? "",
    paymentMethod: expense.paymentMethod ?? "",
    expenseType: expense.expenseType ?? "Essential",
    isRecurring: expense.isRecurring ?? false,
  };

  const onSubmit = async (values: ExpenseFormValues) => {
    await updateExpenseMutation.mutateAsync({
      id: expenseId,
      ...values,
    });

    navigate("/expenses");
  };

  return (
    <div className="mx-auto max-w-2xl">
      <div className="mb-6">
        <h1 className="text-2xl font-semibold text-gray-900">Edit Expense</h1>
        <p className="mt-1 text-sm text-gray-600">
          Update your expense details.
        </p>
      </div>

      <ExpenseForm
        defaultValues={defaultValues}
        onSubmit={onSubmit}
        submitText="Update Expense"
        isSubmitting={updateExpenseMutation.isPending}
        serverError={
          updateExpenseMutation.isError
            ? updateExpenseMutation.error.message
            : undefined
        }
        onCancel={() => navigate("/expenses")}
      />
    </div>
  );
}
