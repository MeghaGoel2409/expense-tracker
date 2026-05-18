import { useState } from "react";
import { Link } from "react-router-dom";
import { useExpenses } from "../hooks/useExpenses";
import { useDeleteExpense } from "../hooks/useDeleteExpense";
import { useExpenseFilters } from "../hooks/useExpenseFilters";
import { ExpenseFilters } from "../components/ExpenseFilters";
import { ExpensesList } from "../components/ExpensesList";
import { Pagination } from "@/components/ui/Pagination";
import { QueryState } from "@/components/ui/QueryState";
import { useCategories } from "@/features/categories/hooks/useCategories";
import { ConfirmDialog } from "@/components/ui/dialogs/ConfirmDialog";

export function ExpensesPage() {
  const [expenseToDelete, setExpenseToDelete] = useState<number | null>(null);

  const {
    appliedFilters,
    queryParams,
    applyFilters,
    clearFilters,
    changePage,
  } = useExpenseFilters();

  const expensesQuery = useExpenses(queryParams);
  const categoriesQuery = useCategories();
  const deleteExpenseMutation = useDeleteExpense();

  const expenses = expensesQuery.data?.items ?? [];

  const categories = Array.isArray(categoriesQuery.data)
    ? categoriesQuery.data
    : [];

  const handleDelete = (id: number) => {
    if (deleteExpenseMutation.isPending) {
      return;
    }

    setExpenseToDelete(id);
  };

  const confirmDelete = async () => {
    if (expenseToDelete === null) {
      return;
    }

    try {
      await deleteExpenseMutation.mutateAsync(expenseToDelete);
      setExpenseToDelete(null);
    } catch {
      // Error toast is handled inside useDeleteExpense.
    }
  };

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-gray-900">Expenses</h1>
          <p className="mt-1 text-sm text-gray-600">
            View and manage your expenses.
          </p>
        </div>

        <Link
          to="/expenses/new"
          className="rounded-lg bg-black px-4 py-2 text-sm font-medium text-white hover:bg-gray-800"
        >
          Add Expense
        </Link>
      </div>

      <ExpenseFilters
        key={JSON.stringify(appliedFilters)}
        values={appliedFilters}
        categories={categories}
        isLoadingCategories={categoriesQuery.isLoading}
        onSearch={applyFilters}
        onClear={clearFilters}
      />

      <QueryState
        isLoading={expensesQuery.isLoading}
        isError={expensesQuery.isError}
        isEmpty={expenses.length === 0}
        errorMessage={expensesQuery.error?.message}
        loadingMessage="Loading expenses..."
        emptyTitle="No expenses found"
        emptyDescription="No expenses match your current filters. Try clearing filters or add a new expense."
        emptyIcon="💳"
        onRetry={() => expensesQuery.refetch()}
        emptyAction={
          <Link
            to="/expenses/new"
            className="inline-flex rounded-lg bg-black px-4 py-2 text-sm font-medium text-white hover:bg-gray-800"
          >
            Add Expense
          </Link>
        }
      >
        <ExpensesList
          expenses={expenses}
          isDeleting={deleteExpenseMutation.isPending}
          onDelete={handleDelete}
        />

        {expensesQuery.data && (
          <Pagination
            pageNumber={expensesQuery.data.pageNumber}
            pageSize={expensesQuery.data.pageSize}
            totalCount={expensesQuery.data.totalCount}
            totalPages={expensesQuery.data.totalPages}
            onPageChange={changePage}
          />
        )}
      </QueryState>

      <ConfirmDialog
        open={expenseToDelete !== null}
        title="Delete expense"
        description="Are you sure you want to delete this expense? This action cannot be undone."
        confirmText="Delete"
        cancelText="Cancel"
        tone="danger"
        isLoading={deleteExpenseMutation.isPending}
        onCancel={() => setExpenseToDelete(null)}
        onConfirm={confirmDelete}
      />
    </div>
  );
}
