using Effects;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Create CardBlueprint", fileName = "CardBlueprint", order = 0)]
public class CardBlueprint : SerializedScriptableObject
{
    [SerializeField] private TargetingType TargetingType;
    [SerializeField] private int EnergyCost;
    [SerializeField] private IEffect[] Effects;

    private string Name => name;

    public string Id => name.ToLower().Replace(" ", "_");

    public Card CreateCard()
    {
        return new Card(Name, Id, TargetingType, EnergyCost, Effects);
    }
}