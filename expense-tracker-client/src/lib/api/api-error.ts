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

export async function getApiErrorMessage(
  error: unknown,
  fallback = "Something went wrong.",
): Promise<string> {
  if (axios.isAxiosError(error)) {
    let data = error.response?.data;

    if (data instanceof Blob) {
      try {
        data = JSON.parse(await data.text());
      } catch {
        data = undefined;
      }
    }

    const result = data as ApiErrorResult | undefined;

    const firstMessage = result?.errors?.find((x) => x.message)?.message;
    if (firstMessage) {
      return firstMessage;
    }

    switch (error.response?.status) {
      case 400:
        return "Please check your input and try again.";

      case 401:
        return "Invalid email or password.";

      case 403:
        return "You are not allowed to perform this action.";

      case 404:
        return "The requested resource was not found.";

      case 409:
        return "A conflicting record already exists.";

      default:
        break;
    }
  }

  if (error instanceof Error && error.message) {
    return error.message;
  }

  return fallback;
}
