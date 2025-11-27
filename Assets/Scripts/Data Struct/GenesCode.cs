using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int[] Code = new int[(int)Taxonomy.NUM_OF_TAXONOMY];

    public int EvoEXP = 0;

    public GenesCode Evolute (GenesCode oldCode)
    {
        GenesCode newCode = new GenesCode();

        newCode.EvoEXP += oldCode.EvoEXP + 1;
        for (int i = 0; i < (int)Taxonomy.NUM_OF_TAXONOMY; ++i)
        {
            if ((newCode.EvoEXP / EXP_REQUIREMENTS[i]) > (oldCode.EvoEXP / EXP_REQUIREMENTS[i]))
            {
                for(int ii = i; ii < (int)Taxonomy.NUM_OF_TAXONOMY; ++ii)
                {
                    newCode.Code[ii] = ++CODE_COUNT[ii];
                }

                break;
            }

            newCode.Code[i] = oldCode.Code[i];
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
}
