using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Runtime.CompilerServices;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager instace;
    [SerializeField] private float globalShakeForce = 1f;
    [SerializeField] private CinemachineImpulseListener impulseListener;
    private CinemachineImpulseDefinition impulseDefinition;

    private void Awake() {
        if(instace == null){
            instace = this;
        }
    }

    public void CameraShake(CinemachineImpulseSource impulseSource){
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }
    public void ScreenShakeFromProfile(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource){
        //apply settings

        //screenshake
        impulseSource.GenerateImpulseWithForce(profile.impactForce);
    }
    private void SetupScreenShakeSettings(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource){
        impulseDefinition = impulseSource.m_ImpulseDefinition;

        //change impulse source settings
        impulseDefinition.m_ImpulseDuration = profile.impactTime;
        impulseSource.m_DefaultVelocity = profile.defaultVelocity;
        impulseDefinition.m_CustomImpulseShape = profile.impulseCurve;

        //change impulse listener settings
        impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
        impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
        impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;

    }
}
