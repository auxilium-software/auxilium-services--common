using System.ComponentModel.DataAnnotations;

namespace AuxiliumSoftware.AuxiliumServices.Common.EntityFramework.EntityModels
{
    public class WemwbsAssessmentEntityModel
    {
        /// <summary>
        /// The unique identifier for the WEMWBS Assessment.
        /// </summary>
        [Key]
        public required Guid Id { get; set; }
        /// <summary>
        /// The timestamp of when the WEMWBS Assessment was created.
        /// </summary>
        public required DateTime CreatedAtUtc { get; set; }
        /// <summary>
        /// The unique identifier of the User who created/completed the WEMWBS Assessment.
        /// </summary>
        public required Guid CreatedBy { get; set; }



        public required int OptimismScore { get; set; }
        public required int UsefulnessScore { get; set; }
        public required int RelaxedScore { get; set; }
        public required int InterestedInPeopleScore { get; set; }
        public required int SpareEnergyScore { get; set; }
        public required int ProblemHandlingScore { get; set; }
        public required int ClearThoughtScore { get; set; }
        public required int FeelingGoodSelfScore { get; set; }
        public required int FeelingCloseToPeopleScore { get; set; }
        public required int ConfidenceScore { get; set; }
        public required int MakingUpOwnMindScore { get; set; }
        public required int FeelingLovedScore { get; set; }
        public required int InterestedInNewThingsScore { get; set; }
        public required int FeelingCheerfulScore { get; set; }



        /// <summary>
        /// The User who completed the WEMWBS Assessment.
        /// </summary>
        public UserEntityModel? CreatedByUser { get; set; }
    }
}
