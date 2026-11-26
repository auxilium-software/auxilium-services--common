using AuxiliumSoftware.AuxiliumServices.Common.Enumerators;
using System.Security.Cryptography;
using System.Text;

namespace AuxiliumSoftware.AuxiliumServices.Common.Utilities
{
    /// <summary>
    /// Utilities related to generating and parsing UUIDs.
    /// </summary>
    public static class UUIDUtilities
    {
        /// <summary>
        /// Takes in a DatabaseObjectType and generates a Version 5 UUID
        /// </summary>
        /// <remarks>
        /// The "WithinTenancy" segment in these paths is a leftover from the multi-tenancy era.
        /// It is kept deliberately: changing a path changes the namespace UUID every new ID of
        /// that type is generated in, which would split existing and new rows across two namespaces.
        /// </remarks>
        private static readonly Dictionary<DatabaseObjectTypeEnum, string> NamespacePaths = new()
        {
            [DatabaseObjectTypeEnum.Calendar_Event]                                    = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Calendar.Event",
            [DatabaseObjectTypeEnum.Calendar_EventInvite]                              = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Calendar.EventInvite",



            [DatabaseObjectTypeEnum.System_Waf_IpWhitelistEntry]                       = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.Waf.IpWhitelistEntry",
            [DatabaseObjectTypeEnum.System_Waf_IpBlacklistEntry]                       = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.Waf.IpBlacklistEntry",
            [DatabaseObjectTypeEnum.System_Waf_UserWhitelistEntry]                     = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.Waf.UserWhitelistEntry",
            [DatabaseObjectTypeEnum.System_Waf_UserBlacklistEntry]                     = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.Waf.UserBlacklistEntry",
            
            [DatabaseObjectTypeEnum.System_BulletinEntry]                              = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.BulletinEntry",
            [DatabaseObjectTypeEnum.System_MetricEntry]                                = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.MetricEntry",
            [DatabaseObjectTypeEnum.System_SettingEntry]                               = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.System.SettingEntry",
            


            [DatabaseObjectTypeEnum.User]                                              = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User",
            [DatabaseObjectTypeEnum.User_AdditionalProperty]                           = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.AdditionalProperty",
            [DatabaseObjectTypeEnum.User_AdditionalProperty_Group]                     = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.AdditionalProperty.Group",
            [DatabaseObjectTypeEnum.User_AdditionalProperty_Group_Translation]         = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.AdditionalProperty.Group.Translation",
            [DatabaseObjectTypeEnum.User_AdditionalProperty_Definition]                = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.AdditionalProperty.Definition",
            [DatabaseObjectTypeEnum.User_AdditionalProperty_Definition_Translation]    = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.AdditionalProperty.Definition.Translation",
            [DatabaseObjectTypeEnum.User_AdditionalProperty_Definition_Alias]          = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.AdditionalProperty.Definition.Alias",
            [DatabaseObjectTypeEnum.User_File]                                         = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.File",
            [DatabaseObjectTypeEnum.User_RefreshToken]                                 = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.RefreshToken",
            [DatabaseObjectTypeEnum.User_TotpRecoveryCode]                             = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.TotpRecoveryCode",
            [DatabaseObjectTypeEnum.User_WemwbsAssessment]                             = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.WemwbsAssessment",
            [DatabaseObjectTypeEnum.User_PasswordSetToken]                             = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.User.PasswordSetToken",
            


            [DatabaseObjectTypeEnum.Case]                                              = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case",
            [DatabaseObjectTypeEnum.Case_AdditionalProperty]                           = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.AdditionalProperty",
            [DatabaseObjectTypeEnum.Case_AdditionalProperty_Group]                     = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.AdditionalProperty.Group",
            [DatabaseObjectTypeEnum.Case_AdditionalProperty_Group_Translation]         = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.AdditionalProperty.Group.Translation",
            [DatabaseObjectTypeEnum.Case_AdditionalProperty_Definition]                = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.AdditionalProperty.Definition",
            [DatabaseObjectTypeEnum.Case_AdditionalProperty_Definition_Translation]    = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.AdditionalProperty.Definition.Translation",
            [DatabaseObjectTypeEnum.Case_AdditionalProperty_Definition_Alias]          = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.AdditionalProperty.Definition.Alias",
            [DatabaseObjectTypeEnum.Case_Worker]                                       = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.Worker",
            [DatabaseObjectTypeEnum.Case_Client]                                       = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.Client",
            [DatabaseObjectTypeEnum.Case_Message]                                      = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.Message",
            [DatabaseObjectTypeEnum.Case_File]                                         = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.File",
            [DatabaseObjectTypeEnum.Case_TimelineEntry]                                = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.TimelineEntry",
            [DatabaseObjectTypeEnum.Case_Todo]                                         = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Case.Todo",



            [DatabaseObjectTypeEnum.Enumerator]                                        = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Enumerator",
            [DatabaseObjectTypeEnum.Enumerator_Translation]                            = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Enumerator.Translation",
            [DatabaseObjectTypeEnum.Enumerator_Value]                                  = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Enumerator.Value",
            [DatabaseObjectTypeEnum.Enumerator_ValueTranslation]                       = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Enumerator.ValueTranslation",



            [DatabaseObjectTypeEnum.Log_CaseModification_EventEntry]                   = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.CaseModification.EventEntry",
            [DatabaseObjectTypeEnum.Log_CaseMessageReadBy_EventEntry]                  = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.CaseMessageReadBy.EventEntry",
            [DatabaseObjectTypeEnum.Log_CaseMerge_EventEntry]                          = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.CaseMerge.EventEntry",
            [DatabaseObjectTypeEnum.Log_CaseMerge_RowChange_EventEntry]                = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.CaseMerge.RowChange.EventEntry",
            [DatabaseObjectTypeEnum.Log_UserMerge_EventEntry]                          = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.UserMerge.EventEntry",
            [DatabaseObjectTypeEnum.Log_UserMerge_RowChange_EventEntry]                = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.UserMerge.RowChange.EventEntry",

            [DatabaseObjectTypeEnum.Log_LoginAttempt_EventEntry]                       = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.LoginAttempt.EventEntry",

            [DatabaseObjectTypeEnum.Log_SystemMessageQueue_EmailSent_EventEntry]       = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.System.MessageQueue.EmailSent.EventEntry",
            [DatabaseObjectTypeEnum.Log_SystemMessageQueue_FailedAction_EventEntry]    = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.System.MessageQueue.FailedAction.EventEntry",

            [DatabaseObjectTypeEnum.Log_SystemBulletin_EntryDismissal_EventEntry]      = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.System.BulletinEntry.Dismissal.EventEntry",
            [DatabaseObjectTypeEnum.Log_SystemBulletin_EntryView_EventEntry]           = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.System.BulletinEntry.View.EventEntry",

            [DatabaseObjectTypeEnum.Log_UserModification_EventEntry]                   = "Auxilium.3.DatabaseObject.MariaDb.WithinTenancy.Log.UserModification.EventEntry",
        };


        /// <summary>
        /// Takes in a DatabaseObjectType and gets the precomputed Namespace UUID for it.
        /// </summary>
        private static readonly Dictionary<DatabaseObjectTypeEnum, Guid> NamespaceUuids =
            NamespacePaths.ToDictionary(
                kvp => kvp.Key,
                kvp => PathToUuid(kvp.Value)
            );

        /// <summary>
        /// Generates a Version 5 UUID for the given DatabaseObjectType.
        /// </summary>
        /// <param name="objectType">The DatabaseObjectType to use</param>
        /// <returns>A UUID (Guid).</returns>
        public static Guid GenerateV5(DatabaseObjectTypeEnum objectType)
        {
            var namespaceId = NamespaceUuids[objectType];
            var name = $"{objectType}_{DateTime.UtcNow.Ticks}_{Guid.NewGuid()}";

            return GenerateV5(namespaceId, name);
        }

        /// <summary>
        /// Generates a Version 5 UUID given a namespace UUID and a name.
        /// </summary>
        /// <param name="namespaceId">The UUID (Guid) that represents the namespace to use.</param>
        /// <param name="name">The name to use.</param>
        /// <returns>A UUID (Guid).</returns>
        public static Guid GenerateV5(Guid namespaceId, string name)
        {
            var namespaceBytes = namespaceId.ToByteArray();
            var nameBytes = Encoding.UTF8.GetBytes(name);

            SwapByteOrder(namespaceBytes);

            var hash = SHA1.HashData(namespaceBytes.Concat(nameBytes).ToArray());

            var newGuid = new byte[16];
            Array.Copy(hash, 0, newGuid, 0, 16);

            newGuid[6] = (byte)(newGuid[6] & 0x0F | 0x50);
            newGuid[8] = (byte)(newGuid[8] & 0x3F | 0x80);

            SwapByteOrder(newGuid);

            return new Guid(newGuid);
        }

        /// <summary>
        /// Converts a path string to a Version 5 UUID using SHA-1 hashing.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Guid PathToUuid(string path)
        {
            var hash = SHA1.HashData(Encoding.UTF8.GetBytes(path));

            var uuid = new byte[16];
            Array.Copy(hash, 0, uuid, 0, 16);

            uuid[6] = (byte)(uuid[6] & 0x0F | 0x50);
            uuid[8] = (byte)(uuid[8] & 0x3F | 0x80);

            return new Guid(uuid);
        }

        /// <summary>
        /// Takes in a GUID byte array and swaps the byte order to match RFC 4122.
        /// </summary>
        /// <param name="guid">A 16-byte array representing a GUID. The array is modified in place.</param>
        private static void SwapByteOrder(byte[] guid)
        {
            SwapBytes(guid, 0, 3);
            SwapBytes(guid, 1, 2);
            SwapBytes(guid, 4, 5);
            SwapBytes(guid, 6, 7);
        }

        /// <summary>
        /// Swaps two bytes in a byte array.
        /// </summary>
        /// <param name="guid">The byte array to target for swapping bytes.</param>
        /// <param name="left">The index of the first byte to swap.</param>
        /// <param name="right">The index of the second byte to swap.</param>
        private static void SwapBytes(byte[] guid, int left, int right)
        {
            (guid[left], guid[right]) = (guid[right], guid[left]);
        }
    }
}

