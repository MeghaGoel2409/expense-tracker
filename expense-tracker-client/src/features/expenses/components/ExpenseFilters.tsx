import { FilterDate, FilterPanel, FilterSelect } from "@/components/ui/filters";
import type { Category } from "@/features/categories/types/category.types";
import { useState } from "react";

export type ExpenseFilterValues = {
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

  const updateFilter = (name: keyof ExpenseFilterValues, value: string) => {
    setDraftValues((current) => ({
      ...current,
      [name]: value,
    }));
  };

  return (
    <FilterPanel
      title="Expense Filters"
      description="Filter expenses by date range and category."
      onSearch={() => onSearch(draftValues)}
      onClear={onClear}
    >
      <FilterDate
        label="From Date"
        value={draftValues.fromDate}
        onChange={(value) => updateFilter("fromDate", value)}
      />

      <FilterDate
        label="To Date"
        value={draftValues.toDate}
        onChange={(value) => updateFilter("toDate", value)}
      />

      <FilterSelect
        label="Category"
        value={draftValues.categoryId}
        disabled={isLoadingCategories}
        placeholder="All Categories"
        options={categories.map((category) => ({
          label: category.name,
          value: String(category.id),
        }))}
        onChange={(value) => updateFilter("categoryId", value)}
      />
    </FilterPanel>
  );
}
