using UnityEngine;

public class BlendTreeController : MonoBehaviour
{
    public void BlendAnimations(PlayerInputHandler player, float inputX, float inputY)
    {
        player.Controller.Animator.SetFloat("horizontal", inputX);
        player.Controller.Animator.SetFloat("vertical", inputY);
    }
}
