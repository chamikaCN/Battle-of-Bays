using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GlobalEventManager
{
    public static event Action gameStarted, gamePaused, gameResumed, gameFinished;

    public static event Action<Ship> shipDestroyed;

    public static void invokeGameStart()
    {
        if (gameStarted != null)
        {
            gameStarted();
        }
    }

    public static void invokeGamePause()
    {
        if (gamePaused != null)
        {
            gamePaused();
        }
    }

    public static void invokeGameResume()
    {
        if (gameResumed != null)
        {
            gameResumed();
        }
    }

    public static void invokeGameFinish()
    {
        if (gameFinished != null)
        {
            gameFinished();
        }
    }

    public static void invokeDestroyShip(Ship s)
    {
        if (shipDestroyed != null)
        {
            shipDestroyed(s);
        }
    }
}
