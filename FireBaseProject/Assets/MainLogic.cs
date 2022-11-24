using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Firebase.Auth;
using Firebase.Extensions;

public class MainLogic : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtGuestUID = null;

    [SerializeField] TMP_InputField inputID = null;
    [SerializeField] TMP_InputField inputName = null;
    [SerializeField] TMP_InputField inputScore = null;

    FirebaseAuth firebaseAuth = null;
    FirebaseUser firebaseUser = null;

    IEnumerator Start()
    {
        // ���̾�̽� �ʱ�ȭ
        Debug.Log("## Initialize Firebase ...");
        // Firebase ���� �������� �ִ� ��� ���ϵ��� �����ϴ��� üũ
        // ���� ������ �ִٸ� fix �õ�..
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                Debug.Log("## [Firebase] Setting up Firebase Auth!!!");
                firebaseAuth = FirebaseAuth.DefaultInstance;
                firebaseAuth.StateChanged += onFirebaseAuthStateChanged;
                onFirebaseAuthStateChanged(this, null);

                // ���� ������ ������ ���ٸ� �͸������� �ٷ� ����
                if (firebaseAuth.CurrentUser == null)
                {
                    Debug.Log("##[Firebase] ������ ������ ������ �ϴ� �Խ�Ʈ�� ����");
                    onGuestLogin();
                }
                else
                {
                    // Firebase Unity SKD is not safe to use here.
                    Debug.Log("##[Firebase] ������ ������ �ֽ��ϴ� : " + firebaseAuth.CurrentUser.UserId);
                    onLoginComplete();
                }
            }
            else
            {
                // Firebase Unity SKD is safe to use here.
                Debug.LogError($"Could not resolve all Firebase dependencied: {task.Result}");
            }
        });

        yield return null;
    }

    // Firebase ���� ���� ����
    void onFirebaseAuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("## onFirebaseAuthStateChanged");
        if (firebaseAuth.CurrentUser != firebaseUser) // ������ �פ������°� �ٲ�� ���...
        {
            // ���ο� ������ �α����ߴٴ� �ǹ�
            bool signedIn = firebaseUser != firebaseAuth.CurrentUser && firebaseAuth.CurrentUser != null;
            if (!signedIn && firebaseUser != null)
            {
                Debug.Log($"##[Firebase - AuthStateChanged] signed out : Provider[{firebaseUser.ProviderId}] UserId[{firebaseUser.UserId}]");
            }
            firebaseUser = firebaseAuth.CurrentUser;
            if (signedIn)
            {
                Debug.Log($"##[Firebase - AuthStateChanged] signed in : Provider[{firebaseUser.ProviderId}] UserId[{firebaseUser.UserId}]");
            }
            // �����͸� �а� ������ �� �ʿ���
            UserDataMgr.Inst.FirebaseUserId = firebaseUser.UserId;
        }
    }

    // �Խ�Ʈ�α��� ��û
    void onGuestLogin()
    {
        if (firebaseUser == null)
        {
            Debug.Log("##[Firenase] �Խ�Ʈ �α��� �õ� !!");
            firebaseAuth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("##[Firenase] SignInAnonymouslyAsync was cancled !!");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("##[Firenase] SignInAnonymouslyAsync encountered an error : " + task.Exception);
                    return;
                }

                firebaseUser = task.Result;
                Debug.Log($"##[Firenase] SignInAnonymouslyAsync was cancled !! Successfully : Provider[{firebaseUser.ProviderId}] UserID[{firebaseUser.UserId}]");
                onLoginComplete();
            });
        }
        else
        {
            Debug.Log("##[Firebase Auth] �̹� �Խ�Ʈ �α��� ����");
        }
    }

    // �α��� �Ϸ�
    void onLoginComplete()
    {
        // ID ���
        txtGuestUID.text = firebaseUser.UserId;
        UserDataMgr.Inst.Init(firebaseUser.UserId);
        OnClick_Load();
    }

    // ������ �б�
    public void OnClick_Load()
    {
        UserDataMgr.Inst.ReadUserInfoData((result) =>
        {
            Debug.Log("## ������ �б� : " + result);

            inputID.text = UserDataMgr.Inst.MyUserInfo.ID;
            inputName.text = UserDataMgr.Inst.MyUserInfo.Name;
            inputScore.text = UserDataMgr.Inst.MyUserInfo.Score.ToString();
        });
    }

    // ������ ����
    public void OnClick_Save()
    {
        UserDataMgr.Inst.SetUserInfo(inputID.text, inputName.text, int.Parse(inputScore.text));
    }
}
