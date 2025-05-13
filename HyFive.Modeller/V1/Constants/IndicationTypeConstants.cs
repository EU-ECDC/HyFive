using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Constants
{
    [TsClass(IncludeNamespace = false)]
    public static class IndicationTypeConstants
    {
        [TsProperty(Constant = true)]
        public static string BeforePatient = "BEFORE_PATIENT";
        [TsProperty(Constant = true)]
        public static string AsepticProcedures = "ASEPTIC_PROCEDURES";
        [TsProperty(Constant = true)]
        public static string BodyFluid = "BODY_FLUID";
        [TsProperty(Constant = true)]
        public static string AfterPatient = "AFTER_PATIENT";
    }
}