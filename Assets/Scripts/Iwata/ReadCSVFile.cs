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

        TextAsset _csvFile = Resources.Load(FileName) as TextAsset; // Resouces下のCSV読み込み
        StringReader reader = new StringReader(_csvFile.text);

        while (reader.Peek() != -1) // reader.Peaekが-1になるまで
        {
            string line = reader.ReadLine(); // 一行ずつ読み込み



            List<string> reverseLine = new List<string>();
            reverseLine = line.Split(',').ToList();
            //string temp_flagName = reverseLine[0];  // フラグ名をよせておく
            //reverseLine.RemoveAt(0); // フラグ名を最後尾にする
            //reverseLine.Add(temp_flagName);
            reverseLine.Reverse();  // CSVの並びをひっくり返す
            string joinLine = string.Join(",", reverseLine);



            _csvDatas.Add(joinLine.Split(',')); // , 区切りでリストに追加
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
