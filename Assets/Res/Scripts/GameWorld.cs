using UnityEngine;

public class GameWorld : MonoBehaviour
{
  [SerializeField] InputPipeline input_pipeline;
  [SerializeField] PlayerController player_controller;

  void Awake()
  {
    
  }

  void Start()
  {
    
  }

  void Update()
  {
    
  }

  void FixedUpdate()
  {
    var input_config = input_pipeline.GetPlayerInput();
    player_controller.UpdateInput(input_config);
    player_controller.Process();
  }
}
