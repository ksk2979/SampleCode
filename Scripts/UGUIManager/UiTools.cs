using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UiTools
    {
        public static UnityEvent ConvertUnityEvent(UnityAction action)
        {
            var uEvent = new UnityEvent();
            uEvent.AddListener(action);
            return uEvent;
        }

        public static EventTrigger.Entry GetEventTrigerEntry(EventTriggerType trigerType,
            UnityAction<BaseEventData> act)
        {
            var release = new EventTrigger.Entry {eventID = trigerType};
            release.callback.AddListener(act);
            return release;
        }


        public static EventTrigger.Entry GetEventTrigerEntry(EventTriggerType trigerType, UnityAction act)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { act(); });
            return entry;
        }


        public static Bounds GetBound(Transform trnas, ref Vector2 center)
        {
            var bounds = GetBound(trnas);
            center = bounds.center;
            return bounds;
        }

        public static Bounds GetBound(Transform trnas)
        {
            var bounds = new Bounds
            {
                min = new Vector2(9999999, 9999999),
                max = new Vector2(-9999999, -9999999)
            };


            //_bounds.center = trnas_.localPosition;

            Debug.Log(trnas.name);
            var grapics = trnas.GetComponentsInChildren<MaskableGraphic>();
            for (var i = 0; i < grapics.Length; i++)
            {
                SetBounds(grapics[i], trnas, ref bounds);
            }

            return bounds;
        }


        private static void SetBounds(MaskableGraphic gp, Transform trnas, ref Bounds bounds)
        {
            var rt = gp.transform as RectTransform;

            var tarGetbounds = GetMyOffSet(rt, trnas);

            if (tarGetbounds.min.x < bounds.min.x)
                bounds.min = new Vector3(tarGetbounds.min.x, bounds.min.y, 0);
            if (tarGetbounds.min.y < bounds.min.y)
                bounds.min = new Vector3(bounds.min.x, tarGetbounds.min.y, 0);
            if (bounds.max.x < tarGetbounds.max.x)
                bounds.max = new Vector3(tarGetbounds.max.x, bounds.max.y, 0);
            if (bounds.max.y < tarGetbounds.max.y)
                bounds.max = new Vector3(bounds.max.x, tarGetbounds.max.y, 0);
        }

        private static Bounds GetMyOffSet(RectTransform rt, Transform trnas)
        {
            var bounds = new Bounds();

            var localPos = trnas.InverseTransformPoint(rt.position) + trnas.localPosition;
            var deltaSize = rt.sizeDelta;

            var sizeWidthHalf = deltaSize.x*0.5f;
            var sizeHeightHalf = deltaSize.y*0.5f;

            bounds.min = new Vector3(localPos.x - sizeWidthHalf, localPos.y - sizeHeightHalf, 0);
            bounds.max = new Vector3(localPos.x + sizeWidthHalf, localPos.y + sizeHeightHalf, 0);

            return bounds;
        }


        public static Vector3 GetLocalPos(RectTransform targetRt, Transform topParrent)
        {
            var widgetWorldPos = targetRt.parent.TransformPoint(targetRt.localPosition);
            return topParrent.InverseTransformPoint(widgetWorldPos);
        }

    }
}