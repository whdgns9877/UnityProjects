using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateObject : MonoBehaviour
{
    MeshFilter meshfilter;
    public int divCount = 120;
    private void Awake()
    {
        meshfilter = GetComponent<MeshFilter>();
    }

    void Start()
    {
        GenerateCube();
    }

    void GenerateCube()
    {
        // 정사각형 그리는 부분 (사각형 하나는 삼각형 2개, 정육면체는 사각형 6개, 정육면체는 총 12개의 Vertax필요)
        //Vector3[] vtxBuffer =
        //{
        //    new Vector3(-0.5f, -0.5f, -0.5f),
        //    new Vector3(-0.5f, -0.5f, 0.5f),
        //    new Vector3(-0.5f, 0.5f, -0.5f),
        //    new Vector3(-0.5f, 0.5f, 0.5f),
        //    new Vector3(0.5f, -0.5f, -0.5f),
        //    new Vector3(0.5f, -0.5f, 0.5f),
        //    new Vector3(0.5f, 0.5f, -0.5f),
        //    new Vector3(0.5f, 0.5f, 0.5f)
        //};

        //// 인덱스버퍼
        //int[] cubeIndeces =
        //{
        //    // -X
        //    0, 1, 2, // 시계방향 : 앞면, 반시계방향 : 뒷면
        //    2, 1, 3,
        //    // +X
        //    4, 6, 5,
        //    5, 6, 7,
        //    // -Y
        //    0, 5, 1,
        //    0, 4, 5,
        //    // +Y
        //    2, 7, 6,
        //    2, 3, 7,
        //    // -Z
        //    0, 6, 4,
        //    0, 2, 6,
        //    // +Z
        //    1, 7, 3,
        //    1, 5, 7,
        //};


        // Practice Make Circle Using Vertax Mesh
        Vector3[] circlevtxBuffer = new Vector3[divCount + 1];
        circlevtxBuffer[0] = Vector3.zero;

        // Two ways to find theta

        //float theta = 2f * Mathf.PI / divCount;
        float theta = (360 / divCount) * Mathf.Deg2Rad;

        for (int i = 0; i < divCount; i++)
        {
            circlevtxBuffer[i + 1] = new Vector3(Mathf.Cos(theta * i), Mathf.Sin(theta * i), 0f);
        }



        int[] circleIndeces = new int[divCount * 3];
        int j = 0;
        for (int i = 0; i < divCount; i++)
        {
            circleIndeces[j] = 0;
            circleIndeces[j + 1] = i + 2;
            if (circleIndeces[j + 1] > divCount)
                circleIndeces[j + 1] = (i + 2) % (divCount + 1) + 1;
            circleIndeces[j + 2] = i + 1;
            if (circleIndeces[j + 2] > divCount)
                circleIndeces[j + 2] = (i + 1) % (divCount) + 1;

            j += 3;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = circlevtxBuffer;
        mesh.triangles = circleIndeces;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        meshfilter.mesh = mesh;
    }
}
