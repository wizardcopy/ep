using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edgar.GraphBasedGenerator.Grid2D;
using UnityEngine;

namespace Edgar.Unity
{
    internal class RandomFixedLevelGraphInputTaskGrid2D : PipelineTask<DungeonGeneratorPayloadGrid2D>
    {
        private readonly FixedLevelGraphConfigGrid2D config;

        public RandomFixedLevelGraphInputTaskGrid2D(FixedLevelGraphConfigGrid2D config)
        {
            this.config = config;
        }

        public override IEnumerator Process()
        {
            if (config.LevelGraphs == null || config.LevelGraphs.Count == 0)
            {
                throw new ConfigurationException("The LevelGraphs list must not be empty. Please assign at least one level graph in the Input config section of the generator component.");
            }

            // Randomly select a LevelGraph
            var selectedLevelGraph = config.GetRandomLevelGraph();

            if (selectedLevelGraph.Rooms.Count == 0)
            {
                throw new ConfigurationException($"Each level graph must contain at least one room. Please add some rooms to the selected level graph called \"{selectedLevelGraph.name}\".");
            }

            var levelDescription = new LevelDescriptionGrid2D();

            // Setup individual rooms
            foreach (var room in selectedLevelGraph.Rooms)
            {
                var roomTemplates = InputSetupUtils.GetRoomTemplates(room, selectedLevelGraph.DefaultRoomTemplateSets, selectedLevelGraph.DefaultIndividualRoomTemplates);

                if (roomTemplates.Count == 0)
                {
                    throw new ConfigurationException($"There are no room templates for the room \"{room.GetDisplayName()}\" and also no room templates in the default set of room templates. Please make sure that the room has at least one room template available.");
                }

                levelDescription.AddRoom(room, roomTemplates);
            }

            var typeOfRooms = selectedLevelGraph.Rooms.First().GetType();

            // Add passages
            foreach (var connection in selectedLevelGraph.Connections)
            {
                if (config.UseCorridors)
                {
                    var corridorRoom = (RoomBase) ScriptableObject.CreateInstance(typeOfRooms);

                    if (corridorRoom is Room basicRoom)
                    {
                        basicRoom.Name = "Corridor";
                    }

                    levelDescription.AddCorridorConnection(connection, corridorRoom,
                        InputSetupUtils.GetRoomTemplates(connection, selectedLevelGraph.CorridorRoomTemplateSets, selectedLevelGraph.CorridorIndividualRoomTemplates));
                }
                else
                {
                    levelDescription.AddConnection(connection);
                }
            }

            InputSetupUtils.CheckIfDirected(levelDescription, selectedLevelGraph);

            Payload.LevelDescription = levelDescription;

            yield return null;
        }
    }
}