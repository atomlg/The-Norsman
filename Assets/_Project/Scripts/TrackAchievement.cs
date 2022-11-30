using UnityEngine;

public class TrackAchievement : InteractionHandler
{
    public Achievements Achievements;
    public string Identifier;
    
    public override void Interact()
    {
        //Achievements.Track(Identifier);
        Debug.Log("Track achievements");
    }
}