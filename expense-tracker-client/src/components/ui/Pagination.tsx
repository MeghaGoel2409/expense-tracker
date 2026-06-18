import { useState } from "react";
import { ChevronLeft, ChevronRight } from "lucide-react";

type PaginationProps = {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  onPageChange: (page: number) => void;
};

function getVisiblePages(pageNumber: number, totalPages: number): number[] {
  const pages = new Set<number>();

  pages.add(1);
  pages.add(totalPages);
  pages.add(pageNumber);

  if (pageNumber > 1) pages.add(pageNumber - 1);
  if (pageNumber < totalPages) pages.add(pageNumber + 1);

  return Array.from(pages).sort((a, b) => a - b);
}

export function Pagination({
  pageNumber,
  pageSize,
  totalCount,
  totalPages,
  onPageChange,
}: PaginationProps) {
  const [pageInput, setPageInput] = useState("");

  if (totalPages <= 1) {
    return null;
  }

  const startItem = (pageNumber - 1) * pageSize + 1;
  const endItem = Math.min(pageNumber * pageSize, totalCount);
  const visiblePages = getVisiblePages(pageNumber, totalPages);
  const showGoToPage = totalPages > 7;

  const goToPage = () => {
    const page = Number(pageInput);

    if (!Number.isInteger(page) || page < 1 || page > totalPages) {
      return;
    }

    onPageChange(page);
    setPageInput("");
  };

  return (
    <div className="mt-4 flex flex-col gap-3 rounded-xl border border-gray-200 bg-white px-4 py-3 text-sm text-gray-600 sm:flex-row sm:items-center sm:justify-between">
      <p>
        Showing{" "}
        <span className="font-medium text-gray-900">
          {startItem}-{endItem}
        </span>{" "}
        of{" "}
        <span className="font-medium text-gray-900">
          {totalCount.toLocaleString()}
        </span>
      </p>

      <div className="flex flex-wrap items-center gap-2">
        <div className="flex items-center gap-1">
          <button
            type="button"
            disabled={pageNumber <= 1}
            onClick={() => onPageChange(pageNumber - 1)}
            aria-label="Go to previous page"
            className="inline-flex h-8 w-8 items-center justify-center rounded-md text-gray-600 transition hover:bg-gray-100 hover:text-gray-900 disabled:cursor-not-allowed disabled:opacity-40"
          >
            <ChevronLeft className="h-4 w-4" />
          </button>

          {visiblePages.map((page, index) => {
            const previousPage = visiblePages[index - 1];
            const showEllipsis = previousPage && page - previousPage > 1;
            const isSelected = page === pageNumber;

            return (
              <div key={page} className="flex items-center gap-1">
                {showEllipsis && (
                  <span className="px-1.5 text-sm text-gray-400">...</span>
                )}

                <button
                  type="button"
                  onClick={() => onPageChange(page)}
                  disabled={isSelected}
                  aria-current={isSelected ? "page" : undefined}
                  className={
                    isSelected
                      ? "inline-flex h-8 min-w-8 items-center justify-center rounded-full bg-gray-900 px-2.5 text-sm font-semibold text-white shadow-sm"
                      : "inline-flex h-8 min-w-8 items-center justify-center rounded-md px-2.5 text-sm font-medium text-gray-600 transition hover:bg-gray-100 hover:text-gray-900"
                  }
                >
                  {page}
                </button>
              </div>
            );
          })}

          <button
            type="button"
            disabled={pageNumber >= totalPages}
            onClick={() => onPageChange(pageNumber + 1)}
            aria-label="Go to next page"
            className="inline-flex h-8 w-8 items-center justify-center rounded-md text-gray-600 transition hover:bg-gray-100 hover:text-gray-900 disabled:cursor-not-allowed disabled:opacity-40"
          >
            <ChevronRight className="h-4 w-4" />
          </button>
        </div>

        {showGoToPage && (
          <div className="flex items-center gap-2 border-l border-gray-200 pl-3">
            <span className="text-sm text-gray-500">Go to</span>

            <input
              type="number"
              min={1}
              max={totalPages}
              value={pageInput}
              onChange={(event) => setPageInput(event.target.value)}
              onKeyDown={(event) => {
                if (event.key === "Enter") {
                  goToPage();
                }
              }}
              className="h-8 w-16 rounded-md border border-gray-300 px-2 text-sm text-gray-900 focus:border-gray-900 focus:outline-none focus:ring-1 focus:ring-gray-900"
              aria-label="Go to page number"
            />
          </div>
        )}
      </div>
    </div>
  );
}
