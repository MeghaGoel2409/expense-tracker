export const categoryStyleMap: Record<
  string,
  { icon: string; className: string }
> = {
  Food: {
    icon: "🍔",
    className: "bg-orange-50 text-orange-700",
  },
  Travel: {
    icon: "✈️",
    className: "bg-blue-50 text-blue-700",
  },
  Shopping: {
    icon: "🛍️",
    className: "bg-pink-50 text-pink-700",
  },
  Bills: {
    icon: "💡",
    className: "bg-yellow-50 text-yellow-700",
  },
  Entertainment: {
    icon: "🎬",
    className: "bg-purple-50 text-purple-700",
  },
  Health: {
    icon: "💊",
    className: "bg-green-50 text-green-700",
  },
};

export function getCategoryStyle(category?: string | null) {
  if (!category) {
    return {
      icon: "📦",
      className: "bg-gray-100 text-gray-700",
    };
  }

  return (
    categoryStyleMap[category] ?? {
      icon: "📦",
      className: "bg-gray-100 text-gray-700",
    }
  );
}
