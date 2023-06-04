using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GlobalEventManager
{
    public static event Action gameStarted, gamePaused, gameResumed;

    public static event Action<Ship> shipDestroyed;

    public static event Action<GameController.Team> gameFinished;

    public static event Action<GameController.Team, int> gameConfigured;

    public static void invokeGameStarted()
    {
        if (gameStarted != null)
        {
            Debug.Log("Game Started");
            gameStarted();
        }
    }

    public static void invokeGameConfigured(GameController.Team playerTeam, int playerHQindex)
    {
        if (gameConfigured != null)
        {
            Debug.Log("Game Configured");
            Debug.Log(playerTeam + " : "+ playerHQindex);
            gameConfigured(playerTeam, playerHQindex);
        }
    }

    public static void invokeGamePaused()
    {
        if (gamePaused != null)
        {
            Debug.Log("Game Paused");
            gamePaused();
        }
    }

    public static void invokeGameResumed()
    {
        if (gameResumed != null)
        {
            Debug.Log("Game Resumed");
            gameResumed();
        }
    }

    public static void invokeGameFinished(GameController.Team winningTeam)
    {
        if (gameFinished != null)
        {
            Debug.Log("Game Finished");
            gameFinished(winningTeam);
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
