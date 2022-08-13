using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projecton : MonoBehaviour
{
    private Scene simulationScene;
    private PhysicsScene physicsScene;

    [SerializeField] private GameObject[] obstacles = new GameObject[0];

    void Start() => CreatePhysicsScene(obstacles);

    public void CreatePhysicsScene(GameObject[] obstacles)
    {
        simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physicsScene = simulationScene.GetPhysicsScene();

        foreach (GameObject obstacle in obstacles)
        {
            GameObject environment = Instantiate(obstacle, obstacle.transform.position, obstacle.transform.rotation);
            SceneManager.MoveGameObjectToScene(environment, simulationScene);

            List<GameObject> childrenToDestroy = new List<GameObject>();

            if (environment.transform.childCount > 0)
            {
                foreach (Transform child in environment.transform)
                {
                    if (!child.GetComponent<Collider>())
                    {
                        childrenToDestroy.Add(child.gameObject);
                        continue;
                    }

                    Renderer render = child.GetComponent<Renderer>();
                    if (render != null) render.enabled = false;
                }
            }
            else
            {
                Renderer render = environment.GetComponent<Renderer>();
                if (render != null) render.enabled = false;
            }

            foreach (GameObject child in childrenToDestroy) Destroy(child);
        }
    }

    public Vector3[] SimulateTrajectory(GameObject prefab, Vector3 pos, Quaternion rotation, Action<Rigidbody> AddForce, int maxPhysicsFrameIterations, float simulationMulti)
    {
        GameObject ghostobj = ObjectPooler.Instance.Spawn(prefab, true, pos, rotation);

        Rigidbody ghostRb = ghostobj.GetComponent<Rigidbody>();
        if (ghostRb == null) return new Vector3[] { Vector3.zero };

        ghostRb.velocity = Vector3.zero;
        ghostobj.transform.SetParent(null);
        SceneManager.MoveGameObjectToScene(ghostobj, simulationScene);
        AddForce(ghostRb);

        Vector3[] positions = new Vector3[maxPhysicsFrameIterations];

        for (int i = 0; i < positions.Length; i++)
        {
            if (simulationMulti <= 0) break;

            physicsScene.Simulate(Time.fixedDeltaTime * simulationMulti * 4f);
            positions[i] = ghostRb.transform.position;
        }

        ghostobj.SetActive(false);
        SceneManager.MoveGameObjectToScene(ghostobj, SceneManager.GetActiveScene());
        ghostobj.transform.SetParent(ObjectPooler.Instance.transform);
        return positions;
    }
}