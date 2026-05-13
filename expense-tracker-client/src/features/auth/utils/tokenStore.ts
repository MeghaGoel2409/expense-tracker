let accessToken: string | null = null;

const SESSION_HINT_KEY = "expense_tracker_has_session";

export const tokenStore = {
  getAccessToken(): string | null {
    return accessToken;
  },

  setAccessToken(token: string | null): void {
    accessToken = token;
  },

  clearAccessToken(): void {
    accessToken = null;
  },

  setSessionHint(): void {
    localStorage.setItem(SESSION_HINT_KEY, "true");
  },

  hasSessionHint(): boolean {
    return localStorage.getItem(SESSION_HINT_KEY) === "true";
  },

  clearSessionHint(): void {
    localStorage.removeItem(SESSION_HINT_KEY);
  },

  clear(): void {
    accessToken = null;
    localStorage.removeItem(SESSION_HINT_KEY);
  },
};
