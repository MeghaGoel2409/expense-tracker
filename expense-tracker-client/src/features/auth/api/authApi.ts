import { apiClient } from "@/lib/api/axios";
import { endpoints } from "@/lib/api/endpoints";
import type {
  AuthResponse,
  AuthUserResponse,
  LoginRequest,
  RefreshTokenResponse,
  RegisterRequest,
} from "../types/auth.types";

export const authApi = {
  async login(request: LoginRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>(
      endpoints.auth.login,
      request,
    );
    return response.data;
  },

  async register(request: RegisterRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>(
      endpoints.auth.register,
      request,
    );
    return response.data;
  },

  async refresh(): Promise<RefreshTokenResponse> {
    const response = await apiClient.post<RefreshTokenResponse>(
      endpoints.auth.refresh,
    );
    return response.data;
  },

  async me(): Promise<AuthUserResponse> {
    const response = await apiClient.get<AuthUserResponse>(endpoints.auth.me);
    return response.data;
  },

  async logout(): Promise<void> {
    await apiClient.post(endpoints.auth.logout);
  },
};
