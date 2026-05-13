type PaginationProps = {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  onPageChange: (page: number) => void;
};

export function Pagination({
  pageNumber,
  pageSize,
  totalCount,
  totalPages,
  onPageChange,
}: PaginationProps) {
  if (totalPages <= 1) return null;

  const startItem = (pageNumber - 1) * pageSize + 1;
  const endItem = Math.min(pageNumber * pageSize, totalCount);

  return (
    <div className="mt-4 flex items-center justify-between text-sm text-gray-600">
      <p>
        Showing {startItem}-{endItem} of {totalCount}
      </p>

      <div className="flex items-center gap-2">
        <button
          type="button"
          disabled={pageNumber <= 1}
          onClick={() => onPageChange(pageNumber - 1)}
          className="rounded-lg border px-3 py-1 disabled:opacity-50"
        >
          Previous
        </button>

        <span>
          Page {pageNumber} of {totalPages}
        </span>

        <button
          type="button"
          disabled={pageNumber >= totalPages}
          onClick={() => onPageChange(pageNumber + 1)}
          className="rounded-lg border px-3 py-1 disabled:opacity-50"
        >
          Next
        </button>
      </div>
    </div>
  );
}
