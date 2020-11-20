using UnityEngine;

public class Citizen : MonoBehaviour
{
    private Color behaviour;
    private int numberOfBehaviour;
    private int numberOfSides;
    private Vector3 normal;
    private float timer;
    private float tmpTime;
    private GameObject Target;
    private float lifeTime = 0;
    public void Start()
    {
        gameObject.GetComponent<Renderer>().material.color = behaviour;
        tmpTime = timer;

    }
    public void Update()
    {
        if (!Defines.pause)
        {
            lifeTime += Time.deltaTime;
            switch (numberOfBehaviour)
            {
                case 1:

                    try
                    {
                        if (Target.activeInHierarchy)
                        {
                            transform.position = Vector2.MoveTowards(transform.position, Target.transform.position, 1f * Time.deltaTime);
                            transform.rotation = Quaternion.FromToRotation(normal, Target.transform.position);
                        }
                    }
                    catch
                    {
                        Target = FindClosestCitizen();
                    };
                    break;
                case 2:
                    transform.position += normal * Time.deltaTime;
                    break;
                case 3:
                    transform.position += normal * Time.deltaTime;
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        float x = normal.x, y = normal.y;
                        var xNew = x * Mathf.Cos(Mathf.PI / 2) - y * Mathf.Sin(Mathf.PI / 2);
                        var yNew = x * Mathf.Sin(Mathf.PI / 2) + y * Mathf.Cos(Mathf.PI / 2);
                        normal = new Vector3(xNew, yNew, normal.z).normalized;
                        timer = tmpTime;
                    }
                    break;
                case 4:
                    break;
            }
        }
    }
    public void SetBehaviour(Color color)
    {
        behaviour = color;
        if (color == Color.red)
            numberOfBehaviour = 1;
        else if (color == Color.blue)
            numberOfBehaviour = 2;
        else if (color == Color.cyan)
            numberOfBehaviour = 3;
        else
            numberOfBehaviour = 4;
    }
    public void SetNumberOfSides(int numOfSides)
    {
        numberOfSides = numOfSides;
    }
    public void SetNormal(Vector3 target)
    {
        normal = target;
    }
    public Vector3 GetNormal()
    {
        return normal;
    }
    public int GetBehaviour()
    {
        return numberOfBehaviour;
    }
    public void SetTimer(float time)
    {
        timer = time;
    }
    public int GetNumberOfSides()
    {
        return numberOfSides;
    }
    public float GetLifeTime()
    {
        return lifeTime;
    }
    public void SetLifeTime()
    {
        lifeTime += Random.Range(0.00001f, 0.99999f);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Citizen")
        {
            var col = collision.gameObject.GetComponent<Citizen>();
            float timer = Defines.changeTimer.text == "" ? 3f : float.Parse(Defines.changeTimer.text);
            if (col.GetNumberOfSides() < GetNumberOfSides())
            {
                GenerateCitizen.Generate(timer, gameObject, collision.gameObject, transform.position);
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
            else if (col.GetNumberOfSides() == GetNumberOfSides())
            {
                SetLifeTime();
                if (col.GetLifeTime() < GetLifeTime())
                {
                    GenerateCitizen.Generate(timer, gameObject, collision.gameObject, transform.position);
                    Destroy(collision.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }
    GameObject FindClosestCitizen()
    {
        GameObject closest = null;
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Citizen");
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in targets)
        {
            if (go != gameObject)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }
}
