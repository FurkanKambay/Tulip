using System;
using FK.Common;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace FK.Tulip.Audio
{
    [Serializable]
    public struct FMODEvent
    {
        [SerializeField] private EventReference reference;

        public EventReference Reference => reference;
        public EventDescription Description => description;
        public EventInstance Instance => instance;

        public bool IsValid => description.isValid();

        private EventDescription description;
        private EventInstance instance;

        public void Describe() =>
            description = RuntimeManager.GetEventDescription(reference);

        public bool DescribeParameter(string parameterName, out PARAMETER_DESCRIPTION parameterDescription) =>
            description.getParameterDescriptionByName(parameterName, out parameterDescription) == RESULT.OK;

        public bool StartNewInstance() => StartNew(out instance);
        public bool CreateNewInstance() => CreateNew(out instance);

        public bool StartInstance() => instance.start() == RESULT.OK;
        public bool SetInstancePaused(bool paused) => instance.setPaused(paused) == RESULT.OK;
        public bool StopInstance() => instance.stop(STOP_MODE.ALLOWFADEOUT) == RESULT.OK;
        public bool ForceStopInstance() => instance.stop(STOP_MODE.IMMEDIATE) == RESULT.OK;

        public bool PlayOneShot()
        {
            bool started = StartNew(out EventInstance oneShotInstance);
            return started && oneShotInstance.release() == RESULT.OK;
        }

        public bool StartNew(out EventInstance eventInstance) =>
            CreateNew(out eventInstance) && eventInstance.start() == RESULT.OK;

        public bool CreateNew(out EventInstance eventInstance)
        {
            if (!IsValid)
            {
                WarnInvalidDescription();
                eventInstance = default;
                return false;
            }

            RESULT result = description.createInstance(out eventInstance);
            return result == RESULT.OK;
        }

        private void WarnInvalidDescription()
        {
#if UNITY_EDITOR
            Log.Warning($"Invalid FMOD event description: {reference.Path} ({reference.Guid})");
#else
            Log.Warning($"Invalid FMOD event description: {reference.Guid}");
#endif
        }
    }
}
