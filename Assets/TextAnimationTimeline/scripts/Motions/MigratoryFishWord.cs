using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;
using Random = UnityEngine.Random;
namespace TextAnimationTimeline.Motions
{

    [ExecuteAlways]

    public class MigratoryFishWord : MotionTextElement
    {
        public bool DebugMode = false;
        public float DebugDuration = 2f;
        [Range(0,1)]public float DebugProcess = 0f;
        private List<Material> _characterMaterials = new List<Material>();
        private List<AlembicStreamPlayer> _characterAlembics = new List<AlembicStreamPlayer>();
        private List<GameObject> _characters = new List<GameObject>();
        private float _wavePower = 14;
        private List<GameObject> _alembicPrefabs = new List<GameObject>();
        private void OnEnable()
        {
            // Init("A",1);
        }

        public override void Init(string word, double duration)
        {
            
            _alembicPrefabs.Add(textAnimationManager.MotionElementAsset.CurveFlowPlane00);
            _alembicPrefabs.Add(textAnimationManager.MotionElementAsset.CurveFlowPlane01);
            _alembicPrefabs.Add(textAnimationManager.MotionElementAsset.CurveFlowPlane02);
            _alembicPrefabs.Add(textAnimationManager.MotionElementAsset.CurveFlowPlane03);
            TextMeshElement = CreateTextMeshElement(word, Font, FontSize);
            TextMeshElement.MotionTextAlignmentOptions = MotionTextAlignmentOptions.MiddleCenter;
            TextMeshElement.alpha = 1f;
            _characterAlembics.Clear();
            _characters.Clear();
            
            var scale = 2;
            var width = 2;
            var x = scale/2f*width;

            var offsetY = -10f;
            foreach (var tmPro in TextMeshElement.Children)
            {

                int num = Random.Range(0, 4);
                // string name =  $"TextPrefab/CurveFlowPlane0{num}";
                // Debug.Log(name);
                tmPro.color = Color.white;
                
                
                var tex = Graphics.TMProToTex2D(tmPro, 1000, textAnimationManager.CaptureCamera);
                var obj = Instantiate(_alembicPrefabs[Random.Range(0,_alembicPrefabs.Count)]);
               
                var renderer = obj.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
                renderer.sharedMaterial = new Material(renderer.sharedMaterial.shader);
                renderer.sharedMaterial.SetTexture("_Texture2D",tex);
                renderer.sharedMaterial.SetFloat("_Alpha",0f);
                obj.transform.SetParent(transform);
                
                obj.transform.localEulerAngles = new Vector3(0, -180, 0);
                obj.transform.localScale = new Vector3(scale, scale, scale);
                obj.transform.localPosition = new Vector3(x, offsetY, 0);
                _characters.Add(obj);
                _characterAlembics.Add(obj.GetComponent<AlembicStreamPlayer>());
                _characterMaterials.Add(renderer.sharedMaterial);
                x += scale*width;
                tmPro.alpha = 0f;
            }


            foreach (var obj in _characters)
            {
                obj.transform.localPosition -= new Vector3(_characters.Count * width * scale / 2f, 0f, 0f);
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
         
            var count = 0;
            var totalDelay = 0.2f;
            var fadeOutDuration = 0.4f;
            var totalFadeinDuration = 1f - fadeOutDuration;
            var delayStep = totalDelay/(_characters.Count-1);
            // var characterDuration = (1f - totalDelay)/_characters.Count;
            var delay = 0f;
            foreach (var alembic in _characterAlembics)
            {
                var characterDuration = (totalFadeinDuration - delay);
                var mat = _characterMaterials[count];
                // if(normalizedTime <delay)mat.SetFloat("_Alpha",0f);
                if (delay<=normalizedTime && normalizedTime < 1f-fadeOutDuration)
                {
                    var progress = Mathf.Clamp(((float) normalizedTime - delay) /characterDuration, 0f, 1f);
                    progress *= 0.5f;
                    alembic.CurrentTime = animationCurveAsset.MigrateWave.Evaluate(progress)* alembic.Duration;
                    mat.SetFloat("_Alpha",animationCurveAsset.MigrateAlpha.Evaluate(progress));
                    mat.SetFloat("_WavePower", (animationCurveAsset.MigrateWavePower.Evaluate(progress))*_wavePower);
                }
                if (normalizedTime >= 1f-fadeOutDuration)
                {
                    
                    var progress =0.5f + 0.5f* Mathf.Clamp(((float) normalizedTime - totalFadeinDuration  ) / fadeOutDuration, 0f, 1f);
                    alembic.CurrentTime = animationCurveAsset.MigrateWave.Evaluate(progress)* alembic.Duration;
                    _characterMaterials[count].SetFloat("_Alpha",animationCurveAsset.MigrateAlpha.Evaluate(progress));
                    mat.SetFloat("_WavePower", (animationCurveAsset.MigrateWavePower.Evaluate(progress))*_wavePower);
                }
            
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