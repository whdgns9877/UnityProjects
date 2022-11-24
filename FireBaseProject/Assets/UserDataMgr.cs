using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase.Firestore;
using Firebase.Extensions;
using System;

// 파이어베이스에 저장하는 데이터 클래스
[FirestoreData]
public class UserInfo
{
    public UserInfo()
    {
        ID = "기본값";
    }
    [FirestoreProperty]
    public string ID { get; set; }

    [FirestoreProperty]
    public string Name { get; set; }

    [FirestoreProperty]
    public int Score { get; set; }

}

public class UserDataMgr : MonoBehaviour
{
    // 싱글턴
    #region Singleton
    private static UserDataMgr instance = null;
    public static UserDataMgr Inst
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<UserDataMgr>();
                if (instance == null)
                {
                    instance = new GameObject("UserDataMgr", typeof(UserDataMgr)).GetComponent<UserDataMgr>();
                    if (instance == null)
                        Debug.LogError("##[Error] UserDataMgr Singleton is failed to initialize");
                }
            }

            return instance;
        }
    }
    #endregion // Singleton

    public string FirebaseUserId { get; set; }

    FirebaseFirestore myFirestore = null;
    DocumentReference myDocRef = null;

    string myUID = "";

    public UserInfo MyUserInfo = new UserInfo();

    public void Init(string uid)
    {
        myUID = uid;
        myFirestore = FirebaseFirestore.DefaultInstance;
        myDocRef = myFirestore.Document($"userlist/{uid}");
    }

    public void ReadUserInfoData(Action<byte> callbackForRead)
    {
        myDocRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot docSnapShot = task.Result;
            // 데이터가 존재하는지 체크해서...
            if (docSnapShot.Exists)
            {
                Debug.Log("## 데이터 가져 오기...");
                MyUserInfo = docSnapShot.ConvertTo<UserInfo>();

                // 완료 후 콜백 호출
                callbackForRead.Invoke(0);
            }
            // 데이터가 없다면 한개 추가...
            else
            {
                myDocRef.SetAsync(MyUserInfo).ContinueWithOnMainThread(userInfoTask =>
                {
                    Debug.Log("## 유저 기본 데이터 추가");

                    // dhksfy gn zhfqor ghcnf
                    callbackForRead.Invoke(0);
                });
            }
        });
    }

    //// UserInfo 변경
    public void SetUserInfo(string strID, string strName, int inScore)
    {
        MyUserInfo.ID = strID;
        MyUserInfo.Name = strName;
        MyUserInfo.Score = inScore;

        // FireStore 에 저장
        Dictionary<string, object> userInfo = new Dictionary<string, object>
        {
            {"ID", strID },
            {"Name", strName },
            {"Score", inScore }
        };
        myDocRef.UpdateAsync(userInfo).ContinueWithOnMainThread(task =>
        {
            Debug.Log($"## UserInfo 변경완료 : {strID} {strName} {inScore}");
        });
    }
}
