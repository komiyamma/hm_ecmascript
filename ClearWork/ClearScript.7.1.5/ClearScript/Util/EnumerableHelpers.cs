// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.ClearScript.Util.COM;

namespace Microsoft.ClearScript.Util
{
    // ReSharper disable once PartialTypeWithSinglePart
    internal static partial class EnumerableHelpers
    {
        public static readonly HostType HostType = HostType.Wrap(typeof(EnumerableHelpers));

        public static IList<T> ToIList<T>(this IEnumerable<T> source)
        {
            return (source as IList<T>) ?? source.ToList();
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var index = 0;
            foreach (var element in source)
            {
                action(element, index++);
            }
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (var element in source)
            {
                yield return element;

                foreach (var descendant in selector(element).Flatten(selector))
                {
                    yield return descendant;
                }
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this T element)
        {
            yield return element;
        }

        public static IEnumerable<string> ExcludeIndices(this IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                if (!int.TryParse(name, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                {
                    yield return name;
                }
            }
        }

        public static IEnumerable<int> GetIndices(this IEnumerable<string> names)
        {
            foreach (var name in names)
            {
                if (int.TryParse(name, NumberStyles.Integer, CultureInfo.InvariantCulture, out var index))
                {
                    yield return index;
                }
            }
        }

        public static IEnumerator<T> GetEnumerator<T>(IEnumerable<T> source)
        {
            return source.GetEnumerator();
        }

        public static IEnumerator GetEnumerator(IEnumerable source)
        {
            return source.GetEnumerator();
        }
    }

    internal sealed class EnumeratorWrapper : IEnumerator
    {
        private readonly IEnumVARIANT enumVariant;

        public EnumeratorWrapper(IEnumVARIANT enumVariant)
        {
            this.enumVariant = enumVariant;
        }

        public bool MoveNext()
        {
            var items = new object[1];
            if (enumVariant.Next(1, items, IntPtr.Zero) == HResult.S_OK)
            {
                Current = items[0];
                return true;
            }

            return false;
        }

        public void Reset()
        {
            enumVariant.Reset();
        }

        public object Current { get; private set; }
    }
}
