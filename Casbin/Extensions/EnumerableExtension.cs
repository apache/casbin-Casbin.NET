// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

using System.Collections.Generic;

namespace Casbin
{
    internal static class EnumerableExtension
    {
        #region DeepEquals

        internal static bool DeepEquals<T>(this IEnumerable<T> enumerable, IEnumerable<T> anotherEnumerable)
        {
            if (enumerable is null || anotherEnumerable is null)
            {
                return false;
            }

            if (enumerable is T[] array && anotherEnumerable is T[] anotherArray)
            {
                return array.DeepEquals(anotherArray);
            }

            if (enumerable is IReadOnlyList<T> list && anotherEnumerable is IReadOnlyList<T> anotherList)
            {
                return list.DeepEquals(anotherList);
            }

            using var enumerator = enumerable.GetEnumerator();
            using var anotherEnumerator = anotherEnumerable.GetEnumerator();
            while (enumerator.MoveNext() && anotherEnumerator.MoveNext())
            {
                if (enumerator.Current is null || anotherEnumerator.Current is null)
                {
                    return false;
                }

                if (enumerator.Current.Equals(anotherEnumerator.Current) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DeepEquals<T>(this T[] array, T[] anotherArray)
        {
            if (array is null || anotherArray is null)
            {
                return false;
            }

            int length = array.Length;
            if (length != anotherArray.Length)
            {
                return false;
            }

            for (int index = 0; index < length; index++)
            {
                if (array[index].Equals(anotherArray[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DeepEquals<T>(this IReadOnlyList<T> list, IReadOnlyList<T> anotherList)
        {
            if (list is null || anotherList is null)
            {
                return false;
            }

            int length = list.Count;
            if (length != anotherList.Count)
            {
                return false;
            }

            for (int index = 0; index < length; index++)
            {
                if (list[index].Equals(anotherList[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DeepEquals<T>(this IEnumerable<IEnumerable<T>> enumerable, IEnumerable<IEnumerable<T>> anotherEnumerable)
        {
            if (enumerable is null || anotherEnumerable is null)
            {
                return false;
            }

            if (enumerable is IEnumerable<T>[] array && anotherEnumerable is IEnumerable<T>[] anotherArray)
            {
                return array.DeepEquals(anotherArray);
            }

            if (enumerable is IReadOnlyList<IEnumerable<T>> list && anotherEnumerable is IReadOnlyList<IEnumerable<T>> anotherList)
            {
                return list.DeepEquals(anotherList);
            }

            using var enumerator = enumerable.GetEnumerator();
            using var anotherEnumerator = anotherEnumerable.GetEnumerator();
            while (enumerator.MoveNext() && anotherEnumerator.MoveNext())
            {
                if (enumerator.Current is null || anotherEnumerator.Current is null)
                {
                    return false;
                }

                if (enumerator.Current.DeepEquals(anotherEnumerator.Current) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DeepEquals<T>(this IEnumerable<T>[] array, IEnumerable<T>[] anotherArray)
        {
            if (array is null || anotherArray is null)
            {
                return false;
            }

            int length = array.Length;
            if (length != anotherArray.Length)
            {
                return false;
            }

            for (int index = 0; index < length; index++)
            {
                if (array[index].DeepEquals(anotherArray[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DeepEquals<T>(this IReadOnlyList<IEnumerable<T>> list, IReadOnlyList<IEnumerable<T>> anotherList)
        {
            if (list is null || anotherList is null)
            {
                return false;
            }

            int length = list.Count;
            if (length != anotherList.Count)
            {
                return false;
            }

            for (int index = 0; index < length; index++)
            {
                if (list[index].DeepEquals(anotherList[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region SetEquals
        internal static bool SetEquals<T>(this IEnumerable<T> enumerable, IEnumerable<T> anotherEnumerable)
        {
            if (enumerable is null || anotherEnumerable is null)
            {
                return false;
            }

            if (enumerable is T[] array && anotherEnumerable is T[] anotherArray)
            {
                return array.SetEquals(anotherArray);
            }

            if (enumerable is IReadOnlyList<T> list && anotherEnumerable is IReadOnlyList<T> anotherList)
            {
                return list.SetEquals(anotherList);
            }

            var set = new HashSet<T>(enumerable);
            var anotherSet = new HashSet<T>(anotherEnumerable);

            return set.SetEquals(anotherSet);
        }

        internal static bool SetEquals<T>(this T[] array, T[] anotherArray)
        {
            if (array is null || anotherArray is null)
            {
                return false;
            }

            var set = new HashSet<T>(array);
            var anotherSet = new HashSet<T>(anotherArray);

            return set.SetEquals(anotherSet);
        }

        internal static bool SetEquals<T>(this IReadOnlyList<T> list, IReadOnlyList<T> anotherList)
        {
            if (list is null || anotherList is null)
            {
                return false;
            }

            var set = new HashSet<T>(list);
            var anotherSet = new HashSet<T>(anotherList);

            return set.SetEquals(anotherSet);
        }
        #endregion
    }
}
