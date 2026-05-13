import { Link } from "react-router-dom";
import { type ReactNode } from "react";

type AuthFormFooterProps = {
  text: ReactNode;
  linkText: string;
  to: string;
};

export function AuthFormFooter({ text, linkText, to }: AuthFormFooterProps) {
  return (
    <p className="mt-6 text-sm text-gray-600">
      {text}{" "}
      <Link to={to} className="font-medium text-black underline">
        {linkText}
      </Link>
    </p>
  );
}
