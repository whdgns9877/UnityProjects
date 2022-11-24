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
        // 파이어베이스 초기화
        Debug.Log("## Initialize Firebase ...");
        // Firebase 관련 의존성이 있는 모든 파일들이 존재하는지 체크
        // 만약 문제가 있다면 fix 시도..
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available)
            {
                Debug.Log("## [Firebase] Setting up Firebase Auth!!!");
                firebaseAuth = FirebaseAuth.DefaultInstance;
                firebaseAuth.StateChanged += onFirebaseAuthStateChanged;
                onFirebaseAuthStateChanged(this, null);

                // 현재 인증된 유저가 없다면 익명인증을 바로 시작
                if (firebaseAuth.CurrentUser == null)
                {
                    Debug.Log("##[Firebase] 인증된 유저가 없으면 일단 게스트를 접속");
                    onGuestLogin();
                }
                else
                {
                    // Firebase Unity SKD is not safe to use here.
                    Debug.Log("##[Firebase] 인증된 유저가 있습니다 : " + firebaseAuth.CurrentUser.UserId);
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

    // Firebase 인증 상태 변경
    void onFirebaseAuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Debug.Log("## onFirebaseAuthStateChanged");
        if (firebaseAuth.CurrentUser != firebaseUser) // 유저의 잉ㄴ증상태가 바뀌는 경우...
        {
            // 새로운 유저로 로그인했다는 의미
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
            // 데이터를 읽고 쓰기할 때 필요한
            UserDataMgr.Inst.FirebaseUserId = firebaseUser.UserId;
        }
    }

    // 게스트로그인 요청
    void onGuestLogin()
    {
        if (firebaseUser == null)
        {
            Debug.Log("##[Firenase] 게스트 로그인 시도 !!");
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
            Debug.Log("##[Firebase Auth] 이미 게스트 로그인 상태");
        }
    }

    // 로그인 완료
    void onLoginComplete()
    {
        // ID 출력
        txtGuestUID.text = firebaseUser.UserId;
        UserDataMgr.Inst.Init(firebaseUser.UserId);
        OnClick_Load();
    }

    // 데이터 읽기
    public void OnClick_Load()
    {
        UserDataMgr.Inst.ReadUserInfoData((result) =>
        {
            Debug.Log("## 데이터 읽기 : " + result);

            inputID.text = UserDataMgr.Inst.MyUserInfo.ID;
            inputName.text = UserDataMgr.Inst.MyUserInfo.Name;
            inputScore.text = UserDataMgr.Inst.MyUserInfo.Score.ToString();
        });
    }

    // 데이터 쓰기
    public void OnClick_Save()
    {
        UserDataMgr.Inst.SetUserInfo(inputID.text, inputName.text, int.Parse(inputScore.text));
    }
}
