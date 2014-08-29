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

namespace Gnomodia
{
    public interface IMod
    {
        String Name { get; }
        String Description { get; }
        String Author { get; }
        Version Version { get; }
        IEnumerable<IModification> Modifications { get; }
        /*IEnumerable<ModDependency> Dependencies { get; }
        IEnumerable<ModType> InitAfter { get; }
        IEnumerable<ModType> InitBefore { get; }
        String SetupData { get; set; }

        void Initialize_PreGame();
        void Initialize_ModDiscovery();
        void Initialize_PreGeneration();

        void PreWorldCreation(ModSaveData data, Game.Map map, Game.CreateWorldOptions options);
        void PostWorldCreation(ModSaveData data, Game.Map map, Game.CreateWorldOptions options);
        void PreGameLoaded(ModSaveData data);
        void AfterGameLoaded(ModSaveData data);
        void PreGameSaved(ModSaveData data);
        void AfterGameSaved(ModSaveData data);*/
    }
}