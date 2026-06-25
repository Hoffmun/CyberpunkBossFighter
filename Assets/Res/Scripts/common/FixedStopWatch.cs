using System;
using UnityEngine;

[Serializable]
public class FixedStopwatch
{
  public bool IsFinished => Elapsed > duration;
  public bool IsReady => Elapsed > cooldown;
  public float Completion => Mathf.Clamp01(Elapsed / duration);

  float _timestamp;
  [SerializeField] float duration;
  [SerializeField] float cooldown;
  float Elapsed => Time.fixedTime - _timestamp;

  public void Reset()
  {
    _timestamp = Time.fixedTime - cooldown - duration - 1;
  }

  public void Split()
  {
    _timestamp = Time.fixedTime;
  }
}