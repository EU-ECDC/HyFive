using System;
using System.Collections.Generic;
using Reinforced.Typings.Attributes;

namespace HyFive.Modeller.V1.Konstanter
{
    [TsClass(IncludeNamespace = false)]
    public class OverforingstatusTypeKonstanter
    {
        [TsProperty(Constant = true)]
        public const string OverfortTilFhi = "OVERFORT_TIL_FHI";
        [TsProperty(Constant = true)]
        public const string OverfortTilKoordinator = "OVERFORT_TIL_KOORDINATOR";

        [TsIgnore]
        public static IEnumerable<string> HentOverforingsstatusTyper(string overforingsstatusType)
        {
            if (overforingsstatusType == null)
                throw new ArgumentException("OverforingsstatusType må ha en verdi");
            if (overforingsstatusType == OverforingstatusTypeKonstanter.OverfortTilKoordinator)
            {
                yield return OverfortTilKoordinator;
                yield return OverfortTilFhi;
                yield break;
            }
            yield return overforingsstatusType;
        }
    }
}