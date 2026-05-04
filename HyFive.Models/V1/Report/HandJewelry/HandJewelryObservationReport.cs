using System;

namespace HyFive.Models.V1.Report.HandJewelry
{
	/// <summary>
	/// Represents a hand jewelry observation report including session and observation metadata
	/// and related facility/observer information.
	/// </summary>
	public class HandJewelryObservationReport
	{
		/// <summary>
		/// Identifier for the session that contains the observation.
		/// </summary>
		public Guid SessionId { get; set; }

		/// <summary>
		/// Identifier for the individual observation.
		/// </summary>
		public Guid ObservationId { get; set; }

		/// <summary>
		/// Creation time of the session (string representation, e.g. ISO 8601).
		/// </summary>
		public string SessionCreationTime { get; set; }

		/// <summary>
		/// Registration time of the observation (string representation, e.g. ISO 8601).
		/// </summary>
		public string ObservationRegistrationTime { get; set; }

		/// <summary>
		/// Name or identifier of the observer who recorded the observation.
		/// </summary>
		public string Observer { get; set; }

		/// <summary>
		/// Abbreviation for the facility where the observation occurred.
		/// </summary>
		public string FacilityAbbreviation { get; set; }

		/// <summary>
		/// Full name of the facility where the observation occurred.
		/// </summary>
		public string Facility { get; set; }

		/// <summary>
		/// Type of the facility (e.g. hospital, clinic).
		/// </summary>
		public string FacilityType { get; set; }

		/// <summary>
		/// Code representing the facility type.
		/// </summary>
		public string FacilityTypeCode { get; set; }

		/// <summary>
		/// Department within the facility where the observation occurred.
		/// </summary>
		public string Department { get; set; }

		/// <summary>
		/// Type of the department (if applicable).
		/// </summary>
		public string DepartmentType { get; set; }

		/// <summary>
		/// Role name of the person observed (e.g. nurse, doctor).
		/// </summary>
		public string RoleName { get; set; }

		/// <summary>
		/// Transfer status associated with the observation.
		/// </summary>
		public string TransferStatus { get; set; }

		/// <summary>
		/// Comments specific to the observation.
		/// </summary>
		public string ObservationComment { get; set; }

		/// <summary>
		/// Comments associated with the session.
		/// </summary>
		public string SessionComment { get; set; }

		/// <summary>
		/// Indicates whether the subject was bare below the elbows.
		/// </summary>
		public string BareBelowElbows { get; set; }

		/// <summary>
		/// Type codes related to the bare below elbows observation.
		/// </summary>
		public string BareBelowElbowsTypeCodes { get; set; }

		/// <summary>
		/// City where the facility is located.
		/// </summary>
		public string City { get; set; }
	}
}
