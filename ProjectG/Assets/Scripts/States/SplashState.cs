﻿using UnityEngine.SceneManagement;
using UnityEngine;

public class SplashState : GameState
{
    private float m_SplashTimer;

    protected override void OnStart()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnSceneChanged;
        SceneManager.LoadScene("Splash");

        AudioManager.Instance.PlayAutomaticAudioGroup(5, DataManager.Data.AmbientGroup);
        AudioManager.Instance.SetChannelVolume(AudioManager.ChannelType.FX, 0.3f);

        AudioManager.Instance.Play2DAudio(Resources.Load<AudioClip>("Audio/Ambience/Sea Waves"), AudioManager.ChannelType.AMBIENCE, true);
        AudioManager.Instance.SetChannelVolume(AudioManager.ChannelType.AMBIENCE, 0);
        AudioManager.Instance.FadeChannel(AudioManager.ChannelType.AMBIENCE, 0.3f, 1);
    }

    protected override void OnEnd()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    protected override void OnUpdate()
    {
        m_SplashTimer += Time.deltaTime;
        if(m_SplashTimer > 18)
        {
            GameManager.Instance.TransitionToNewState(GameManager.Instance.State<MainMenuState>());
        }
    }

    protected override void OnFixedUpdate()
    {
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {

    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(loadedScene);
    }
}
