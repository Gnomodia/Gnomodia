/*
 *  Gnomodia
 *
 *  Copyright © 2013 Faark (http://faark.de/)
 *  Copyright © 2013 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU Lesser General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU Lesser General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Gnomodia.Utility
{
    public static class DependencySorter
    {
        private class DependencySortElement<T>
        {
            public readonly T Element;
            public readonly HashSet<T> Dependencies;
            public DependencySortElement(Tuple<T, IEnumerable<T>> data)
            {
                Element = data.Item1;
                Dependencies = new HashSet<T>();
                foreach (var element in data.Item2)
                {
                    Dependencies.Add(element);
                }
            }
        }

        public static List<T> Sort<T>(IEnumerable<Tuple<T, IEnumerable<T>>> data)
        {
            var openNodes = new HashSet<DependencySortElement<T>>();
            var noDependencyNodes = new Queue<DependencySortElement<T>>();
            var processedNodes = new List<T>();
            foreach (var sortElement in data.Select(e => new DependencySortElement<T>(e)))
            {
                if (sortElement.Dependencies.Count <= 0)
                {
                    noDependencyNodes.Enqueue(sortElement);
                }
                else
                {
                    openNodes.Add(sortElement);
                }
            }
            while (noDependencyNodes.Count > 0)
            {
                var node = noDependencyNodes.Dequeue();
                openNodes.RemoveWhere(el =>
                {
                    if (el.Dependencies.Contains(node.Element))
                    {
                        el.Dependencies.Remove(node.Element);
                        if (el.Dependencies.Count <= 0)
                        {
                            noDependencyNodes.Enqueue(el);
                            return true;
                        }
                    }
                    return false;
                });
                processedNodes.Add(node.Element);
            }
            if (openNodes.Count > 0)
            {
                throw new InvalidOperationException("Cannot sort by dependency (graph has at least one cycle or unlisted nodes)");
            }
            return processedNodes;
        }
    }
}