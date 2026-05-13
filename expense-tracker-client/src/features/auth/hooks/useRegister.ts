import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { useAuth } from "./useAuth";
import type { RegisterRequest } from "../types/auth.types";
import { getApiErrorMessage } from "@/lib/api/api-error";

export function useRegister() {
  const { register } = useAuth();
  const navigate = useNavigate();

  return useMutation<void, Error, RegisterRequest>({
    mutationFn: async (request: RegisterRequest) => {
      try {
        await register(request);
      } catch (error) {
        throw new Error(getApiErrorMessage(error, "Registration failed."));
      }
    },
    onSuccess: () => {
      navigate("/", { replace: true });
    },
  });
}
