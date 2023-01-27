using System;
using DG.Tweening;
using UnityEngine;

namespace Dragging
{
  public class DragSystem : MonoBehaviour
  {
    private DragObject dragObject { get; set; }
    private Vector3    mousePositionOffset { get; set; }

    private Camera mainCamera => Camera.main ? Camera.main : throw new NullReferenceException("No main camera found!");

    private void Update()
    {
      if (Input.GetMouseButtonDown(0))
      {
        Debug.Log("Mouse down");
        if (!dragObject)
        {
          if (castRay(out RaycastHit raycast_hit))
          {
            dragObject = raycast_hit.collider.gameObject.GetComponent<DragObject>();
            if (!dragObject)
              return;

            mousePositionOffset = raycast_hit.point - raycast_hit.collider.gameObject.transform.position;
          }
        }
      }

      if (Input.GetMouseButtonUp(0))
        dragObject = null;

      updateObjectPosition();
    }

    private bool castRay( out RaycastHit raycast_hit )
    {
      Camera main_camera = mainCamera;

      Vector3 screen_mouse_pos_far = new Vector3(Input.mousePosition.x, Input.mousePosition.y, main_camera.farClipPlane);
      Vector3 screen_mouse_pos_near = new Vector3(Input.mousePosition.x, Input.mousePosition.y, main_camera.nearClipPlane);

      Vector3 world_mouse_pos_far  = main_camera.ScreenToWorldPoint(screen_mouse_pos_far);
      Vector3 world_mouse_pos_near = main_camera.ScreenToWorldPoint(screen_mouse_pos_near);

      return Physics.Raycast(world_mouse_pos_near, world_mouse_pos_far - world_mouse_pos_near, out raycast_hit);
    }

    private void updateObjectPosition()
    {
      if (!dragObject)
        return;

      Camera main_camera = mainCamera;
      Vector3 mouse_position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, main_camera.WorldToScreenPoint(dragObject.transform.position).z);
      Vector3 world_position = main_camera.ScreenToWorldPoint(mouse_position);
      world_position -= mousePositionOffset;
      world_position.y = 0.5f;

      dragObject.transform.DOMove(world_position, -1.0f);// TODO: move y position to constants
    }
  }
}