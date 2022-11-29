using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class BigDigitQuad : MonoBehaviour
{
    int CW = 3;
    int CH = 3;
    int BH = 9;
    int BW = 9;

    int[,] MyDigits;
    int[] digits;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void makeQuad()
    {
        /// ù ��° 3x3 ���� ä���
        int idx = 0;

        for(int h = 0; h < CH; ++h)
        {
            for(int w = 0; h < CW; ++w)
            {
                MyDigits[h, w] = digits[idx++];
            }
        }

        // 3X3 9���� �� 1��°�� �̿��ؼ� 2,3��° ä���
        int sYidx = 1, sXidx = 3;
        int curYidx = 0, curXidx = 0;
        
        for(int i = 1; i <= 2; ++i)
        {
            idx = 0;
            sYidx = i;
            sXidx = i * 3;
            for(int h = 0; h < CH; ++h)
            {
                for(int w = 0; w < CW; ++w)
                {
                    curYidx = (h + sYidx) % 3;
                    MyDigits[curYidx, w + sXidx] = digits[idx++];
                }
            }
        }

        // 3x3 9�� �� �� 3���� �߰� 3�� (+ �ǾƷ�����)�� �����ϵ� ��ĭ ����������
    }
}
