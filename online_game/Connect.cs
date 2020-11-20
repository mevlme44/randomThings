using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Connect : MonoBehaviour
{
    public static IEnumerator ConnectToRoom(int Game)
    {
        var Data = new WWWForm();
        Data.AddField("game", Game.ToString());
        Data.AddField("id", PlayFabLogin.ReturnMobileID());
        var Query = new WWW("http://mevlme44.myjino.ru/connect.php/", Data.data);
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            Query.MoveNext();
            if (Query.text == "No room")
                Debug.LogError(Query.text);
            else
            {
                GlobalDefines.RecRoom = int.Parse(Query.text);
                Debug.Log(Query.text);
            }
        }
        Query.Dispose();
    }

    public static IEnumerator LoadPlayers(int Game)
    {
        yield return new WaitForSeconds(3);
        Debug.Log(GlobalDefines.RecRoom);
        var Data = new WWWForm();
        Data.AddField("game", Game.ToString());
        Data.AddField("room", GlobalDefines.RecRoom.ToString());
        var Query = new WWW("http://mevlme44.myjino.ru/load.php/", Data.data);
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            Query.MoveNext();
            var tmp = Query.text.Split(' ');
            GlobalDefines.RecPlayer1 = tmp[0];
            GlobalDefines.RecPlayer2 = tmp[1];
            GlobalDefines.RecPlayer3 = tmp[2];
            GlobalDefines.RecPlayer4 = tmp[3];
            GlobalDefines.RecPlayer5 = tmp[4];
            Debug.LogError(GlobalDefines.RecPlayer1 + " " + GlobalDefines.RecPlayer2 + " " + GlobalDefines.RecPlayer3 + " " + GlobalDefines.RecPlayer4 + " " + GlobalDefines.RecPlayer5);
        }
        Query.Dispose();
    }
    public static IEnumerator LeaveFromRoom(int Game, int Room, string Id)
    {
        var Data = new WWWForm();
        Data.AddField("game", Game.ToString());
        Data.AddField("room", Room.ToString());
        Data.AddField("id", Id);
        var Query = new WWW("http://mevlme44.myjino.ru/leave.php/", Data.data);
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            Query.MoveNext();
            if (Query.text == "Success!")
            {
                Debug.Log("Correct exit");
            }
            else
            {
                Debug.LogError(Query.text);
            }
        }
        Query.Dispose();
    }
    public static IEnumerator LeaveFromRoom(int Game, int Room, string Id,bool disconnect)
    {
        var Data = new WWWForm();
        Data.AddField("game", Game.ToString());
        Data.AddField("room", Room.ToString());
        Data.AddField("id", Id);
        var Query = new WWW("http://mevlme44.myjino.ru/disconnect.php/", Data.data);
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            Query.MoveNext();
            if (Query.text == "Success!")
            {
                Debug.Log("Correct exit");
            }
            else
            {
                Debug.LogError(Query.text);
            }
        }
        Query.Dispose();
    }

    public static IEnumerator loadIn(MonoBehaviour parent, int Game)
    {

        yield return parent.StartCoroutine(ConnectToRoom(Game));
        yield return parent.StartCoroutine(CheckPlayers(Game, parent));
        GlobalDefines.Save = true;
        yield return parent.StartCoroutine(LoadPlayers(Game));
        GlobalDefines.WaitPlayer = false;
        
        SceneManager.LoadScene("Game" + Game.ToString());
    }
    public static IEnumerator ExitLoad(MonoBehaviour parent,int Game)
    {
        yield return parent.StartCoroutine(LeaveFromRoom(Game, GlobalDefines.RecRoom, PlayFabLogin.ReturnMobileID()));
    }
    public static IEnumerator loadOut(MonoBehaviour parent, int Game)
    {
        if(Game == 1)
        {
            yield return parent.StartCoroutine(CloudSave.POST(Game.ToString(), GlobalDefines.RecRoom, GlobalDefines.returnsFirstGame.ToString()));
            yield return parent.StartCoroutine(WaitForExit(Game, parent));
            yield return parent.StartCoroutine(CheckWinner(Game, parent,GlobalDefines.Que));
        }
        if (Game == 2)
        {
            yield return parent.StartCoroutine(CloudSave.POST(Game.ToString(), GlobalDefines.RecRoom, GlobalDefines.returnsSecondGame.ToString()));
            yield return parent.StartCoroutine(WaitForExit(Game, parent));
            yield return parent.StartCoroutine(CheckWinner(Game, parent,GlobalDefines.Que));
        }
        if (Game == 3)
        {       
            yield return parent.StartCoroutine(CloudSave.POST(Game.ToString(), GlobalDefines.RecRoom, GlobalDefines.returnsThirdGame.ToString()));
            yield return parent.StartCoroutine(WaitForExit(Game, parent));
            yield return parent.StartCoroutine(CheckWinner(Game, parent, GlobalDefines.Que));
        }  
        yield return parent.StartCoroutine(LeaveFromRoom(Game,GlobalDefines.RecRoom,PlayFabLogin.ReturnMobileID()));

        SceneManager.LoadScene("Win");
        yield return new WaitForSeconds(10);
        SceneManager.LoadScene("SampleScene");
    }
    public static IEnumerator CheckPlayers(int Game, MonoBehaviour parent)
    {
        var Data = new WWWForm();
        Data.AddField("game", Game.ToString());
        Data.AddField("room", GlobalDefines.RecRoom.ToString());
        var Query = new WWW("http://mevlme44.myjino.ru/check.php/", Data.data);
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            Query.MoveNext();
            if (Query.text == "No")
            {
                GlobalDefines.WaitPlayer = true;
                yield return new WaitForSeconds(8);
                yield return parent.StartCoroutine(CheckPlayers(Game, parent));

            }

            
               

        }
        Query.Dispose();
    }
    public static IEnumerator WaitForExit(int Game, MonoBehaviour parent)
    {
        var Data = new WWWForm();
        Data.AddField("game", Game.ToString());
        Data.AddField("room", GlobalDefines.RecRoom.ToString()); 
        var Query = new WWW("http://mevlme44.myjino.ru/exit.php/", Data.data);
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            Query.MoveNext();
            if (Query.text == "No")
            {
                yield return new WaitForSeconds(5);
                yield return parent.StartCoroutine(WaitForExit(Game, parent));

            }
            else
            {
                GlobalDefines.Que = Query.text;
                Debug.LogError(Query.text);
                GlobalDefines.WaitPlayerForExit = false;
            }
        }
        Query.Dispose();
    }

    public static IEnumerator CheckWinner(int Game, MonoBehaviour parent, string Que)
    {
        var Data = new WWWForm();
        Data.AddField("game", Game.ToString());
        Data.AddField("room", GlobalDefines.RecRoom.ToString());
        Data.AddField("que", Que);
        var Query = new WWW("http://mevlme44.myjino.ru/checkAllPlayers.php/", Data.data);
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            Query.MoveNext();
            var tmp = Query.text.Split(' ');
            Debug.LogError(Query.text);
            GlobalDefines.RecWinner = tmp[0];
            var id = tmp[2];
            if (id == PlayFabLogin.ReturnMobileID())
                GlobalDefines.Win = true;
            GlobalDefines.RecWinScore = int.Parse(tmp[1]);
            
        }
        Query.Dispose();
    }
    public static IEnumerator Change(string id, string name)
    {
        var Data = new WWWForm();
        Data.AddField("id",id);
        Data.AddField("name", name);
        var Query = new WWW("http://mevlme44.myjino.ru/name.php/", Data.data);
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            Query.MoveNext();
            GlobalDefines.Name = Query.text;

        }
        Query.Dispose();
    }
    public static IEnumerator Disconnect(MonoBehaviour parent,string id, int Game,int score)
    {
        yield return parent.StartCoroutine(CloudSave.POST(Game.ToString(), GlobalDefines.RecRoom, score.ToString()));
        yield return parent.StartCoroutine(LeaveFromRoom(Game, GlobalDefines.RecRoom, PlayFabLogin.ReturnMobileID(), true));
    }
    public static IEnumerator FirstLog(string id)
    {
        var Data = new WWWForm();
        Data.AddField("id", id);
        var Query = new WWW("http://mevlme44.myjino.ru/firstConnect.php/", Data.data);
        yield return Query;
        if (Query.error != null)
        {
            Debug.Log("Server does not respond : " + Query.error);
        }
        else
        {
            
            Query.MoveNext();
            if (Query.text == "Yes")
            {
                GlobalDefines.FirstLogin = true;
            }  
            else
            {
                GlobalDefines.FirstLogin = false;
            }

        }
        Query.Dispose();
    }
    public void OnApplicationQuit()
    {
        
        StartCoroutine(Disconnect(this,PlayFabLogin.ReturnMobileID(),GlobalDefines.RecGame,0));
    }
}
