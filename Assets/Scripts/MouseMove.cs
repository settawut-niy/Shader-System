using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    Renderer myRenderer;
    RaycastHit hit;

    bool isShowCircle = false;


    void Awake()
    {
        myRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            myRenderer.material.SetVector("_CirclePosition", new Vector4(hit.point.x, 0, hit.point.z, 0));
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isShowCircle)
            {
                isShowCircle = true;
                myRenderer.material.SetFloat("_IsShowCircle", isShowCircle ? 1f : 0f);
            }
            else
            {
                isShowCircle = false;
                myRenderer.material.SetFloat("_IsShowCircle", isShowCircle ? 1f : 0f);
            }

        }
    }
}
