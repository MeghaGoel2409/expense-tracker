import { useState, useEffect, useRef } from "react";
import { Link } from "react-router-dom";
import { useExpenses } from "../hooks/useExpenses";
import { useDeleteExpense } from "../hooks/useDeleteExpense";
import { useExpenseFilters } from "../hooks/useExpenseFilters";
import { ExpenseFilters } from "../components/ExpenseFilters";
import { ExpensesList } from "../components/ExpensesList";
import { getExpensePeriodLabel } from "../constants/expensePeriods";
import { Pagination } from "@/components/ui/Pagination";
import { QueryState } from "@/components/ui/QueryState";
import { useCategories } from "@/features/categories/hooks/useCategories";
import { ConfirmDialog } from "@/components/ui/dialogs/ConfirmDialog";
import { Download } from "lucide-react";
import { useAuth } from "@/features/auth/hooks/useAuth";
import { FeatureKeys } from "@/features/feature-settings/types/featureSettings.types";
import { useCreateExpenseExport } from "@/features/expense-exports/hooks/useCreateExpenseExport";
import { useExpenseExportStatus } from "@/features/expense-exports/hooks/useExpenseExportStatus";

import { useDownloadExpenseExport } from "@/features/expense-exports/hooks/useDownloadExpenseExport";
import { notify } from "@/components/ui/toast/notify";

export function ExpensesPage() {
  const [expenseToDelete, setExpenseToDelete] = useState<number | null>(null);
  const [activeExportJobId, setActiveExportJobId] = useState<number | null>(
    null,
  );
  const handledExportJobIdRef = useRef<number | null>(null);

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
  const createExportMutation = useCreateExpenseExport();

  const exportStatusQuery = useExpenseExportStatus(activeExportJobId);
  const downloadExportMutation = useDownloadExpenseExport();

  useEffect(() => {
    if (
      activeExportJobId === null ||
      handledExportJobIdRef.current === activeExportJobId
    ) {
      return;
    }

    if (exportStatusQuery.isError) {
      handledExportJobIdRef.current = activeExportJobId;

      notify.error(
        "Could not check export status",
        exportStatusQuery.error?.message ||
          "The export status could not be retrieved.",
      );

      queueMicrotask(() => {
        setActiveExportJobId(null);
      });

      return;
    }

    if (!exportStatusQuery.data) {
      return;
    }

    const { status, errorMessage } = exportStatusQuery.data;

    if (status === "Completed") {
      handledExportJobIdRef.current = activeExportJobId;

      downloadExportMutation.mutate(activeExportJobId, {
        onSettled: () => {
          setActiveExportJobId(null);
        },
      });

      return;
    }

    if (status === "Failed") {
      handledExportJobIdRef.current = activeExportJobId;

      notify.error(
        "Export failed",
        errorMessage || "The expense export could not be completed.",
      );

      queueMicrotask(() => {
        setActiveExportJobId(null);
      });
    }
  }, [
    activeExportJobId,
    exportStatusQuery.data,
    exportStatusQuery.error,
    exportStatusQuery.isError,
    downloadExportMutation,
  ]);

  const { hasFeature } = useAuth();

  const isExportEnabled = hasFeature(FeatureKeys.expenseExport);
  const isExportInProgress =
    activeExportJobId !== null ||
    createExportMutation.isPending ||
    downloadExportMutation.isPending;

  const expenses = expensesQuery.data?.items ?? [];

  const categories = Array.isArray(categoriesQuery.data)
    ? categoriesQuery.data
    : [];

  const totalCount = expensesQuery.data?.totalCount ?? 0;

  const selectedCategory = categories.find(
    (category) => String(category.id) === appliedFilters.categoryId,
  );

  const selectedPeriod =
    appliedFilters.period === "custom"
      ? `${appliedFilters.fromDate || "Start"} to 
            ${appliedFilters.toDate || "End"}`
      : getExpensePeriodLabel(appliedFilters.period);

  const emptyDescription =
    `No transactions found for ${selectedPeriod}` +
    (selectedCategory ? `and ${selectedCategory.name}` : "");

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

  const handleExport = async () => {
    handledExportJobIdRef.current = null;
    try {
      const result = await createExportMutation.mutateAsync({
        format: "Csv",
        filter: {
          categoryId: appliedFilters.categoryId
            ? Number(appliedFilters.categoryId)
            : undefined,
          fromDate: appliedFilters.fromDate || undefined,
          toDate: appliedFilters.toDate || undefined,
        },
      });

      setActiveExportJobId(result.exportJobId);
    } catch {
      // Error toast is handled inside useCreateExpenseExport.
    }
  };

  return (
    <div>
      <div className="mb-4 flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-gray-900">Transactions</h1>
          <p className="mt-1 text-sm text-gray-600">
            {totalCount.toLocaleString()} transaction
            {totalCount === 1 ? "" : "s"} found
          </p>
        </div>

        <div className="flex items-center gap-2">
          {isExportEnabled && (
            <button
              type="button"
              onClick={handleExport}
              disabled={isExportInProgress || totalCount === 0}
              className="inline-flex items-center gap-2 rounded-lg border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-60"
            >
              <Download className="h-4 w-4" />

              {createExportMutation.isPending
                ? "Requesting..."
                : downloadExportMutation.isPending
                  ? "Downloading..."
                  : activeExportJobId !== null
                    ? "Preparing..."
                    : "Export CSV"}
            </button>
          )}

          <Link
            to="/expenses/new"
            className="rounded-lg bg-black px-4 py-2 text-sm font-medium text-white hover:bg-gray-800"
          >
            Add Expense
          </Link>
        </div>
      </div>

      <ExpenseFilters
        key={JSON.stringify(appliedFilters)}
        values={appliedFilters}
        categories={categories}
        isLoadingCategories={categoriesQuery.isLoading}
        onSearch={applyFilters}
        onClear={clearFilters}
      />

      <div className="mb-4 flex flex-wrap items-center gap-2 text-sm text-gray-600">
        <span>Showing</span>

        <span className="rounded-full bg-gray-100 px-2.5 py-1 text-xs font-medium text-gray-700">
          {selectedPeriod}
        </span>

        <span className="rounded-full bg-gray-100 px-2.5 py-1 text-xs font-medium text-gray-700">
          {selectedCategory?.name ?? "All categories"}
        </span>
      </div>

      <QueryState
        isLoading={expensesQuery.isLoading}
        isError={expensesQuery.isError}
        isEmpty={expenses.length === 0}
        errorMessage={expensesQuery.error?.message}
        loadingMessage="Loading expenses..."
        emptyTitle="No transactions found"
        emptyDescription={emptyDescription}
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
