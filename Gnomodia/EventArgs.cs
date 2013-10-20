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
    public class EventArgs<T> : EventArgs
    {
        public T Argument;
        public EventArgs(T arg)
        {
            Argument = arg;
        }
    }

    public class EventArgs<T, T2> : EventArgs<T>
    {
        public T2 Argument2;
        public EventArgs(T arg, T2 arg2)
            : base(arg)
        {
            Argument2 = arg2;
        }
    }
    
    public class EventArgs<T, T2, T3> : EventArgs<T, T2>
    {
        public T3 Argument3;
        public EventArgs(T arg, T2 arg2, T3 arg3)
            : base(arg, arg2)
        {
            Argument3 = arg3;
        }
    }
}