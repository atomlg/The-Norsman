using _Project.Scripts.Rewards;
using UnityEngine;

namespace _Project.Scripts.Interactions
{
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
}