// src/features/expenses/components/ExpenseForm.tsx

import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  expenseSchema,
  type ExpenseFormInput,
  type ExpenseFormValues,
} from "../schemas/expenseSchemas";
import { useCategories } from "@/features/categories/hooks/useCategories";

type ExpenseFormProps = {
  defaultValues: ExpenseFormInput;
  onSubmit: (values: ExpenseFormValues) => void | Promise<void>;
  submitText: string;
  isSubmitting?: boolean;
  serverError?: string;
  onCancel: () => void;
};

export function ExpenseForm({
  defaultValues,
  onSubmit,
  submitText,
  isSubmitting = false,
  serverError,
  onCancel,
}: ExpenseFormProps) {
  const categoriesQuery = useCategories();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ExpenseFormInput, unknown, ExpenseFormValues>({
    resolver: zodResolver(expenseSchema),
    defaultValues,
  });

  useEffect(() => {
    reset(defaultValues);
  }, [defaultValues, reset]);

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      noValidate
      className="space-y-5 rounded-2xl border bg-white p-6 shadow-sm"
    >
      <div>
        <label className="mb-1 block text-sm font-medium">Amount</label>
        <input
          type="number"
          step="0.01"
          {...register("amount")}
          className="w-full rounded-lg border border-gray-300 px-3 py-2"
        />
        {errors.amount && (
          <p className="mt-1 text-sm text-red-600">{errors.amount.message}</p>
        )}
      </div>

      <div>
        <label className="mb-1 block text-sm font-medium">Date</label>
        <input
          type="date"
          {...register("expenseDate")}
          className="w-full rounded-lg border border-gray-300 px-3 py-2"
        />
        {errors.expenseDate && (
          <p className="mt-1 text-sm text-red-600">
            {errors.expenseDate.message}
          </p>
        )}
      </div>

      <div>
        <label className="mb-1 block text-sm font-medium">Category</label>
        <select
          {...register("categoryId")}
          disabled={categoriesQuery.isLoading}
          className="w-full rounded-lg border border-gray-300 px-3 py-2"
        >
          <option value={0}>Select category</option>
          {categoriesQuery.data?.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </select>
        {errors.categoryId && (
          <p className="mt-1 text-sm text-red-600">
            {errors.categoryId.message}
          </p>
        )}
      </div>

      {/* <div>
        <label className="mb-1 block text-sm font-medium">Currency</label>
        <input
          type="text"
          {...register("currency")}
          className="w-full rounded-lg border border-gray-300 px-3 py-2"
        />
      </div> */}

      <div>
        <label className="mb-1 block text-sm font-medium">Merchant</label>
        <input
          type="text"
          {...register("merchant")}
          className="w-full rounded-lg border border-gray-300 px-3 py-2"
          placeholder="Target, Amazon, Costco..."
        />
      </div>

      <div>
        <label className="mb-1 block text-sm font-medium">Payment Method</label>
        <input
          type="text"
          {...register("paymentMethod")}
          className="w-full rounded-lg border border-gray-300 px-3 py-2"
          placeholder="Credit Card, Cash, Debit..."
        />
      </div>

      <div>
        <label className="mb-1 block text-sm font-medium">Expense Type</label>
        <select
          {...register("expenseType")}
          className="w-full rounded-lg border border-gray-300 px-3 py-2"
        >
          <option value="Essential">Essential</option>
          <option value="NonEssential">Non-Essential</option>
        </select>
        {errors.expenseType && (
          <p className="mt-1 text-sm text-red-600">
            {errors.expenseType.message}
          </p>
        )}
      </div>

      <div>
        <label className="mb-1 block text-sm font-medium">Notes</label>
        <textarea
          {...register("notes")}
          className="w-full rounded-lg border border-gray-300 px-3 py-2"
          rows={3}
        />
      </div>

      <label className="flex items-center gap-2 text-sm">
        <input type="checkbox" {...register("isRecurring")} />
        Recurring expense
      </label>

      {serverError && (
        <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
          {serverError}
        </div>
      )}

      <div className="flex justify-end gap-3">
        <button
          type="button"
          onClick={onCancel}
          className="rounded-lg border px-4 py-2 text-sm"
        >
          Cancel
        </button>

        <button
          type="submit"
          disabled={isSubmitting}
          className="rounded-lg bg-black px-4 py-2 text-sm font-medium text-white disabled:opacity-60"
        >
          {isSubmitting ? "Saving..." : submitText}
        </button>
      </div>
    </form>
  );
}
