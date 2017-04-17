using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class TournamentStatusRuntime
    {
        partial void CustomInitialize()
        {
            
        }

        public void CustomActivity()
        {
            if (!Wave1AnimateAnimation.IsPlaying())
            {
                Wave1AnimateAnimation.Play();
            }
            if (!Wave2AnimateAnimation.IsPlaying())
            {
                Wave2AnimateAnimation.Play();
            }
            if (!Wave3AnimateAnimation.IsPlaying())
            {
                Wave3AnimateAnimation.Play();
            }
            if (!Wave4AnimateAnimation.IsPlaying())
            {
                Wave4AnimateAnimation.Play();
            }
            if (!TournamentFishInstance.FishSwimAnimation.IsPlaying())
            {
                TournamentFishInstance.FishSwimAnimation.Play();
            }
            if (!TournamentFishInstance1.FishSwimAnimation.IsPlaying())
            {
                TournamentFishInstance1.FishSwimAnimation.Play();
            }
            if (!TournamentFishInstance2.FishSwimAnimation.IsPlaying())
            {
                TournamentFishInstance2.FishSwimAnimation.Play();
            }
        }

        private void CustomDestroy()
        {


        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
    }
}
