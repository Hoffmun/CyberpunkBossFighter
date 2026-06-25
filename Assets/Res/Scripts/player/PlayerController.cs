using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
  Rigidbody2D rb;

  [Header("Walking")] 
  [SerializeField] float maxWalkCos = 0.5f;
  [SerializeField] float WalkSpeed = 15;

  [Header("Jumping")] 
  [SerializeField] int JumpsCount = 1;
  [SerializeField] float JumpSpeed;
  [SerializeField] AnimationCurve JumpSpeedCurve = AnimationCurve.Linear(0, 1, 1, 0);
  [SerializeField] FixedStopwatch JumpStopwatch = new ();
  bool Is_jumping => !JumpStopwatch.IsFinished;
  float Jump_completion => JumpStopwatch.Completion;
  int jumps_left = 0;
  bool was_on_the_ground = false;

  [Header("Falling")] 
  [SerializeField] float FallSpeed;

  [SerializeField] ContactFilter2D _contactFilter;
  ContactPoint2D? _groundContact;
  ContactPoint2D? _ceilingContact;
  ContactPoint2D? _wallContact;
  readonly ContactPoint2D[] _contacts = new ContactPoint2D[16];

  float desired_direction = 0;
  bool wants_to_jump = false;

  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();

    jumps_left = JumpsCount;
  }

  public void UpdateInput(InputProvider config)
  {
    desired_direction = config.desired_direction.x;
    bool now_wants_to_jump = config.desired_direction.y > 0.5f;

    if (now_wants_to_jump != wants_to_jump)
    {
      wants_to_jump = now_wants_to_jump;
      UpdateJumpState();
    }
  }

  void UpdateJumpState()
  {
    if (wants_to_jump)
    {
      if (jumps_left <= 0) return;

      jumps_left--;
      JumpStopwatch.Split();
    }
    else
    {
      JumpStopwatch.Reset();
    }
  }

  void FindContacts()
  {
    _groundContact = null;
    _ceilingContact = null;
    _wallContact = null;

    float groundProjection = maxWalkCos;
    float wallProjection = maxWalkCos;
    float ceilingProjection = -maxWalkCos;

    int numberOfContacts = rb.GetContacts(_contactFilter, _contacts);
    for (var i = 0; i < numberOfContacts; i++)
    {
      var contact = _contacts[i];
      float projection = Vector2.Dot(Vector2.up, contact.normal);

      if (projection > groundProjection)
      {
        _groundContact = contact;
        groundProjection = projection;
      }
      else if (projection < ceilingProjection)
      {
        _ceilingContact = contact;
        ceilingProjection = projection;
      }
      else if (projection <= wallProjection)
      {
        _wallContact = contact;
        wallProjection = projection;
      }
    }
  }

  public void Process()
  {
    FindContacts();

    Vector2 previous_velocity = rb.linearVelocity;
    Vector2 velocity_change = Vector2.zero;

    if (wants_to_jump && Is_jumping)
    {
      was_on_the_ground = false;
      float current_jump_speed = JumpSpeed * JumpSpeedCurve.Evaluate(Jump_completion);
      velocity_change.y = current_jump_speed - previous_velocity.y;

      if (_ceilingContact.HasValue)
      {
        JumpStopwatch.Reset();
      }
    }
    else if (_groundContact.HasValue)
    {
      jumps_left = JumpsCount;
      was_on_the_ground = true;
    }
    else
    {
      if (was_on_the_ground)
      {
        was_on_the_ground = false;
        jumps_left--;
      }

      velocity_change.y = (-FallSpeed - previous_velocity.y) * 0.125f;
    }

    velocity_change.x = (desired_direction * WalkSpeed - previous_velocity.x) * 0.25f;
    if (_wallContact.HasValue)
    {
      var wall_direction = (int) Mathf.Sign(_wallContact.Value.point.x - transform.position.x);
      var walk_direction = (int) Mathf.Sign(desired_direction);

      if (walk_direction == wall_direction) velocity_change.x = 0;
    }

    rb.AddForce(velocity_change, ForceMode2D.Impulse);
  }
}
