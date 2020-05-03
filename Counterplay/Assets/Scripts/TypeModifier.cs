using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================================================================
public class TypeModifier : MonoBehaviour
{
    //==========================================================================
    public struct Type
    {
        public string state;
        public int[] atkRange;
        public float atkDmg;
        public float[] moveRange;
    }

    public Type type;
    public List<Type> types = new List<Type>();

    public TextAsset dataModifiers;

    //==========================================================================
    private void Start()
    {
        string[] data = dataModifiers.text.Split(new char[] { '\n' });
        for(int i=1; i <= data.Length-1; i++)
        {
            string[] row = data[i].Split(new char[] { ',' });

            for (int j = 0; j < row.Length; j++)
                row[j] = row[j].Trim();

            Type t = new Type();

            t.state = row[0];

            t.moveRange = new float[7];
            float.TryParse(row[1], out t.moveRange[0]);
            float.TryParse(row[2], out t.moveRange[1]);
            float.TryParse(row[3], out t.moveRange[2]);
            float.TryParse(row[4], out t.moveRange[3]);
            float.TryParse(row[5], out t.moveRange[4]);
            float.TryParse(row[6], out t.moveRange[5]);
            float.TryParse(row[7], out t.moveRange[6]);

            t.atkRange = new int[7];
            int.TryParse(row[8],  out t.atkRange[0]);
            int.TryParse(row[9],  out t.atkRange[1]);
            int.TryParse(row[10], out t.atkRange[2]);
            int.TryParse(row[11], out t.atkRange[3]);
            int.TryParse(row[12], out t.atkRange[4]);
            int.TryParse(row[13], out t.atkRange[5]);
            int.TryParse(row[14], out t.atkRange[6]);
            
            types.Add(t);

        }
    }
}
