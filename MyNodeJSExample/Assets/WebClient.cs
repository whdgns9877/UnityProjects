using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Networking;
using Newtonsoft.Json;

[System.Serializable]
public class UserInfo
{
    public string name;
    public int age;
    public int tall;
    public int Level;
}

public class WebClient : MonoBehaviour
{
    [SerializeField] ScrollRect srUserList = null;

    [SerializeField] TMP_InputField inputName = null;
    [SerializeField] TMP_InputField inputAge = null;
    [SerializeField] TMP_InputField inputTall = null;

    public string ServerURL = null;
    public int Port = 0;

    public List<UserInfo> myUserInfo = null;
    int curMyIdx = -1;

    bool isProcessing = false;

    // ���� ��ư ����
    public void OnClick_Save()
    {
        if (isProcessing) return;

        myUserInfo[curMyIdx].name = inputName.text;
        myUserInfo[curMyIdx].age = int.Parse(inputAge.text);
        myUserInfo[curMyIdx].tall = int.Parse(inputTall.text);

        // JSON ������ �����͸� ���ڿ��� ��ȯ
        string jsonData = JsonUtility.ToJson(myUserInfo[curMyIdx]);

        isProcessing = true;
        StartCoroutine(processSave(jsonData));
    }

    IEnumerator processSave(string jsonData)
    {
        // URL �����
        string tartgetURL = ServerURL + ":" + Port + "/saveuserinfo";

        using (UnityWebRequest request = UnityWebRequest.Post(tartgetURL, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest(); // ������ �����͸� ������ �ڵ�

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // ���� ó��
                Debug.Log(request.error);
            }
            else
            {
                // ���� ó��
                Debug.Log("Save ok");
            }
        }

        isProcessing = false;
    }
    // 
    // ������ �����ؼ� ���������͸� �޾� �´�.
    // �޾� �� �����͸� ScrollView �� ����Ѵ�.
    public void OnClick_Load()
    {
        // ���� Ŭ�� ���� �ڵ�
        // �ѹ� ������ ����Ǹ� �Ϸ�� ������ �� ������� �ʴ´�.
        if (isProcessing) return;

        isProcessing = true;
        StartCoroutine(processLoad());
    }

    IEnumerator processLoad()
    {
        // URL ����� -  ���1, 2 �� ���� nodejs �������� �ٸ��� ó���� �ʿ��ϴ�.

        // http://localhost:8080/getuserlist
        string targetURL = ServerURL + ":" + Port + "/getuserlist";

        // using UnityEngine.Networking; �ʿ�...
        using (UnityWebRequest www = UnityWebRequest.Get(targetURL))
        {
            // ������ ������ ��û�ϱ�
            yield return www.SendWebRequest();

            UnityEngine.Debug.Log(www.downloadHandler.text);

            /// www.downloadHandler.text : ������ ���� JSON ������ �����͸� �޴´�.			
            myUserInfo = JsonConvert.DeserializeObject<List<UserInfo>>(www.downloadHandler.text);

            // �޾ƿ� ���������� ��Ͽ��� ���������� 1���� �����ٰ� UIUserData �� �����Ѵ�.
            for (int i = 0; i < myUserInfo.Count; i++)
            {

                UIUserData instUIObj = getLoadUserData();
                instUIObj.Init(this.gameObject, myUserInfo[i].name, i);
            }




            www.Dispose(); // www ��ü�� �����ϴ� ��...
        }

        isProcessing = false;
    }

    // UIUserData �� ���� �޽��� ó�� �Լ�
    // idx �� myUserInfo �� �ε���
    void selectUserData(int idx)
    {
        inputName.text = myUserInfo[idx].name;
        inputAge.text = myUserInfo[idx].age.ToString();
        inputTall.text = myUserInfo[idx].tall.ToString();

        curMyIdx = idx; // ���� ���õ� �������� �ε���
    }

    // UIUserData �������� �ε����� �ʾҴٸ� �������� �ε��ϰ�,
    // �� �������� ������� UIUserData ��ü�� �Ѱ� ����
    UIUserData prefabUIData = null;

    UIUserData getLoadUserData()
    {
        if(prefabUIData == null)
            prefabUIData = Resources.Load<UIUserData>("UIUserData");

        // �����ϸ鼭 �θ� ��ũ�Ѻ��� content�� ����
        UIUserData instUIObj = Instantiate(prefabUIData, srUserList.content.transform);
        return instUIObj;
    }

    // ���� ��ư ����
    public void OnClick_SaveLevel()
    {
        if (isProcessing) return;

        // ���� UI ���� ������ ���� �ִ´�..(���� ���� ����ġ ȹ�� �Ŀ� �������� �Ǵ� ���� �ùķ��̼�)
        //myUserInfo.Level = int.Parse(inputAge.text);

        // JSON ������ �����͸� ���ڿ��� ��ȯ
        //string jsonData = string.Format("{{\"Level\" : \"{0}\"}}", myUserInfo.Level); // jsonData = "{"Level":123}"

        isProcessing = true;
        //StartCoroutine(processSaveLevel(jsonData));
    }

    IEnumerator processSaveLevel(string jsonData)
    {
        // URL �����
        string tartgetURL = ServerURL + ":" + Port + "/savelevel";

        using (UnityWebRequest request = UnityWebRequest.Post(tartgetURL, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest(); // ������ �����͸� ������ �ڵ�


            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // ���� ó��
                Debug.Log(request.error);
            }
            else
            {
                // ���� ó��
                Debug.Log(request.downloadHandler.text);
            }
        }

        isProcessing = false;
    }

    // ������ �߰��ϱ�
    public void OnClick_AddData()
    {
        if (isProcessing) return;

        // ��Ʈ���� �����͸� ������ �ִ� �ڵ�
        UserInfo userInfo = new UserInfo();
        userInfo.name = inputName.text;
        userInfo.age = int.Parse(inputAge.text);
        userInfo.tall = int.Parse(inputTall.text);


        // JSON ������ �����͸� ���ڿ��� ��ȯ
        string jsonData = JsonUtility.ToJson(userInfo);
        UnityEngine.Debug.Log(jsonData);

        isProcessing = true;
        StartCoroutine(processAddData(jsonData));


        // Ŭ���̾�Ʈ �����Ϳ� �߰�
        myUserInfo.Add(userInfo);
        // UI ��ü 1�� �����
        UIUserData instUIObj = getLoadUserData();
        instUIObj.Init(this.gameObject, userInfo.name, myUserInfo.Count - 1);
    }
    IEnumerator processAddData(string jsonData)
    {
        string targetURL = ServerURL + ":" + Port + "/adduserinfo";

        using (UnityWebRequest request = UnityWebRequest.Post(targetURL, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest(); // ������ �����͸� ������ �ڵ�
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // ���� ó��
                UnityEngine.Debug.Log(request.error);
            }
            else
            {
                // ���� ó��
                UnityEngine.Debug.Log(request.downloadHandler.text);
            }
        }
        isProcessing = false;

    }


    // ������ �����ϱ�
    public void OnClick_DeleteData()
    {
        if (isProcessing) return;

        // ���� UI ���� ������ ���� �ִ´� (���� ���� ����ġ ȹ�� �Ŀ� �������� �Ǵ� ���� �ùķ��̼�)
        UserInfo userInfo = new UserInfo();
        userInfo.name = inputName.text;
        userInfo.age = int.Parse(inputAge.text);
        userInfo.tall = int.Parse(inputTall.text);

        // Json ������ �����͸� ���ڿ��� ��ȯ
        string jsonData = $"{{\"name\":\"{inputName.text}\"}}";
        Debug.Log(jsonData);

        isProcessing = true;
        StartCoroutine(processDeleteData(jsonData));
    }

    IEnumerator processDeleteData(string jsonData)
    {
        // URL �����
        string tartgetURL = ServerURL + ":" + Port + "/delete";

        using (UnityWebRequest request = UnityWebRequest.Post(tartgetURL, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest(); // ������ �����͸� ������ �ڵ�
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // ���� ó��
                Debug.Log(request.error);
            }
            else
            {
                // ���� ó��
                Debug.Log(request.downloadHandler.text);

                // ���ʿ� �̸� ��Ͽ��� ���� ������ �̸��� UI�� ����
                Destroy(srUserList.content.transform.GetChild(curMyIdx).gameObject);

                // myUserInfo���� �̸��� �ش��ϴ� UserInfo ������ ����
                myUserInfo.RemoveAt(curMyIdx);
            }
        }

        isProcessing = false;
    }
}
