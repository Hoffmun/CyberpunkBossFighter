using UnityEngine;
using UnityEngine.InputSystem;

public class InputPipeline : MonoBehaviour
{
  [SerializeField] InputActionReference player_movement_ref;

  void Start()
  {
    
  }

  public InputProvider GetPlayerInput()
  {
    InputProvider config = new()
    {
      desired_direction = player_movement_ref.action.ReadValue<Vector2>()
    };
    return config;
  }
}

public struct InputProvider
{
  public Vector2 desired_direction;
}