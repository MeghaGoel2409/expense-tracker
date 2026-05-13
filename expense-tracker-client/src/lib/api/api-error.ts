import axios from "axios";

export type ErrorItem = {
  code: string | null;
  message: string | null;
  type:
    | "Validation"
    | "NotFound"
    | "Forbidden"
    | "Unauthorized"
    | "Conflict"
    | "Failure";
};

type ApiErrorResult = {
  isSuccess?: boolean;
  errors?: ErrorItem[] | null;
  hasErrors?: boolean;
};

export function getApiErrorMessage(
  error: unknown,
  fallback = "Something went wrong.",
): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ApiErrorResult | undefined;

    const firstMessage = data?.errors?.find((x) => x.message)?.message;
    if (firstMessage) {
      return firstMessage;
    }

    if (error.response?.status === 401) {
      return "Invalid email or password.";
    }

    if (error.response?.status === 403) {
      return "You are not allowed to perform this action.";
    }

    if (error.response?.status === 409) {
      return "A conflicting record already exists.";
    }

    if (error.response?.status === 400) {
      return "Please check your input and try again.";
    }
  }

  if (error instanceof Error && error.message) {
    return error.message;
  }

  return fallback;
}
