using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public static class HandJewelryTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string AllClear = "ALL_CLEAR";

        [TsProperty(Constant = true)]
        public static string Ring = "RING";

        [TsProperty(Constant = true)]
        public static string WatchBracelet = "WATCH_BRACELET";

        [TsProperty(Constant = true)]
        public static string LongNails = "LONG_NAILS";

        [TsProperty(Constant = true)]
        public static string ArtificialNailsShellac = "ARTIFICIAL_NAILS_SHELLAC";

        [TsProperty(Constant = true)]
        public static string ShortSleeved = "SHORT_SLEEVED";

        [TsProperty(Constant = true)]
        public static string LongSleeved = "LONG_SLEEVED";
    }
}