using System;
using System.Collections.Generic;

namespace holmgang.Desktop
{
    public class AISimpleDialogueComponent : BehaviourComponent
    {
        public List<string> speechs;
        int cur = 0;
        float cooldown = 3f; // in seconds
        float timer = 0f;

        public AISimpleDialogueComponent():base(){}
        public AISimpleDialogueComponent(params string[] speechs)
        {
            this.speechs = new List<string>(speechs);
        }

        public string getNextSpeech()
        {
            if(speechs.Count == 0 || timer > 0f)
                return "";
            if(cur >= speechs.Count)
                cur = 0;
            timer = cooldown;
            return speechs[cur++];
        }

        public void update(float deltaS)
        {
            timer -= deltaS;
            if(timer <= 0)
                timer = 0;
        }
    }
}
