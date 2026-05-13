import { FilterDate, FilterPanel, FilterSelect } from "@/components/ui/filters";
import type { Category } from "@/features/categories/types/category.types";

export type ExpenseFilterValues = {
  fromDate: string;
  toDate: string;
  categoryId: string;
};

type ExpenseFiltersProps = {
  values: ExpenseFilterValues;
  categories: Category[];
  isLoadingCategories?: boolean;
  onChange: (values: ExpenseFilterValues) => void;
  onSearch: () => void;
  onClear: () => void;
};

export function ExpenseFilters({
  values,
  categories,
  isLoadingCategories = false,
  onChange,
  onSearch,
  onClear,
}: ExpenseFiltersProps) {
  const updateFilter = (name: keyof ExpenseFilterValues, value: string) => {
    onChange({
      ...values,
      [name]: value,
    });
  };

  return (
    <FilterPanel
      title="Expense Filters"
      description="Filter expenses by date range and category."
      onSearch={onSearch}
      onClear={onClear}
    >
      <FilterDate
        label="From Date"
        value={values.fromDate}
        onChange={(value) => updateFilter("fromDate", value)}
      />

      <FilterDate
        label="To Date"
        value={values.toDate}
        onChange={(value) => updateFilter("toDate", value)}
      />

      <FilterSelect
        label="Category"
        value={values.categoryId}
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
