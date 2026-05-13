import {
  useCallback,
  useEffect,
  useMemo,
  useState,
  type PropsWithChildren,
} from "react";
import { authApi } from "../api/authApi";
import type { LoginRequest, RegisterRequest, User } from "../types/auth.types";
import { tokenStore } from "../utils/tokenStore";
import { AuthContext, type AuthContextValue } from "./auth-context";
import {
  registerAuthFailureHandler,
  unregisterAuthFailureHandler,
} from "@/lib/api/refreshManager";

export function AuthProvider({ children }: PropsWithChildren) {
  const [user, setUser] = useState<User | null>(null);
  const [isInitializing, setIsInitializing] = useState(true);

  const clearSession = useCallback(() => {
    tokenStore.clear();
    setUser(null);
  }, []);

  const register = useCallback(async (request: RegisterRequest) => {
    const result = await authApi.register(request);

    if (!result.isSuccess || !result.data?.accessToken || !result.data.user) {
      throw new Error(result.errors?.[0]?.message ?? "Registration failed.");
    }

    tokenStore.setAccessToken(result.data.accessToken);
    tokenStore.setSessionHint();
    setUser(result.data.user);
  }, []);

  const login = useCallback(async (request: LoginRequest) => {
    const result = await authApi.login(request);

    if (!result.isSuccess || !result.data?.accessToken || !result.data.user) {
      throw new Error(result.errors?.[0]?.message ?? "Login failed.");
    }

    tokenStore.setAccessToken(result.data.accessToken);
    tokenStore.setSessionHint();
    setUser(result.data.user);
  }, []);

  const logout = useCallback(async () => {
    try {
      await authApi.logout();
    } finally {
      clearSession();
    }
  }, [clearSession]);

  useEffect(() => {
    registerAuthFailureHandler(clearSession);

    return () => {
      unregisterAuthFailureHandler();
    };
  }, [clearSession]);

  useEffect(() => {
    const initializeAuth = async () => {
      if (!tokenStore.hasSessionHint()) {
        setIsInitializing(false);
        return;
      }

      try {
        const refreshResult = await authApi.refresh();

        if (
          !refreshResult.isSuccess ||
          !refreshResult.data?.accessToken ||
          !refreshResult.data.user
        ) {
          clearSession();
          return;
        }

        tokenStore.setAccessToken(refreshResult.data.accessToken);
        tokenStore.setSessionHint();
        setUser(refreshResult.data.user);
      } catch {
        clearSession();
      } finally {
        setIsInitializing(false);
      }
    };

    void initializeAuth();
  }, [clearSession]);

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: !!user,
      isInitializing,
      login,
      register,
      logout,
      setUser,
      clearSession,
    }),
    [user, isInitializing, login, register, logout, clearSession],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
