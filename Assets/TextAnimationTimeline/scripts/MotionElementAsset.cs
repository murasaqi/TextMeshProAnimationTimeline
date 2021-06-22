using UnityEngine;
using UnityEditor;
using TMPro;
namespace TextAnimationTimeline
{
   
    [CreateAssetMenu(menuName = "TextAnimationTimeline/Create MotionElementAsset Instance")]
    public class MotionElementAsset:ScriptableObject
    {
        public GameObject CurveFlowPlane00;
        public GameObject CurveFlowPlane01;
        public GameObject CurveFlowPlane02;
        public GameObject CurveFlowPlane03;
        public GameObject FlyOffPaper00;
        public GameObject FlyOffPaper01;
        public GameObject FlyOffPaper02;
        

    }
}