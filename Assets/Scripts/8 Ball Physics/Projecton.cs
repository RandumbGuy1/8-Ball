using System;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projecton : MonoBehaviour
{
    private Scene simulationScene;
    private PhysicsScene physicsScene;

    public static Projecton Instance { get; private set; }

    [SerializeField] private GameObject[] obstacles = new GameObject[0];

    void Awake() => Instance = this;
    void Start() => CreatePhysicsScene(obstacles);

    public void CreatePhysicsScene(GameObject[] obstacles)
    {
        simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physicsScene = simulationScene.GetPhysicsScene();

        foreach (GameObject obstacle in obstacles)
        {
            GameObject environment = ObjectPooler.Instance.Spawn(obstacle, false);
            environment.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(environment, simulationScene);
        }
    }

    public Vector3[] SimulateTrajectory(GameObject prefab, Vector3 pos, Quaternion rotation, Action<Rigidbody> AddForce, int maxPhysicsFrameIterations)
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
            physicsScene.Simulate(Time.fixedDeltaTime);
            positions[i] = ghostRb.transform.position;

            if (ghostRb.velocity.sqrMagnitude < 0.01f) break;
         }

        ghostobj.SetActive(false);
        SceneManager.MoveGameObjectToScene(ghostobj, SceneManager.GetActiveScene());
        ghostobj.transform.SetParent(ObjectPooler.Instance.transform);
        return positions;
    }
}