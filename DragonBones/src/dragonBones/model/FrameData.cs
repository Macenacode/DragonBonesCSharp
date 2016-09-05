﻿using System.Collections.Generic;
using System.Collections;

namespace dragonBones
{
    /**
 * @private
 */
    public class ActionData : BaseObject
    {
        public ActionType type;
        public BoneData bone;
        public SlotData slot;
        public ArrayList data = new ArrayList();

        public ActionData()
        {
        }

        protected override void _onClear()
        {
            type = ActionType.Play;
            bone = null;
            slot = null;
            data.Clear();
        }
    }

    /**
     * @private
     */
    public class EventData : BaseObject
    {

        public EventType type;
        public string name;
        //public any data; // TODO
        public BoneData bone;
        public SlotData slot;

        public EventData()
        {
        }

        protected override void _onClear()
        {
            type = EventType.Frame;
            name = null;
            //data = null;
            bone = null;
            slot = null;
        }
    }

    /**
     * @private
     */
    public abstract class FrameData<T> : BaseObject where T : FrameData<T>
    {
        public float position;
        public float duration;
        public T prev;
        public T next;

        public FrameData()
        {
        }

        /**
         * @inheritDoc
         */
        protected override void _onClear()
        {
            position = 0;
            duration = 0;
            prev = null;
            next = null;
        }
    }

    /**
     * @private
     */
    public abstract class TweenFrameData<T> : FrameData<T> where T : TweenFrameData<T>
    {
        public static List<float>samplingCurve(List<float> curve, uint frameCount)
        {
            if (curve.Count == 0 || frameCount == 0)
            {
                return null;
            }

            var samplingTimes = frameCount + 2;
            var samplingStep = 1.0f / samplingTimes;
            var sampling = new List<float>((int)(samplingTimes - 1) * 2);

            //
            //curve = curve.concat();
            //curve.unshift(0, 0);
            //curve.push(1, 1);

            var stepIndex = 0;
            for (var i = 0; i<samplingTimes - 1; ++i) {
                var step = samplingStep * (i + 1);
                while (curve[stepIndex + 6] < step) // stepIndex + 3 * 2
                {
                    stepIndex += 6; // stepIndex += 3 * 2
                }

                var x1 = curve[stepIndex];
                var x4 = curve[stepIndex + 6];

                var t = (step - x1) / (x4 - x1);
                var l_t = 1 - t;

                var powA = l_t * l_t;
                var powB = t * t;

                var kA = l_t * powA;
                var kB = 3 * t * powA;
                var kC = 3 * l_t * powB;
                var kD = t * powB;

                sampling[i * 2] = kA* x1 + kB* curve[stepIndex + 2] + kC* curve[stepIndex + 4] + kD* x4;
                sampling[i * 2 + 1] = kA* curve[stepIndex + 1] + kB* curve[stepIndex + 3] + kC* curve[stepIndex + 5] + kD* curve[stepIndex + 7];
            }

            return sampling;
        }

        public float tweenEasing;
        public List<float> curve;

        public TweenFrameData()
        {
        }

        /**
         * @inheritDoc
         */
        protected override void _onClear()
        {
            base._onClear();

            tweenEasing = 0.0f;
            curve = null;
        }
    }

    /**
     * @private
     */
    public class AnimationFrameData : FrameData<AnimationFrameData>
    {
        public List<ActionData> actions = new List<ActionData>();
        public List<EventData> events = new List<EventData>();

        public AnimationFrameData()
        {
        }

        /**
         * @inheritDoc
         */
        protected override void _onClear()
        {
            base._onClear();

            foreach (var action in actions)
            {
                action.returnToPool();
            }

            foreach (var evt in events)
            {
                evt.returnToPool();
            }

            actions.Clear();
            events.Clear();
        }
    }

    /**
     * @private
     */
    public class BoneFrameData : TweenFrameData<BoneFrameData>
    {
        public bool tweenScale;
        public float tweenRotate;
        public Transform transform = new Transform();

        public BoneFrameData()
        {
        }

        /**
         * @inheritDoc
         */
        protected override void _onClear()
        {
            base._onClear();

            tweenScale = false;
            tweenRotate = 0.0f;
            transform.identity();
        }
    }

    /**
     * @private
     */
    public class SlotFrameData : TweenFrameData<SlotFrameData>
    {
        public static ColorTransform DEFAULT_COLOR = new ColorTransform();
        public static ColorTransform generateColor()
        {
            return new ColorTransform();
        }

        public int displayIndex;
        public int zOrder;
        public ColorTransform color;

        public SlotFrameData()
        {
        }

        /**
         * @inheritDoc
         */
        protected override void _onClear()
        {
            base._onClear();

            displayIndex = 0;
            zOrder = 0;
            color = null;
        }
    }

    /**
     * @private
     */
    public class ExtensionFrameData : TweenFrameData<ExtensionFrameData>
    {
        public ExtensionType type;
        public List<float> tweens = new List<float>();
        public List<float> keys = new List<float>();

        public ExtensionFrameData()
        {
        }
        /**
         * @inheritDoc
         */
        protected override void _onClear()
        {
            base._onClear();

            type = ExtensionType.FFD;
            tweens.Clear();
            keys.Clear();
        }
    }
}
