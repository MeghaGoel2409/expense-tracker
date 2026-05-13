import axios from "axios";
import { tokenStore } from "@/features/auth/utils/tokenStore";
import { handleUnauthorizedError } from "./refreshManager";

const baseURL = import.meta.env.VITE_API_BASE_URL;

if (!baseURL) {
  throw new Error("VITE_API_BASE_URL is not configured.");
}

export const apiClient = axios.create({
  baseURL,
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use((config) => {
  const accessToken = tokenStore.getAccessToken();

  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }

  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const status = error.response?.status;
    const requestUrl = error.config?.url as string | undefined;

    const isLoginRequest = requestUrl?.includes("auth/login");
    const isRegisterRequest = requestUrl?.includes("auth/register");
    const isRefreshRequest = requestUrl?.includes("/auth/refresh");
    const isLogoutRequest = requestUrl?.includes("/auth/logout");

    if (
      status !== 401 ||
      isLoginRequest ||
      isRegisterRequest ||
      isRefreshRequest ||
      isLogoutRequest
    ) {
      return Promise.reject(error);
    }

    return handleUnauthorizedError(error, (config) => apiClient(config));
  },
);
