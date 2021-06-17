using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TextAnimationTimeline.Motions
{

    [ExecuteAlways]

    public class MigratoryFishWord : MotionTextElement
    {
        public bool DebugMode = false;
        public float DebugDuration = 2f;
        [Range(0,1)]public float DebugProcess = 0f;
        private List<Texture2D> _characterTextures = new List<Texture2D>();
        private List<GameObject> _characterObjects = new List<GameObject>();

        private void OnEnable()
        {
            // Init("A",1);
        }

        public override void Init(string word, double duration)
        {
            TextMeshElement = CreateTextMeshElement(word, Font, FontSize);
            TextMeshElement.MotionTextAlignmentOptions = MotionTextAlignmentOptions.MiddleCenter;
            TextMeshElement.alpha = 1f;

            foreach (var tmPro in TextMeshElement.Children)
            {
   
                var tex = Graphics.TMProToTex2D(tmPro, 1000, textAnimationManager.CaptureCamera);
                var obj = GameObject.Instantiate(Resources.Load<GameObject>("TextPrefab/CurveFlowPlane00"));
                var renderer = obj.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
                renderer.sharedMaterial = new Material(Resources.Load<Shader>("Materials/WaterWaveShader"));
                renderer.sharedMaterial.SetTexture("_Texture2D",tex);
                obj.transform.SetParent(transform);
                tmPro.alpha = 0f;
            }
    
            
            // var randomRangeMin = new Vector3(-40, -100, -0);
            // var randomRangeMax = new Vector3(40, 100, 0);
            //
            // foreach (var child in TextMeshElement.Children)
            // {
            //     var fadeInMotion = child.gameObject.AddComponent<RandomFlutteringMoveIn>();
            //     fadeInMotion.Init(
            //             child.transform.localPosition,
            //             child.transform.localEulerAngles,
            //             new Vector3(0,-child.fontSize*0.4f,0),
            //             randomRangeMin,
            //             randomRangeMax,
            //             animationCurveAsset.SteepIn
            //     );
            //     // _randomFlutteringMovesFadeIn.Add(fadeInMotion);
            //     
            //     
            //     
            //     var fadeOutMotion = child.gameObject.AddComponent<RandomFlutteringMoveOut>();
            //     fadeOutMotion.Init(
            //         child.transform.localPosition,
            //         child.transform.localEulerAngles,
            //         new Vector3(0,child.fontSize*0.4f,0),
            //         randomRangeMin,
            //         randomRangeMax,
            //         animationCurveAsset.SteepOut
            //     );
            //     _randomFlutteringMoveFadeOut.Add(fadeOutMotion);
            // }
            
        }
        
        public override void ProcessFrame(double normalizedTime, double seconds)
        {
            // var fadeInDuration = 0.7f;
            // var fadeOutDuration = 1f - fadeInDuration;
            //
            //
            // var totalFadeInDelay = 0.3f*fadeInDuration;
            // var totalDuration = fadeInDuration - totalFadeInDelay;
            // var delayStep =totalFadeInDelay / (TextMeshElement.Children.Count - 1);
            // var childDuration = totalDuration / (TextMeshElement.Children.Count - 1);
            // var delay = 0f;
            // var count = 0;
            // foreach (var child in TextMeshElement.Children)
            // {
            //     
            //     var inMotion = _randomFlutteringMovesFadeIn[count];
            //     var outMotion = _randomFlutteringMoveFadeOut[count];
            //     if (normalizedTime > delay)
            //     {
            //         var process = Mathf.Clamp(((float)normalizedTime - delay) / childDuration, 0f, 1f);
            //         inMotion.OnProcess(process);
            //         child.alpha = process;
            //     }
            //     var fadeOutDelay = delay + childDuration;
            //     if (normalizedTime > fadeOutDelay)
            //     {
            //         var process = Mathf.Clamp(((float)normalizedTime - fadeOutDelay) / fadeOutDuration, 0f, 1f);
            //         outMotion.OnProcess(process);
            //         child.alpha = (1f-process);
            //     }
            //     delay += delayStep;
            //     count++;
            // }
            
           
        }

        private void Update()
        {
            if(DebugMode)ProcessFrame(DebugProcess,DebugDuration);
        }
    }
}