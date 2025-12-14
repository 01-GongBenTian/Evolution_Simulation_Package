using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenesCode
{
    public enum Taxonomy
    {
        KINGDOM = 0,
        PHYLUM,
        CLASS,
        ORDER,
        FAMILY,
        GENUS,
        SPECIES,
        SUB_SPECIES,
        NUM_OF_TAXONOMY
    }

    private static readonly int[] EXP_REQUIREMENTS = { 2187, 729, 243, 81, 27, 9, 3, 1};
    public static int[] CODE_COUNT = new int[(int)Taxonomy.NUM_OF_TAXONOMY] {1, 1, 1, 1, 1, 1, 1, 1};

    public int[] Code;

    public int EvoEXP = 0;

    public GenesCode Evolute()
    {
        GenesCode newCode = new GenesCode();
        newCode.Code = new int[(int)Taxonomy.NUM_OF_TAXONOMY];

        newCode.EvoEXP += this.EvoEXP + 1;
        for (int i = 0; i < (int)Taxonomy.NUM_OF_TAXONOMY; ++i)
        {
            if ((newCode.EvoEXP / EXP_REQUIREMENTS[i]) > (this.EvoEXP / EXP_REQUIREMENTS[i]))
            {
                for(int ii = i; ii < (int)Taxonomy.NUM_OF_TAXONOMY; ++ii)
                {
                    newCode.Code[ii] = ++CODE_COUNT[ii];
                }

                break;
            }

            newCode.Code[i] = this.Code[i];
        }

        return newCode;
    }

    public string GetCode()
    {
        return string.Format("{0:0}.{1:0}.{2:0}.{3:0}.{4:0}.{5:0}.{6:0}.{7:0}",
            Code[(int)Taxonomy.KINGDOM],
            Code[(int)Taxonomy.PHYLUM],
            Code[(int)Taxonomy.CLASS],
            Code[(int)Taxonomy.ORDER],
            Code[(int)Taxonomy.FAMILY],
            Code[(int)Taxonomy.GENUS],
            Code[(int)Taxonomy.SPECIES],
            Code[(int)Taxonomy.SUB_SPECIES]);
    }

    public Color GetCodeColor()
    {
        float r = (Code[0] * Code[4] * Code[3]) % 256 / 255f;
        float g = (Code[1] * Code[5] * Code[7]) % 256 / 255f;
        float b = (Code[2] * Code[6] * Code[3] * Code[7]) % 256 / 255f;

        return new Color(r, g, b);
    }
}
