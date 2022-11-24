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

    // 저장 버튼 연결
    public void OnClick_Save()
    {
        if (isProcessing) return;

        myUserInfo[curMyIdx].name = inputName.text;
        myUserInfo[curMyIdx].age = int.Parse(inputAge.text);
        myUserInfo[curMyIdx].tall = int.Parse(inputTall.text);

        // JSON 형식의 데이터를 문자열로 변환
        string jsonData = JsonUtility.ToJson(myUserInfo[curMyIdx]);

        isProcessing = true;
        StartCoroutine(processSave(jsonData));
    }

    IEnumerator processSave(string jsonData)
    {
        // URL 만들기
        string tartgetURL = ServerURL + ":" + Port + "/saveuserinfo";

        using (UnityWebRequest request = UnityWebRequest.Post(tartgetURL, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest(); // 실제로 데이터를 보내는 코드

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // 에러 처리
                Debug.Log(request.error);
            }
            else
            {
                // 정상 처리
                Debug.Log("Save ok");
            }
        }

        isProcessing = false;
    }
    // 
    // 서버에 접속해서 유저데이터를 받아 온다.
    // 받아 온 데이터를 ScrollView 에 출력한다.
    public void OnClick_Load()
    {
        // 연속 클릭 방지 코드
        // 한번 눌러서 실행되면 완료될 때까지 또 실행되지 않는다.
        if (isProcessing) return;

        isProcessing = true;
        StartCoroutine(processLoad());
    }

    IEnumerator processLoad()
    {
        // URL 만들기 -  방법1, 2 에 따라서 nodejs 서버에서 다르게 처리가 필요하다.

        // http://localhost:8080/getuserlist
        string targetURL = ServerURL + ":" + Port + "/getuserlist";

        // using UnityEngine.Networking; 필요...
        using (UnityWebRequest www = UnityWebRequest.Get(targetURL))
        {
            // 지정된 서버에 요청하기
            yield return www.SendWebRequest();

            UnityEngine.Debug.Log(www.downloadHandler.text);

            /// www.downloadHandler.text : 서버가 보낸 JSON 형식의 데이터를 받는다.			
            myUserInfo = JsonConvert.DeserializeObject<List<UserInfo>>(www.downloadHandler.text);

            // 받아온 유저데이터 목록에서 유저데이터 1개씩 가져다가 UIUserData 를 생성한다.
            for (int i = 0; i < myUserInfo.Count; i++)
            {

                UIUserData instUIObj = getLoadUserData();
                instUIObj.Init(this.gameObject, myUserInfo[i].name, i);
            }




            www.Dispose(); // www 객체를 삭제하는 것...
        }

        isProcessing = false;
    }

    // UIUserData 가 보낸 메시지 처리 함수
    // idx 는 myUserInfo 의 인덱스
    void selectUserData(int idx)
    {
        inputName.text = myUserInfo[idx].name;
        inputAge.text = myUserInfo[idx].age.ToString();
        inputTall.text = myUserInfo[idx].tall.ToString();

        curMyIdx = idx; // 현재 선택된 데이터의 인덱스
    }

    // UIUserData 프리팹을 로드하지 않았다면 프리팹을 로드하고,
    // 그 프리팹을 기반으로 UIUserData 객체를 한개 생성
    UIUserData prefabUIData = null;

    UIUserData getLoadUserData()
    {
        if(prefabUIData == null)
            prefabUIData = Resources.Load<UIUserData>("UIUserData");

        // 생성하면서 부모를 스크롤뷰의 content로 설정
        UIUserData instUIObj = Instantiate(prefabUIData, srUserList.content.transform);
        return instUIObj;
    }

    // 저장 버튼 연결
    public void OnClick_SaveLevel()
    {
        if (isProcessing) return;

        // 레벨 UI 값을 변수에 먼저 넣는다..(게임 상의 경험치 획득 후에 레벨업이 되는 것을 시뮬레이션)
        //myUserInfo.Level = int.Parse(inputAge.text);

        // JSON 형식의 데이터를 문자열로 변환
        //string jsonData = string.Format("{{\"Level\" : \"{0}\"}}", myUserInfo.Level); // jsonData = "{"Level":123}"

        isProcessing = true;
        //StartCoroutine(processSaveLevel(jsonData));
    }

    IEnumerator processSaveLevel(string jsonData)
    {
        // URL 만들기
        string tartgetURL = ServerURL + ":" + Port + "/savelevel";

        using (UnityWebRequest request = UnityWebRequest.Post(tartgetURL, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest(); // 실제로 데이터를 보내는 코드


            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // 에러 처리
                Debug.Log(request.error);
            }
            else
            {
                // 정상 처리
                Debug.Log(request.downloadHandler.text);
            }
        }

        isProcessing = false;
    }

    // 데이터 추가하기
    public void OnClick_AddData()
    {
        if (isProcessing) return;

        // 컨트롤의 데이터를 변수에 넣는 코드
        UserInfo userInfo = new UserInfo();
        userInfo.name = inputName.text;
        userInfo.age = int.Parse(inputAge.text);
        userInfo.tall = int.Parse(inputTall.text);


        // JSON 형식의 데이터를 문자열로 변환
        string jsonData = JsonUtility.ToJson(userInfo);
        UnityEngine.Debug.Log(jsonData);

        isProcessing = true;
        StartCoroutine(processAddData(jsonData));


        // 클라이언트 데이터에 추가
        myUserInfo.Add(userInfo);
        // UI 객체 1개 만들고
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

            yield return request.SendWebRequest(); // 실제로 데이터를 보내는 코드
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // 에러 처리
                UnityEngine.Debug.Log(request.error);
            }
            else
            {
                // 정상 처리
                UnityEngine.Debug.Log(request.downloadHandler.text);
            }
        }
        isProcessing = false;

    }


    // 데이터 삭제하기
    public void OnClick_DeleteData()
    {
        if (isProcessing) return;

        // 레벨 UI 값을 변수에 먼저 넣는다 (게임 상의 경험치 획득 후에 레벨업이 되는 것을 시뮬레이션)
        UserInfo userInfo = new UserInfo();
        userInfo.name = inputName.text;
        userInfo.age = int.Parse(inputAge.text);
        userInfo.tall = int.Parse(inputTall.text);

        // Json 형식의 데이터를 문자열로 변환
        string jsonData = $"{{\"name\":\"{inputName.text}\"}}";
        Debug.Log(jsonData);

        isProcessing = true;
        StartCoroutine(processDeleteData(jsonData));
    }

    IEnumerator processDeleteData(string jsonData)
    {
        // URL 만들기
        string tartgetURL = ServerURL + ":" + Port + "/delete";

        using (UnityWebRequest request = UnityWebRequest.Post(tartgetURL, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest(); // 실제로 데이터를 보내는 코드
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                // 에러 처리
                Debug.Log(request.error);
            }
            else
            {
                // 정상 처리
                Debug.Log(request.downloadHandler.text);

                // 왼쪽에 이름 목록에서 현재 삭제한 이름도 UI도 삭제
                Destroy(srUserList.content.transform.GetChild(curMyIdx).gameObject);

                // myUserInfo에도 이름에 해당하는 UserInfo 데이터 삭제
                myUserInfo.RemoveAt(curMyIdx);
            }
        }

        isProcessing = false;
    }
}
