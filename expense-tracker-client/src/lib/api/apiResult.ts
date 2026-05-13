import type { ErrorItem } from "./api-error";

export interface ApiResult<T> {
  isSuccess: boolean;
  errors: ErrorItem[] | null;
  hasErrors: boolean;
  data: T | null;
}

export function unwrapApiResult<T>(
  result: ApiResult<T>,
  fallbackMessage: string,
): T {
  if (!result.isSuccess || result.data === null) {
    throw new Error(result.errors?.[0]?.message ?? fallbackMessage);
  }

  return result.data;
}
