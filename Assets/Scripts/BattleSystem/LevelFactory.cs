using System.Collections.Generic;
using System.Linq;
using BattleSystem;
using YamlDotNet.Serialization;

public class LevelFactory
{
    private Dictionary<string, LevelData> _levelData = new();
    public LevelFactory(string rawLevelData)
    {
        var cardFactory = GlobalState.GetCardFactory();
        var deserializer = new DeserializerBuilder().Build();
        var levelData = deserializer.Deserialize<LevelData[]>(rawLevelData);

        foreach (var levelInfo in levelData)
        {
            _levelData.Add(levelInfo.id, levelInfo);
        }
        
        // var cards = cardDatas.Select(x => x.CreateCardBlueprint());
        // foreach (var blueprint in cards) _cardBlueprints.Add(blueprint.Id, blueprint);
    }

    private struct LevelData
    {
        public string name;
        public string id;
        public EncounterData[] encounters;
    }

    private struct EncounterData
    {
        public EncounterEnemy[] enemies;
    }

    private struct EncounterEnemy
    {
        public string name;
        public int hp;
        public string[] cards;
    }

    public Level GetLevel(string id)
    {
        var levelInfo = _levelData[id];
        var encounters = new Encounter[levelInfo.encounters.Length];
        for (var i = 0; i < levelInfo.encounters.Length; i++)
        {
            var encounterInfo = levelInfo.encounters[i];
            var enemies = encounterInfo.enemies.Select(x =>
                new Enemy(x.name, x.hp, x.cards)).ToArray();
            var encounter = new Encounter(enemies);
            encounters[i] = encounter;
        }
        var level = new Level(levelInfo.name, encounters);
        return level;
    }
}