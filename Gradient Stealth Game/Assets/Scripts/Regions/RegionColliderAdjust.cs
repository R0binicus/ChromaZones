using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionColliderAdjust : MonoBehaviour
{
    [SerializeField]
    private BoxCollider2D platformCollider;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

      private void Start()
      {
          platformCollider.size 
              = new Vector2(spriteRenderer.size.x, platformCollider.size.y);
      }
}
