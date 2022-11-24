using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIUserData : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtName = null;

    GameObject myParent = null;
    int myIdx = -1;

    public void Init(GameObject parent, string strName, int idx)
    {
        myParent = parent;
        txtName.text = strName;
        myIdx = idx;
    }

    // ³ª¸¦ ´­·¶´Ù...
    public void OnClick_Me()
    {
        myParent.SendMessage("selectUserData", myIdx, SendMessageOptions.DontRequireReceiver);
    }
}
