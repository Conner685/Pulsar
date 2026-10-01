using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private InputReader inputReader;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        inputReader.ShipMoveEvent += HandleShipMove;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnDestroy()
    {
        inputReader.ShipMoveEvent -= HandleShipMove;
    }

    private void HandleShipMove(Vector2 obj)
    {
        Debug.Log("Handling ship move!");
    }
}