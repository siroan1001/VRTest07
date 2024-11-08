using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ReadCSVFile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<string[]> ReadCSV(string FileName)
    {
        List<string[]> _csvDatas = new List<string[]>();

        TextAsset _csvFile = Resources.Load(FileName) as TextAsset; // Resouces����CSV�ǂݍ���
        StringReader reader = new StringReader(_csvFile.text);

        while (reader.Peek() != -1) // reader.Peaek��-1�ɂȂ�܂�
        {
            string line = reader.ReadLine(); // ��s���ǂݍ���



            List<string> reverseLine = new List<string>();
            reverseLine = line.Split(',').ToList();
            //string temp_flagName = reverseLine[0];  // �t���O�����悹�Ă���
            //reverseLine.RemoveAt(0); // �t���O�����Ō���ɂ���
            //reverseLine.Add(temp_flagName);
            reverseLine.Reverse();  // CSV�̕��т��Ђ�����Ԃ�
            string joinLine = string.Join(",", reverseLine);



            _csvDatas.Add(joinLine.Split(',')); // , ��؂�Ń��X�g�ɒǉ�
        }

        //for(int i = 0; i < _csvDatas.Count; i++)
        //{
        //    for(int j = 0; j < _csvDatas[i].Length; j++)
        //    {
        //        Debug.Log("[" + i + "] [" + j + "] " + _csvDatas[i][j]);
        //    }
        //}

        return _csvDatas;
    }
}
