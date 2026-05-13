import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import { tokenStore } from "@/features/auth/utils/tokenStore";
import type { RefreshTokenResponse } from "@/features/auth/types/auth.types";
import { endpoints } from "./endpoints";

type AuthFailureHandler = () => void;

type RetryableRequestConfig = InternalAxiosRequestConfig & {
  _retry?: boolean;
};

let isRefreshing = false;
let refreshPromise: Promise<string | null> | null = null;
let onAuthFailure: AuthFailureHandler | null = null;

function getRefreshUrl(): string {
  const baseURL = import.meta.env.VITE_API_BASE_URL;

  if (!baseURL) {
    throw new Error("VITE_API_BASE_URL is not configured.");
  }

  return `${baseURL}${endpoints.auth.refresh}`;
}

async function refreshAccessToken(): Promise<string | null> {
  try {
    const response = await axios.post(getRefreshUrl(), undefined, {
      withCredentials: true,
      headers: {
        "Content-Type": "application/json",
      },
    });

    const result = response.data as RefreshTokenResponse;

    const newAccessToken = result.data?.accessToken;

    if (!result.isSuccess || !newAccessToken) {
      tokenStore.clear();
      return null;
    }

    tokenStore.setAccessToken(newAccessToken);
    tokenStore.setSessionHint();

    return newAccessToken;
  } catch {
    tokenStore.clear();
    return null;
  }
}

export function registerAuthFailureHandler(handler: AuthFailureHandler): void {
  onAuthFailure = handler;
}

export function unregisterAuthFailureHandler(): void {
  onAuthFailure = null;
}

export async function handleUnauthorizedError(
  error: AxiosError,
  retryRequest: (config: InternalAxiosRequestConfig) => Promise<unknown>,
): Promise<unknown> {
  const originalRequest = error.config as RetryableRequestConfig | undefined;

  if (!originalRequest) {
    return Promise.reject(error);
  }

  if (originalRequest._retry) {
    onAuthFailure?.();
    return Promise.reject(error);
  }

  originalRequest._retry = true;

  if (!isRefreshing) {
    isRefreshing = true;
    refreshPromise = refreshAccessToken().finally(() => {
      isRefreshing = false;
    });
  }

  const newAccessToken = await refreshPromise;

  if (!newAccessToken) {
    onAuthFailure?.();
    return Promise.reject(error);
  }

  originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;

  return retryRequest(originalRequest);
}
