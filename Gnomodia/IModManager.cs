/*
 *  Gnomodia
 *
 *  Copyright © 2013 Faark (http://faark.de/)
 *  Copyright © 2013, 2014 Alexander Krivács Schrøder (https://alexanderschroeder.net/)
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

using System.Collections.Generic;
using Gnomodia.Events;

namespace Gnomodia
{
    internal interface IModManager
    {
        IModMetadata[] ModMetadata { get; }
        IModMetadata GetModMetadata(IMod mod);

        IMod CreateOrGet(IModMetadata metadata);
        IMod CreateOrGet(string modId);

        IMod[] CreateOrGetAllMods();
        
        void OnPreGameInitializeEvent(PreGameInitializeEventArgs args);
        void OnPostGameInitializeEvent(PostGameInitializeEventArgs args);
        void OnPreSaveGameEvent(PreSaveGameEventArgs args);
        void OnPostSaveGameEvent(PostSaveGameEventArgs args);
        void OnPreLoadGameEvent(PreLoadGameEventArgs args);
        void OnPostLoadGameEvent(PostLoadGameEventArgs args);
    }
}