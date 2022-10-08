using UnityEngine;

namespace PolyternityStuff.Utils
{
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Returns true if any animation is playing.
        /// </summary>
        public static bool IsPlaying(this Animator animator)
        {
            return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
        }
    
        /// <summary>
        /// Returns true if animation with a given name is playing.
        /// </summary>
        public static bool IsPlaying(this Animator animator, string animationName)
        {
            return animator.IsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        }
    
        /// <summary>
        /// Returns true if animation with a given hash is playing.
        /// </summary>
        public static bool IsPlaying(this Animator animator, int animationHash)
        {
            return animator.IsPlaying() && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == animationHash;
        }
    }
}