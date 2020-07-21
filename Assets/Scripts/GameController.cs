using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    MapGenerator generator;
    public Camera cam;
    System.Random random;
    int gameSeed;
    public LayerMask waterLayerMask,shipsLayerMask;
    public GameObject ship;
    List<GameObject> placedShips;
    Ship currentShip;
    void Start()
    {
        generator = GetComponent<MapGenerator>();
        random = new System.Random();
        gameSeed = random.Next(10000);
        generator.GenerateMap(gameSeed);
        placedShips = generator.PlaceShips(ship,3);
        currentShip = placedShips[0].GetComponent<Ship>();
        currentShip.activateSelector();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 1500, shipsLayerMask)){
                currentShip.deactivateSelector();
                currentShip = hit.collider.GetComponent<Ship>();
                currentShip.activateSelector();
            }
            else if(Physics.Raycast(ray,out hit,1500, waterLayerMask)){
                currentShip.moveToPoint(hit.point);
            }
        }
    }
}
