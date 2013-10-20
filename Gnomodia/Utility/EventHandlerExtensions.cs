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

namespace Gnomodia.Utility
{
    public static class EventHandlerExtensions
    {
        public static void TryRaise<T, T2, T3>(this EventHandler<EventArgs<T, T2, T3>> handler, object self, T arg1, T2 arg2, T3 arg3)
        {
            if (handler != null)
            {
                handler.Invoke(self, new EventArgs<T, T2, T3>(arg1, arg2, arg3));
            }
        }
        public static void TryRaise<T, T2>(this EventHandler<EventArgs<T, T2>> handler, object self, T arg1, T2 arg2)
        {
            if (handler != null)
            {
                handler.Invoke(self, new EventArgs<T, T2>(arg1, arg2));
            }
        }
        public static void TryRaise<T>(this EventHandler<EventArgs<T>> handler, object self, T args)
        {
            if (handler != null)
            {
                handler.Invoke(self, new EventArgs<T>(args));
            }
        }
        public static void TryRaise(this EventHandler handler, object self)
        {
            if (handler != null)
            {
                handler.Invoke(self, new EventArgs());
            }
        }
        public static void TryRaise<T>(this EventHandler<T> handler, object self, T args) where T : EventArgs
        {
            if (handler != null)
            {
                handler.Invoke(self, args);
            }
        }
    }
}