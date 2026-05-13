import { useMutation } from "@tanstack/react-query";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from "./useAuth";
import type { LoginRequest } from "../types/auth.types";
import { getApiErrorMessage } from "@/lib/api/api-error";

type LocationState = {
  from?: {
    pathname?: string;
  };
};

export function useLogin() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const state = location.state as LocationState | null;
  const redirectTo = state?.from?.pathname || "/";

  return useMutation<void, Error, LoginRequest>({
    mutationFn: async (request: LoginRequest) => {
      try {
        await login(request);
      } catch (error) {
        throw new Error(getApiErrorMessage(error, "Login failed."));
      }
    },
    onSuccess: () => {
      navigate(redirectTo, { replace: true });
    },
  });
}
