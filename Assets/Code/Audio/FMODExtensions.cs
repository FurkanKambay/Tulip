using System;
using FK.Common.Extensions;
using FMOD;
using FMOD.Studio;
using JetBrains.Annotations;

namespace FK.Tulip.Audio
{
    [PublicAPI]
    public static class FMODExtensions
    {
        public static bool PlayOneShot(this EventInstance @event) =>
            @event.start() == RESULT.OK && @event.release() == RESULT.OK;

        public static bool SetParameter(this EventInstance @event, in PARAMETER_DESCRIPTION parameter, float value) =>
            @event.setParameterByID(parameter.id, value) == RESULT.OK;

        public static bool SetParameter(this EventInstance @event, in PARAMETER_DESCRIPTION parameter, int value) =>
            @event.setParameterByID(parameter.id, value) == RESULT.OK;

        public static bool SetParameter(this EventInstance @event, in PARAMETER_DESCRIPTION parameter, bool value) =>
            @event.setParameterByID(parameter.id, value ? 1f : 0f) == RESULT.OK;

        public static bool SetParameter<T>(this EventInstance @event, in PARAMETER_DESCRIPTION parameter, T value)
            where T : struct, Enum =>
            @event.setParameterByID(parameter.id, value.AsLong()) == RESULT.OK;
    }
}
