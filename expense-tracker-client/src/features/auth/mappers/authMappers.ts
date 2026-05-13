// src/features/auth/mappers/authMappers.ts

import type {
  LoginFormValues,
  RegisterFormValues,
} from "../schemas/authSchemas";
import type { LoginRequest, RegisterRequest } from "../types/auth.types";

export function mapLoginToRequest(values: LoginFormValues): LoginRequest {
  return {
    email: values.email.trim(),
    password: values.password,
  };
}

export function mapRegisterToRequest(
  values: RegisterFormValues,
): RegisterRequest {
  return {
    firstName: values.firstName.trim(),
    lastName: values.lastName.trim(),
    email: values.email.trim(),
    password: values.password,
    confirmPassword: values.confirmPassword,
  };
}
