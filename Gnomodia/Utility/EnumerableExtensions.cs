using System;
using System.Collections.Generic;
using System.Linq;

namespace Gnomodia.Utility
{
    public static class EnumerableExtensions
    {
        public static string Join(this IEnumerable<string> source, string seperator)
        {
            return string.Join(seperator, source);
        }

        public static T ElementAfterOrDefault<T>(this IEnumerable<T> source, T beforeElement)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var comparer = EqualityComparer<T>.Default;
            bool foundIt = false;
            foreach (var element in source)
            {
                if (foundIt)
                {
                    return element;
                }
                
                if (comparer.Equals(element, beforeElement))
                {
                    foundIt = true;
                }
            }
            return default(T);
        }

        public static T ElementBeforeOrDefault<T>(this IEnumerable<T> source, T afterElement)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var comparer = EqualityComparer<T>.Default;
            T previous = default(T);
            foreach (var element in source)
            {
                if (comparer.Equals(element, afterElement))
                {
                    return previous;
                }
                
                previous = element;
            }
            return default(T);
        }

        public static T ElementAfterOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            bool foundIt = false;
            foreach (var element in source)
            {
                if (foundIt)
                {
                    return element;
                }
                
                if (predicate(element))
                {
                    foundIt = true;
                }
            }
            return default(T);
        }

        public static T ElementBeforeOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            T previous = default(T);
            foreach (var element in source)
            {
                if (predicate(element))
                {
                    return previous;
                }
                
                previous = element;
            }
            return default(T);
        }

        public static T ElementAfter<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            bool foundIt = false;
            foreach (var element in source)
            {
                if (foundIt)
                {
                    return element;
                }
                
                if (predicate(element))
                {
                    foundIt = true;
                }
            }

            if (foundIt)
            {
                throw new InvalidOperationException("Found element is the last item of the collection");
            }
            
            throw new InvalidOperationException("No elements in collection match the condition");
        }

        public static T ElementAfter<T>(this IEnumerable<T> source, T beforeElement)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var comparer = EqualityComparer<T>.Default;
            bool foundIt = false;
            foreach (var element in source)
            {
                if (foundIt)
                {
                    return element;
                }
                
                if (comparer.Equals(element, beforeElement))
                {
                    foundIt = true;
                }
            }

            if (foundIt)
            {
                throw new InvalidOperationException("Found element is the last item of the collection");
            }
            
            throw new InvalidOperationException("No elements in collection match the condition");
        }

        public static int IndexOf<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int index = 0;
            foreach (var item in source)
            {
                if (predicate(item)) return index;
                index++;
            }
            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T element, IEqualityComparer<T> comparer)
        {
            return IndexOf(source, e => comparer.Equals(e, element));
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T element)
        {
            return IndexOf(source, element, EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> Union<T>(this IEnumerable<T> source, T element)
        {
            return source.Union(new[] { element });
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T element)
        {
            return source.Concat(new[] { element });
        }

        public static bool SequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, T, bool> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (first == null)
            {
                throw new ArgumentNullException("first");
            }
            if (second == null)
            {
                throw new ArgumentNullException("second");
            }

            using (var enumerator = first.GetEnumerator())
            {
                using (var enumerator2 = second.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator2.MoveNext() || !comparer(enumerator.Current, enumerator2.Current))
                        {
                            return false;
                        }
                    }
                    if (enumerator2.MoveNext())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //http://stackoverflow.com/questions/1577822/passing-a-single-item-as-ienumerablet
        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the wrapped object.</typeparam>
        /// <param name="item"> The object to wrap.</param>
        /// <returns>
        /// An IEnumerable&lt;T&gt; consisting of a single item.
        /// </returns>
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}