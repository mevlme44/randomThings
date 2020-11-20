using UnityEngine;

public class Mirror : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var col = collision.gameObject.GetComponent<Citizen>();
        if (collision.tag == "Citizen" && col.GetBehaviour() == 2)
        {
            col.SetNormal(-col.GetNormal());
        }
    }
}
