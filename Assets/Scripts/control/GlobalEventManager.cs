using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GlobalEventManager
{
    public static event Action gameStarted, gameConfigured, gamePaused, gameResumed, gameFinished;

    public static event Action<Ship> shipDestroyed;

    public static void invokeGameStarted()
    {
        if (gameStarted != null)
        {
            gameStarted();
        }
    }

    public static void invokeGameConfigured()
    {
        if (gameConfigured != null)
        {
            gameConfigured();
        }
    }

    public static void invokeGamePaused()
    {
        if (gamePaused != null)
        {
            gamePaused();
        }
    }

    public static void invokeGameResumed()
    {
        if (gameResumed != null)
        {
            gameResumed();
        }
    }

    public static void invokeGameFinished()
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
