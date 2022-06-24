using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projecton : MonoBehaviour
{
    private Scene simulationScene;
    private PhysicsScene physicsScene;

    [SerializeField] private GameObject[] obstacles = new GameObject[0];

    //void Start() => CreatePhysicsScene(obstacles);

    public void CreatePhysicsScene(GameObject[] obstacles)
    {
        simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        physicsScene = simulationScene.GetPhysicsScene();

        foreach (GameObject obstacle in obstacles)
        {
            GameObject environment = ObjectPooler.Instance.Spawn(obstacle, false);
            SceneManager.MoveGameObjectToScene(environment, simulationScene);
        }
    }

    [SerializeField] private int maxPhysicsFrameIterations = 100;

    public Vector3[] SimulateTrajectory(GameObject prefab, Vector3 pos, Quaternion rotation, UnityAction<Rigidbody> Force)
    {
        Rigidbody ghostRb = prefab.GetComponent<Rigidbody>();
        if (ghostRb == null) return new Vector3[] { Vector3.zero };

        GameObject ghostobj = ObjectPooler.Instance.Spawn(prefab, true, pos, rotation);
        SceneManager.MoveGameObjectToScene(ghostobj, simulationScene);

        Force(ghostRb);

        Vector3[] positions = new Vector3[maxPhysicsFrameIterations];

        for (int i = 0; i < positions.Length; i++)
        {
            physicsScene.Simulate(Time.fixedDeltaTime);
            positions[i] = ghostobj.transform.position;
        }

        SceneManager.MoveGameObjectToScene(ghostobj, SceneManager.GetActiveScene());

        return positions;
    }
}