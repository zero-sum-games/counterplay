using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeModifier : MonoBehaviour
{
    public TextAsset dataModifiers;
    public Type type;

    public List<Type> types = new List<Type>();


    public struct Type
    {
        public string state;
        public int[] atkRange;
        public float atkDmg;
        //float atkDef;
        public float[] moveRange;
    }

    private void Start()
    {
        string[] data = dataModifiers.text.Split(new char[] { '\n' });
        for(int i=1; i <= data.Length-1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });
            Type t = new Type();
            t.state = row[0];
            float.TryParse(row[1], out t.atkDmg);

            //Build movement modifiers based on tileType
            t.moveRange = new float[4];
            float.TryParse(row[2], out t.moveRange[0]);
            float.TryParse(row[3], out t.moveRange[1]);
            float.TryParse(row[4], out t.moveRange[2]);
            float.TryParse(row[5], out t.moveRange[3]);

            t.atkRange = new int[4];
            int.TryParse(row[6], out t.atkRange[0]);
            int.TryParse(row[7], out t.atkRange[1]);
            int.TryParse(row[8], out t.atkRange[2]);
            int.TryParse(row[9], out t.atkRange[3]);
            
            types.Add(t);

        }
        /*
         * <Summary>
         * For debuging purposes only
         * <Summary>
        foreach (Type t in types)
        {
            Debug.Log(t.state + t.atkRange + t.atkDmg + "moveRange list length is " + t.moveRange.Length + "\n");
            foreach (float v in t.moveRange)
            {
                Debug.Log(v);
            }   
        }
        */
    }

    


}
