using System.Collections;
using Code.Core;
using Code.Managers;
using Code.Scenes.Initializer;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace Code.Sounds
{
    public class SoundManager : MonoBehaviour, IOnceManager, ICombatStageBehavior, ISceneInit
    {
        public int Priority => 0;
        public void OnSceneInit()
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                if (IsCombatStage == false)
                    PlaySound(initBgm.soundName);
                else
                    PlaySound(combatBgm.soundName);
            });
        }

        public void OnSceneExit()
        {
        }

        public bool IsCombatStage { get; set; }

        [SerializeField] private SoundListSO soundListSo;
        [SerializeField] private SoundSO initBgm;
        [SerializeField] private SoundSO combatBgm;
        [SerializeField] private AudioMixer mixer;

        public void Initialize()
        {
        }

        public void PlaySound(string soundName)
        {
            print(soundName);
            GameObject obj = new GameObject();
            obj.name = soundName + " Sound";
            AudioSource source = obj.AddComponent<AudioSource>();
            SoundSO so = soundListSo.SoundsDictionary[soundName];
            if (so.soundType == SoundType.SFX)
                source.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            else if (so.soundType == SoundType.BGM)
            {
                source.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
            }
            else
            {
                Debug.LogWarning("Type이 없습니다");
                source.outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[0];
            }

            SetAudio(source, so);
        }

        private void SetAudio(AudioSource source, SoundSO sounds)
        {
            source.clip = sounds.clip;
            source.loop = sounds.loop;
            source.priority = sounds.Priority;
            source.volume = sounds.volume;
            source.pitch = sounds.pitch;
            source.panStereo = sounds.stereoPan;
            source.spatialBlend = sounds.SpatialBlend;
            source.Play();
            if (!sounds.loop)
            {
                StartCoroutine(DestroyCo(source.clip.length, source.gameObject));
            }
        }

        IEnumerator DestroyCo(float endTime, GameObject obj)
        {
            yield return new WaitForSecondsRealtime(endTime);
            Destroy(obj);
        }
    }

    public enum SoundType
    {
        BGM,
        SFX
    }
}