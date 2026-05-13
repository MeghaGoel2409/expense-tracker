import type { ReactNode } from "react";

type QueryStateProps = {
  isLoading: boolean;
  isError: boolean;
  isEmpty: boolean;
  errorMessage?: string;
  loadingMessage?: string;
  emptyTitle?: string;
  emptyDescription?: string;
  emptyIcon?: ReactNode;
  emptyAction?: ReactNode;
  onRetry?: () => void;
  children: ReactNode;
};

function LoadingSkeleton({ message }: { message: string }) {
  return (
    <div className="overflow-hidden rounded-2xl border bg-white shadow-sm">
      <div className="border-b bg-gray-50 px-6 py-4">
        <div className="h-5 w-40 animate-pulse rounded bg-gray-200" />
        <div className="mt-2 h-4 w-64 animate-pulse rounded bg-gray-200" />
      </div>

      <div className="divide-y">
        {Array.from({ length: 6 }).map((_, rowIndex) => (
          <div
            key={rowIndex}
            className="grid grid-cols-1 gap-4 px-6 py-4 md:grid-cols-6"
          >
            {Array.from({ length: 6 }).map((_, colIndex) => (
              <div
                key={colIndex}
                className="h-4 animate-pulse rounded bg-gray-100"
              />
            ))}
          </div>
        ))}
      </div>

      <div className="border-t bg-gray-50 px-6 py-3 text-sm text-gray-500">
        {message}
      </div>
    </div>
  );
}

function ErrorState({
  message,
  onRetry,
}: {
  message: string;
  onRetry?: () => void;
}) {
  return (
    <div className="rounded-2xl border border-red-200 bg-red-50 p-6 shadow-sm">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-start">
        <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-red-100 text-red-700">
          <span className="text-xl font-bold">!</span>
        </div>

        <div className="flex-1">
          <h3 className="text-base font-semibold text-red-900">
            Unable to load data
          </h3>

          <p className="mt-1 text-sm text-red-700">{message}</p>

          {onRetry && (
            <button
              type="button"
              onClick={onRetry}
              className="mt-4 rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800"
            >
              Try again
            </button>
          )}
        </div>
      </div>
    </div>
  );
}

function EmptyState({
  title,
  description,
  icon,
  action,
}: {
  title: string;
  description: string;
  icon?: ReactNode;
  action?: ReactNode;
}) {
  return (
    <div className="rounded-2xl border border-dashed bg-white p-10 text-center shadow-sm">
      <div className="mx-auto mb-5 flex h-20 w-20 items-center justify-center rounded-full bg-gray-100 text-4xl">
        {icon ?? "📄"}
      </div>

      <h3 className="text-lg font-semibold text-gray-900">{title}</h3>

      <p className="mx-auto mt-2 max-w-md text-sm leading-6 text-gray-500">
        {description}
      </p>

      {action && <div className="mt-6">{action}</div>}
    </div>
  );
}

export function QueryState({
  isLoading,
  isError,
  isEmpty,
  errorMessage = "Something went wrong. Please try again.",
  loadingMessage = "Loading...",
  emptyTitle = "No records found",
  emptyDescription = "Try changing your filters or add a new record.",
  emptyIcon,
  emptyAction,
  onRetry,
  children,
}: QueryStateProps) {
  if (isLoading) {
    return <LoadingSkeleton message={loadingMessage} />;
  }

  if (isError) {
    return <ErrorState message={errorMessage} onRetry={onRetry} />;
  }

  if (isEmpty) {
    return (
      <EmptyState
        title={emptyTitle}
        description={emptyDescription}
        icon={emptyIcon}
        action={emptyAction}
      />
    );
  }

  return <>{children}</>;
}
