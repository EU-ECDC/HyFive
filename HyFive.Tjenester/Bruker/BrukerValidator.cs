using System;

namespace HyFive.Tjenester.Bruker
{
    public class BrukerValidator
    {
        public static bool HarNavnOgHprNummerEllerGyldigPseudonym(Modeller.V1.Bruker.Bruker bruker)
        {
            return !string.IsNullOrEmpty(bruker.Fornavn)
                   && !string.IsNullOrEmpty(bruker.Etternavn)
                   && (!string.IsNullOrEmpty(bruker.HPRNummer) || ErGyldigIdentPseudonym(bruker.IdentPseudonym));
        }

        public static bool ErGyldigIdentPseudonym(string pseudonym)
        {
            return IsBase64String(pseudonym) && pseudonym?.Length == 44;
        }

        public static bool IsBase64String(string base64)
        {
            var buffer = new Span<byte>(new byte[base64.Length]);
            var isBase64String = Convert.TryFromBase64String(base64, buffer, out var converted);
            return isBase64String;
        }
    }
}
