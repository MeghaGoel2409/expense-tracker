import type { ApiResult } from "@/lib/api/apiResult";

export interface User {
  userId: string | null;
  email: string | null;
  firstName: string | null;
  lastName: string | null;
  fullName: string | null;
  roles: string[] | null;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface AuthTokenResponse {
  accessToken: string | null;
  expiresAtUtc: string;
  user: User | null;
}

export type AuthResponse = ApiResult<AuthTokenResponse>;
export type RefreshTokenResponse = ApiResult<AuthTokenResponse>;
export type AuthUserResponse = ApiResult<User>;
