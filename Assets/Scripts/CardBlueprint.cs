using Effects;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Create CardBlueprint", fileName = "CardBlueprint", order = 0)]
public class CardBlueprint : SerializedScriptableObject
{
    
    private string Name => name;
    [SerializeField] private TargetingType TargetingType;
    [SerializeField] private int EnergyCost;
    [SerializeField] private IEffect[] Effects;
    
    public Card CreateCard()
    {
        return new Card(Name, Id, TargetingType, EnergyCost, Effects);
    }
    
    public string Id => name.ToLower().Replace(" ", "_");
}