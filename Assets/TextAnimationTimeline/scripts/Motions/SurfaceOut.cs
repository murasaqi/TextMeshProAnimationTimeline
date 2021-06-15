using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TextAnimationTimeline.Motions
{
    public class RandomFlutteringMoveIn : MonoBehaviour
    {
        private Vector3 _endPosition;
        private Vector3 _endEuler;
        private Vector3 _startPosition;
        private Vector3 _startEuler;
        private AnimationCurve _animationCurve;

        public void Init(Vector3 endPosition, Vector3 endEuler, Vector3 distance, Vector3 randomRotateRangeMin,Vector3 randomRotateRangeMax, AnimationCurve animationCurve)
        {
            _endPosition = endPosition;
            _endEuler = endEuler;
            _startPosition = endPosition + distance;
            _startEuler = endEuler
                + new Vector3(
                    Random.Range(randomRotateRangeMax.x, randomRotateRangeMin.x),
                    Random.Range(randomRotateRangeMax.y, randomRotateRangeMin.y),
                    Random.Range(randomRotateRangeMax.z, randomRotateRangeMin.z)
                );
            transform.localPosition = _startPosition;
            transform.localEulerAngles = _startEuler;
            _animationCurve = animationCurve;

        }

        public void OnProcess(float process)
        {
            transform.localPosition = Vector3.Lerp(_startPosition, _endPosition, _animationCurve.Evaluate(Mathf.Clamp(process,0f,1f)));
            transform.localEulerAngles = Vector3.Lerp(_startEuler, _endEuler, _animationCurve.Evaluate(Mathf.Clamp(process, 0f, 1f)));

        }
    }
    
    public class RandomFlutteringMoveOut : MonoBehaviour
    {
        private Vector3 _startPosition;
        private Vector3 _startEuler;
        private Vector3 _endPosition;
        private Vector3 _endEuler;
        private AnimationCurve _animationCurve;

        public void Init(Vector3 startPosition, Vector3 startEuler, Vector3 distance, Vector3 randomRotateRangeMin,Vector3 randomRotateRangeMax, AnimationCurve animationCurve)
        {
            _startPosition = startPosition;
            _startEuler = startEuler;
            _endPosition = _startPosition + distance;
            _endEuler = _startEuler
                + new Vector3(
                    Random.Range(randomRotateRangeMax.x, randomRotateRangeMin.x),
                    Random.Range(randomRotateRangeMax.y, randomRotateRangeMin.y),
                    Random.Range(randomRotateRangeMax.z, randomRotateRangeMin.z)
                );
            transform.localPosition = _startPosition;
            transform.localEulerAngles = _startEuler;
            _animationCurve = animationCurve;

        }

        public void OnProcess(float process)
        {
            transform.localPosition = Vector3.Lerp(_startPosition, _endPosition, _animationCurve.Evaluate(Mathf.Clamp(process,0f,1f)));
            transform.localEulerAngles = Vector3.Lerp(_startEuler, _endEuler, _animationCurve.Evaluate(Mathf.Clamp(process, 0f, 1f)));

        }
    }
    
    
    [ExecuteAlways]

    public class SurfaceOut : MotionTextElement
    {
        public bool DebugMode = false;
        public float DebugDuration = 2f;
        [Range(0,1)]public float DebugProcess = 0f;
        private List<RandomFlutteringMoveIn> _randomFlutteringMovesFadeIn = new List<RandomFlutteringMoveIn>();
        private List<RandomFlutteringMoveOut> _randomFlutteringMoveFadeOut = new List<RandomFlutteringMoveOut>();
        public override void Init(string word, double duration)
        {
            _randomFlutteringMovesFadeIn.Clear();
            _randomFlutteringMoveFadeOut.Clear();
            TextMeshElement = CreateTextMeshElement(word, Font, FontSize);
            TextMeshElement.MotionTextAlignmentOptions = MotionTextAlignmentOptions.MiddleCenter;
            TextMeshElement.alpha = 0f;

            var randomRangeMin = new Vector3(-40, -100, -0);
            var randomRangeMax = new Vector3(40, 100, 0);
            
            foreach (var child in TextMeshElement.Children)
            {
                var fadeInMotion = child.gameObject.AddComponent<RandomFlutteringMoveIn>();
                fadeInMotion.Init(
                        child.transform.localPosition,
                        child.transform.localEulerAngles,
                        new Vector3(0,-child.fontSize*0.4f,0),
                        randomRangeMin,
                        randomRangeMax,
                        animationCurveAsset.SteepIn
                );
                _randomFlutteringMovesFadeIn.Add(fadeInMotion);
                
                
                
                var fadeOutMotion = child.gameObject.AddComponent<RandomFlutteringMoveOut>();
                fadeOutMotion.Init(
                    child.transform.localPosition,
                    child.transform.localEulerAngles,
                    new Vector3(0,child.fontSize*0.4f,0),
                    randomRangeMin,
                    randomRangeMax,
                    animationCurveAsset.SteepOut
                );
                _randomFlutteringMoveFadeOut.Add(fadeOutMotion);
            }
            
        }
        
        public override void ProcessFrame(double normalizedTime, double seconds)
        {
            var fadeInDuration = 0.7f;
            var fadeOutDuration = 1f - fadeInDuration;
            
            
            var totalFadeInDelay = 0.3f*fadeInDuration;
            var totalDuration = fadeInDuration - totalFadeInDelay;
            var delayStep =totalFadeInDelay / (TextMeshElement.Children.Count - 1);
            var childDuration = totalDuration / (TextMeshElement.Children.Count - 1);
            var delay = 0f;
            var count = 0;
            foreach (var child in TextMeshElement.Children)
            {
                
                var inMotion = _randomFlutteringMovesFadeIn[count];
                var outMotion = _randomFlutteringMoveFadeOut[count];
                if (normalizedTime > delay)
                {
                    var process = Mathf.Clamp(((float)normalizedTime - delay) / childDuration, 0f, 1f);
                    inMotion.OnProcess(process);
                    child.alpha = process;
                }
                var fadeOutDelay = delay + childDuration;
                if (normalizedTime > fadeOutDelay)
                {
                    var process = Mathf.Clamp(((float)normalizedTime - fadeOutDelay) / fadeOutDuration, 0f, 1f);
                    outMotion.OnProcess(process);
                    child.alpha = (1f-process);
                }
                delay += delayStep;
                count++;
            }
            
           
        }

        private void Update()
        {
            if(DebugMode)ProcessFrame(DebugProcess,DebugDuration);
        }
    }
}