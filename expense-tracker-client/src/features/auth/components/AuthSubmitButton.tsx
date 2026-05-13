type AuthSubmitButtonProps = {
  isLoading: boolean;
  idleText: string;
  loadingText: string;
};

export function AuthSubmitButton({
  isLoading,
  idleText,
  loadingText,
}: AuthSubmitButtonProps) {
  return (
    <button
      type="submit"
      disabled={isLoading}
      className="w-full rounded-lg bg-black px-4 py-2 text-sm font-medium text-white disabled:cursor-not-allowed disabled:opacity-60"
    >
      {isLoading ? loadingText : idleText}
    </button>
  );
}
