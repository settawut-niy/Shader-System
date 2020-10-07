using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderStory : MonoBehaviour
{
    Renderer myRenderer;
    RaycastHit hit;

    bool isShowCircle = false;
    bool isWait = false;
    [SerializeField] float waitTime = 2f;

    [Range(0, 1)] float t = 0;
    [Range(0, 1)] float t2 = 0;
    [SerializeField][Range(0, 1)] float speed = 0.5f;
    [SerializeField] float circleSize = 0.5f;
    [SerializeField] float circleSizeLimit = 20f;

    enum StoryState
    {
        IntroState,
        ShowState,
        EndState,
        WaitState
    }

    StoryState currentStoryState;

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

        switch (currentStoryState)
        {
            case StoryState.IntroState:
                IntroStage();
                break;
            case StoryState.ShowState:
                ShowStage();
                break;
            case StoryState.EndState:
                EndState();
                break;
            case StoryState.WaitState:
                StartCoroutine(WaitTime(waitTime));
                break;
        }
       
    }

    void IntroStage ()
    {
        if (isWait){ return; }

        if (myRenderer.material.GetFloat("_WaveRange") < 5.2f)
        {
            t += Time.deltaTime;
            myRenderer.material.SetFloat("_WaveRange", Mathf.Lerp(-5.2f, 5.2f, t*speed));
        }
        else if (myRenderer.material.GetFloat("_WaveRange") >= 5.2f && myRenderer.material.GetColor("_ExternalVoronoiColor") == Color.black)
        {
            currentStoryState = StoryState.ShowState;
            myRenderer.material.SetFloat("_CircleSize", 0f);
            t = 0;
            t2 = 0;
        }

        if (myRenderer.material.GetFloat("_WaveRange") >= 0 && myRenderer.material.GetColor("_ExternalVoronoiColor") != Color.black)
        {
            t2 += Time.deltaTime;
            myRenderer.material.SetColor("_ExternalVoronoiColor", Color.Lerp(Color.white, Color.black, t2 * speed));
        }
    }

    void ShowStage()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!isShowCircle)
            {
                isShowCircle = true;
                myRenderer.material.SetColor("_ExternalVoronoiColor", Color.white);
            }
            else
            {
                currentStoryState = StoryState.EndState;
                isShowCircle = false;
                t = 0;
            }
        }

        if (isShowCircle)
        {
            if (t >= 1) { return; }
            t += Time.deltaTime;
            myRenderer.material.SetFloat("_CircleSize", (Mathf.Lerp(0f, circleSize, t)));
        }
    }

    void EndState ()
    {
        if (myRenderer.material.GetColor("_InternalVoronoiColor") != Color.white || myRenderer.material.GetFloat("_CircleSize") < circleSizeLimit)
        {
            t += Time.deltaTime;
            myRenderer.material.SetColor("_InternalVoronoiColor", Color.Lerp(Color.black, Color.white, t*speed));
            myRenderer.material.SetFloat("_CircleSize", (Mathf.Lerp(circleSize, circleSizeLimit, t*speed)));
        }
        else if (myRenderer.material.GetColor("_InternalVoronoiColor") == Color.white || myRenderer.material.GetFloat("_CircleSize") >= circleSizeLimit)
        {
            currentStoryState = StoryState.WaitState;
            myRenderer.material.SetFloat("_WaveRange", -5.2f);
            myRenderer.material.SetColor("_InternalVoronoiColor", Color.black);
            t = 0;
        }
    }

    IEnumerator WaitTime (float time)
    {
        isWait = true;
        yield return new WaitForSeconds(time);
        isWait = false;
        currentStoryState = StoryState.IntroState;
    }
}
