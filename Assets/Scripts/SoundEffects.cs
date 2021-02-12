using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public AudioSource audio;
    public AudioClip footstep1;
    public AudioClip footstep2;
    public AudioClip footstep3;
    public AudioClip footstep4;
    public AudioClip jump1;
    public AudioClip jump2;
    public AudioClip throw1;
    public AudioClip throw2;
    public AudioClip enemyDestroy;
    public AudioClip packageDelivered;
    public AudioClip outOfTime;
    public AudioClip levelFinish;

    public void PlaySound(string sound)
    {
        switch(sound)
        {
            case "Footstep1":
                audio.PlayOneShot(footstep1);
                break;
            case "Footstep2":
                audio.PlayOneShot(footstep2);
                break;
            case "Footstep3":
                audio.PlayOneShot(footstep3);
                break;
            case "Footstep4":
                audio.PlayOneShot(footstep4);
                break;
            case "Jump1":
                audio.PlayOneShot(jump1);
                break;
            case "Jump2":
                audio.PlayOneShot(jump2);
                break;
            case "Throw1":
                audio.PlayOneShot(throw1);
                break;
            case "Throw2":
                audio.PlayOneShot(throw2);
                break;
            case "EnemyDestroy":
                audio.PlayOneShot(enemyDestroy);
                break;
            case "PackageDelivered":
                audio.PlayOneShot(packageDelivered);
                break;
            case "OutOfTime":
                audio.PlayOneShot(outOfTime);
                break;
            case "LevelFinish":
                audio.PlayOneShot(levelFinish);
                break;
            default:
                break;
        }
    }
}
