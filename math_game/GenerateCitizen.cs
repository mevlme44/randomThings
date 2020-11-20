using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateCitizen : MonoBehaviour
{
    public InputField countOfCitizen;
    public GameObject prefab;
    public Slider[] probability;
    public InputField changeTimer;
    public Animator panelError;
    public Text textError;
    private bool error = false;
    public GameObject plane;
    public void Start()
    {
        Defines.countOfCitizen = countOfCitizen;
        Defines.prefab = prefab;
        Defines.probability = probability;
        Defines.changeTimer = changeTimer;
        Defines.plane = plane;
    }
    public void Tap()
    {
        CheckInputError();
        if (!error)
        {
            Generate(int.Parse(Defines.countOfCitizen.text), float.Parse(Defines.changeTimer.text), gameObject);
        }
        error = false;
    }
    public static void Generate(int countOfCitizen, float timer,GameObject citizen)
    {
        GenerateFigure link = new GenerateFigure();
        int N;
        Citizen tmpObj;
        PolygonCollider2D tmpPolyCol;
        float randomAngle = 0;
        Color randomColor = Color.white;
        List<Vector3>[] normals = new List<Vector3>[countOfCitizen];
        for (int i = 0; i < countOfCitizen; i++)
        {
            normals[i] = new List<Vector3>();
        }
        for (int i = 0; i < countOfCitizen; i++)
        {
            N = Random.Range(3, 7);
            randomAngle = Random.Range(0, 180);
            randomColor = GenerateColor();
            var newCitizen = Instantiate(Defines.prefab, new Vector2(Random.Range(-4*Defines.modified, 5*Defines.modified), Random.Range(-4 * Defines.modified, 5 * Defines.modified)),
                             Quaternion.AngleAxis(randomAngle, citizen.transform.forward));
            tmpObj = newCitizen.gameObject.GetComponent<Citizen>();
            tmpPolyCol = newCitizen.GetComponent<PolygonCollider2D>();
            newCitizen.name = Random.Range(0, 100).ToString();
            tmpObj.SetNumberOfSides(N);
            tmpObj.SetBehaviour(randomColor);
            link.PolyMesh(0.5f, N, tmpPolyCol);
            Vector2[] sides = tmpPolyCol.points;
            tmpObj.SetTimer(timer);
            for (int j = 0; j < N; j++)
            {
                normals[i].Add(sides[j]);
            }
            int side = Random.Range(1, N + 1);
            Vector3 rotation;
            if (side != N)
                rotation = GetProjected(normals[i][side - 1], normals[i][side], citizen.transform.position).normalized;
            else
                rotation = GetProjected(normals[i][side - 1], normals[i][0], citizen.transform.position).normalized;
            float x = rotation.x, y = rotation.y;
            var xNew = x * Mathf.Cos(randomAngle * Mathf.PI / 180) - y * Mathf.Sin(randomAngle * Mathf.PI / 180);
            var yNew = x * Mathf.Sin(randomAngle * Mathf.PI / 180) + y * Mathf.Cos(randomAngle * Mathf.PI / 180);
            rotation = new Vector3(xNew, yNew, rotation.z).normalized;
            tmpObj.SetNormal(rotation);
        }
    }
    public static void Generate(float timer, GameObject citizen,GameObject collision, Vector2 position)
    {
        Citizen tmpObj;
        PolygonCollider2D tmpPolyCol;
        GenerateFigure link = new GenerateFigure();
        float randomAngle = Random.Range(0, 180);
        int N = collision.GetComponent<Citizen>().GetNumberOfSides() + citizen.GetComponent<Citizen>().GetNumberOfSides();
        Vector3[] normals = new Vector3[N];
        var newCitizen = Instantiate(Defines.prefab,position, Quaternion.AngleAxis(randomAngle, citizen.transform.forward));
        tmpObj = newCitizen.gameObject.GetComponent<Citizen>();
        tmpPolyCol = newCitizen.GetComponent<PolygonCollider2D>();
        newCitizen.name = Random.Range(0, 100).ToString();
        tmpObj.SetNumberOfSides(N);
        tmpObj.SetBehaviour(GenerateColor());
        link.PolyMesh(0.5f, N, tmpPolyCol);
        Vector2[] sides = tmpPolyCol.points;
        tmpObj.SetTimer(timer);
        newCitizen.transform.localScale =citizen.transform.localScale * 1.125f;
        if(newCitizen.transform.localScale.x > Defines.plane.transform.localScale.x*2)
        {
            Camera.main.orthographicSize *= 1.5f;
            Defines.plane.transform.localScale *= 1.5f;
            Defines.modified++;
        }
        for (int j = 0; j < N; j++)
        {
            normals[j] = (sides[j]);
        }
        int side = Random.Range(1, N + 1);
        Vector3 rotation;
        if (side != N)
            rotation = GetProjected(normals[side - 1], normals[side], citizen.transform.position).normalized;
        else
            rotation = GetProjected(normals[side - 1], normals[0], citizen.transform.position).normalized;
        float x = rotation.x, y = rotation.y;
        var xNew = x * Mathf.Cos(randomAngle * Mathf.PI / 180) - y * Mathf.Sin(randomAngle * Mathf.PI / 180);
        var yNew = x * Mathf.Sin(randomAngle * Mathf.PI / 180) + y * Mathf.Cos(randomAngle * Mathf.PI / 180);
        rotation = new Vector3(xNew, yNew, rotation.z).normalized;
        tmpObj.SetNormal(rotation);
    }
    public void CheckInputError()
    {
        if (Defines.changeTimer.text.Contains("."))
        {
            StartCoroutine(ErrorAnimation("Лишний символ"));
            error = true;
        }
        else if (Defines.countOfCitizen.text == "" || Defines.changeTimer.text == "")
        {
            StartCoroutine(ErrorAnimation("Пустое поле!"));
            error = true;
        }
        else if (int.Parse(Defines.countOfCitizen.text) <= 0 || float.Parse(Defines.changeTimer.text) <= 0)
        {
            StartCoroutine(ErrorAnimation("Слишком мало"));
            error = true;
        }
        else if (int.Parse(Defines.countOfCitizen.text) >= 30)
        {
            StartCoroutine(ErrorAnimation("Слишком много"));
            error = true;
        }
    }
    public static Vector3 GetProjected(Vector3 s, Vector3 f, Vector3 c)
    {
        Vector3 startToFinish = f - s;
        Vector3 prj = Vector3.Project(c - s, startToFinish);
        return prj + s;
    }
    public static Color GenerateColor()
    {
        int[] probability = new int[4];
        if (Defines.probability[0].value == 0 && Defines.probability[1].value == 0 && Defines.probability[2].value == 0 &&
           Defines.probability[3].value == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                probability[i] = 1;
            }
            var tmp = Random.Range(0, 4);
            if (tmp == 0)
                return Color.red;
            else if (tmp == 1)
                return Color.blue;
            else if (tmp == 2)
                return Color.cyan;
            else return Color.green;

        }
        int sum = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += (int)Defines.probability[i].value;
        }
        var tmpRand = Random.Range(0, sum);
        if (tmpRand <= Defines.probability[0].value)
            return Color.red;
        else if (tmpRand <= Defines.probability[1].value + Defines.probability[0].value)
            return Color.blue;
        else if (tmpRand <= Defines.probability[2].value + Defines.probability[1].value + Defines.probability[0].value)
            return Color.cyan;
        else if (tmpRand <= Defines.probability[3].value + Defines.probability[2].value +
                 Defines.probability[1].value + Defines.probability[0].value)
            return Color.green;

        return Color.black;
    }
    public IEnumerator ErrorAnimation(string message)
    {
        textError.text = message;
        panelError.enabled = true;
        yield return new WaitForSeconds(1.75f);
        panelError.enabled = false;
    }
}
