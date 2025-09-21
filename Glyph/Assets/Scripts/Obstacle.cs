using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour, IInteractable
{

    private Rigidbody2D rb;
    private FixedJoint2D joint;
    private bool isGrabbed = false;

    private GameObject player;
    private PlayerController playerController;

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        if (!CanInteract())
            return;

        if (!isGrabbed)
            Grab();
        else
            Release();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    void Grab()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        joint = gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = player.GetComponent<Rigidbody2D>();
        joint.enableCollision = false;
        isGrabbed = true;

        playerController.StateMachine.ChangeState(playerController.CrouchState);
        playerController.StateMachine.BlockState(playerController.CrouchState);
    }

    void Release()
    {
        if (joint != null)
        {
            Destroy(joint);
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.ResetVelocity();
        isGrabbed = false;

        playerController.StateMachine.UnBlockAnyBlockedState();
    }

    public bool IsGrabbed()
    {
        return isGrabbed;
    }

    public string GetDisplayName()
    {
        throw new System.NotImplementedException();
    }

    public bool Interact(GameObject interactor)
    {
        throw new System.NotImplementedException();
    }
}
