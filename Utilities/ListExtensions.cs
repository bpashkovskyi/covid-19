namespace Covid19.Utilities
{
    using System.Collections.Generic;

    internal static class ListExtensions
    {
        public static T GetPreviousElement<T>(this List<T> collection, T currentElement, int shift)
        {
            var currentElementIndex = collection.IndexOf(currentElement);
            var previousElementIndex = currentElementIndex - shift;

            if (previousElementIndex < 0)
            {
                return default;
            }

            var previousElement = collection[previousElementIndex];
            return previousElement;
        }
    }
}