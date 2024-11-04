using Unity.Mathematics;
using ProtoBuf;

// Contains just the data of the player; purely numerical
[ProtoContract]
public class PlayerData
{
    [ProtoMember(1)]
    public float3 position;
    [ProtoMember(2)]
    public float3 lastPosition;
    [ProtoMember(3)]
    public quaternion rotation;
    [ProtoMember(4)] 
    public float health;
    [ProtoMember(5)] 
    public float maxHealth;

    public PlayerData()
    {
    }
    
    public PlayerData(float3 position, float3 lastPosition, quaternion rotation, float health, float maxHealth)
    {
        this.position = position;
        this.lastPosition = lastPosition;
        this.rotation = rotation;
        this.health = health;
        this.maxHealth = maxHealth;
    }
}
