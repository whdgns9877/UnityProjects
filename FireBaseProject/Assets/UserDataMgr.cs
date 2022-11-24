using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase.Firestore;
using Firebase.Extensions;
using System;

// ���̾�̽��� �����ϴ� ������ Ŭ����
[FirestoreData]
public class UserInfo
{
    public UserInfo()
    {
        ID = "�⺻��";
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
    // �̱���
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
            // �����Ͱ� �����ϴ��� üũ�ؼ�...
            if (docSnapShot.Exists)
            {
                Debug.Log("## ������ ���� ����...");
                MyUserInfo = docSnapShot.ConvertTo<UserInfo>();

                // �Ϸ� �� �ݹ� ȣ��
                callbackForRead.Invoke(0);
            }
            // �����Ͱ� ���ٸ� �Ѱ� �߰�...
            else
            {
                myDocRef.SetAsync(MyUserInfo).ContinueWithOnMainThread(userInfoTask =>
                {
                    Debug.Log("## ���� �⺻ ������ �߰�");

                    // dhksfy gn zhfqor ghcnf
                    callbackForRead.Invoke(0);
                });
            }
        });
    }

    //// UserInfo ����
    public void SetUserInfo(string strID, string strName, int inScore)
    {
        MyUserInfo.ID = strID;
        MyUserInfo.Name = strName;
        MyUserInfo.Score = inScore;

        // FireStore �� ����
        Dictionary<string, object> userInfo = new Dictionary<string, object>
        {
            {"ID", strID },
            {"Name", strName },
            {"Score", inScore }
        };
        myDocRef.UpdateAsync(userInfo).ContinueWithOnMainThread(task =>
        {
            Debug.Log($"## UserInfo ����Ϸ� : {strID} {strName} {inScore}");
        });
    }
}
