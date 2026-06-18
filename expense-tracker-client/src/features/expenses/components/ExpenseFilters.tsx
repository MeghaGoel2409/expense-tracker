import {
  FilterActionButton,
  FilterDate,
  FilterPanel,
  FilterSelect,
} from "@/components/ui/filters";
import { FilterChipGroup } from "@/components/ui/filters/FilterChipGroup";
import type { Category } from "@/features/categories/types/category.types";
import { Search, X } from "lucide-react";
import { useState } from "react";
import { expensePeriodOptions } from "../constants/expensePeriods";
import type { ExpensePeriod } from "../types/expense.types";

export type ExpenseFilterValues = {
  period: ExpensePeriod;
  fromDate: string;
  toDate: string;
  categoryId: string;
};

type ExpenseFiltersProps = {
  values: ExpenseFilterValues;
  categories: Category[];
  isLoadingCategories?: boolean;
  onSearch: (values: ExpenseFilterValues) => void;
  onClear: () => void;
};

export function ExpenseFilters({
  values,
  categories,
  isLoadingCategories = false,
  onSearch,
  onClear,
}: ExpenseFiltersProps) {
  const [draftValues, setDraftValues] = useState<ExpenseFilterValues>(values);

  const isCustomPeriod = draftValues.period === "custom";

  const updateDraftValue = (name: keyof ExpenseFilterValues, value: string) => {
    setDraftValues((current) => ({
      ...current,
      [name]: value,
    }));
  };

  const applyFilters = (nextValues: ExpenseFilterValues) => {
    setDraftValues(nextValues);
    onSearch(nextValues);
  };

  const handlePeriodChange = (period: ExpensePeriod) => {
    const nextValues: ExpenseFilterValues = {
      ...draftValues,
      period,
      fromDate: period === "custom" ? draftValues.fromDate : "",
      toDate: period === "custom" ? draftValues.toDate : "",
    };

    if (period === "custom") {
      setDraftValues(nextValues);
      return;
    }

    applyFilters(nextValues);
  };

  const handleCategoryChange = (categoryId: string) => {
    const nextValues: ExpenseFilterValues = {
      ...draftValues,
      categoryId,
    };

    if (isCustomPeriod) {
      setDraftValues(nextValues);
      return;
    }

    applyFilters(nextValues);
  };

  return (
    <FilterPanel
      onSearch={() => onSearch(draftValues)}
      actions={
        <>
          {isCustomPeriod && (
            <FilterActionButton type="submit" tone="primary" icon={Search}>
              Search
            </FilterActionButton>
          )}

          <FilterActionButton onClick={onClear} icon={X}>
            Clear
          </FilterActionButton>
        </>
      }
    >
      <div className="flex flex-col gap-3 lg:flex-row lg:items-end">
        <div className="flex flex-wrap gap-2">
          <FilterChipGroup
            value={draftValues.period}
            options={expensePeriodOptions}
            onChange={handlePeriodChange}
          />
        </div>

        <div className="w-full lg:w-56">
          <FilterSelect
            value={draftValues.categoryId}
            disabled={isLoadingCategories}
            placeholder="All Categories"
            options={categories.map((category) => ({
              label: category.name,
              value: String(category.id),
            }))}
            onChange={handleCategoryChange}
          />
        </div>
      </div>

      {isCustomPeriod && (
        <div className="mt-3 grid gap-3 md:grid-cols-2 lg:max-w-md">
          <FilterDate
            label="From Date"
            value={draftValues.fromDate}
            onChange={(value) => updateDraftValue("fromDate", value)}
          />

          <FilterDate
            label="To Date"
            value={draftValues.toDate}
            onChange={(value) => updateDraftValue("toDate", value)}
          />
        </div>
      )}
    </FilterPanel>
  );
}
